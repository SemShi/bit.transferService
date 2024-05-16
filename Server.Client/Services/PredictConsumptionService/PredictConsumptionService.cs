using Google.Protobuf.Collections;
using gRPC.Common;
using gRPC.Common.Protos;
using Grpc.Core;
using Server.Core;
using Server.Core.gRPC.Client;
using Server.Core.Services;
using String = System.String;

namespace Server.Client.Services
{
    public class PredictConsumptionService : ConsumptionService.ConsumptionServiceBase, IPredictConsumptionService
    {
        private readonly ILogger<PredictConsumptionService> _logger;
        private readonly IConfiguration _cfg;
        private readonly IDomainClientDataService _domainClientDataService;
        private readonly ICentralService _centralService;
        private readonly INormalizeDataService _normalizeDataService;
        private readonly ISqliteService _databaseService;
        protected readonly IWebHostEnvironment _hostEnvironment;

        public PredictConsumptionService(
            ILogger<PredictConsumptionService> logger,
            IConfiguration cfg,
            IDomainClientDataService domainClientDataService,
            INormalizeDataService normalizeDataService, 
            ISqliteService databaseService, ICentralService centralService, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _cfg = cfg;
            _domainClientDataService = domainClientDataService;
            _normalizeDataService = normalizeDataService;
            _databaseService = databaseService;
            _centralService = centralService;
            _hostEnvironment = hostEnvironment;
        }

        public override async Task<BaseResponse> PredictConsumption(PredictConsumptionRequest request, ServerCallContext context)
        {
            if(request.RequestGuid == String.Empty)
                return Helpers.GetBaseResponseError("0", "Empty request guid");
            if (request.MeteringPointGuid == String.Empty)
                return Helpers.GetBaseResponseError("0", "Empty meteringPointGuid");

            _logger.LogInformation("Вызвана процедура {0}", "PredictConsumption");

            var clientServiceAddress =
                _cfg.GetSection("Servers").GetSection("ClientService").Value ?? string.Empty;

            if (!_hostEnvironment.IsDevelopment())
            {
                if (clientServiceAddress == string.Empty)
                    return Helpers.GetBaseResponseError("0", "");
            }

            var requestModel = new HourlyConsumptionRequest()
            {
                DBeg = request.StartDate.AddDays(-62),
                DEnd = request.StartDate,
                MeteringPointGuid = request.MeteringPointGuid
            };

            Result<DateTimeValueResponse> clientServiceResponse;
            try
            {
                if (!_hostEnvironment.IsDevelopment())
                    clientServiceResponse =
                        await _domainClientDataService.GetHourlyConsumption(requestModel, context,
                            clientServiceAddress);
                else
                    clientServiceResponse = new SuccessResult<DateTimeValueResponse>( new DateTimeValueResponse()
                        {
                            Error = null,
                            Result = new DateTimeValueResponse.Types.DateTimeValueData()
                            {
                                DateTimeValue = { Helpers.GenerateValues(requestModel.DEnd) }
                            }
                        });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Helpers.GetBaseResponseError("0", "");
            }

            var coefficients = await _normalizeDataService.GetMinMaxDictionary(clientServiceResponse.Data.Result.DateTimeValue);

            if (!coefficients.Success)
                return Helpers.GetBaseResponseError("0", ((ErrorResult<RequestСoefficientMinMax>)coefficients).Message);


            coefficients.Data.MeteringUnitGuid = request.RequestGuid;

            var dbResponse = await _databaseService.AddCoefficients(coefficients.Data);

            if (!dbResponse.Success)
                return Helpers.GetBaseResponseError("0", ((ErrorResult)dbResponse).Message);

            var normalizedData =
                await _normalizeDataService.Normalize(clientServiceResponse.Data.Result.DateTimeValue, coefficients.Data);

            if (!normalizedData.Success)
                return Helpers.GetBaseResponseError("0", ((ErrorResult<RepeatedField<DateTimeValue>>)normalizedData).Message);

            var centralServiceAddress =
                _cfg.GetSection("Servers").GetSection("CentralService").Value ?? string.Empty;

            if (centralServiceAddress == string.Empty)
                return Helpers.GetBaseResponseError("0", "");

            var serverRequest = new CentralServerRequest()
            {
                RequestGuid = request.RequestGuid,
                MeteringPointGuid = request.MeteringPointGuid,
                StartDate = request.StartDate,
                DateTimeValue = { normalizedData.Data }
            };

            var responseCentralService =
                await _centralService.SendDataToServer(serverRequest, context, centralServiceAddress);

            return responseCentralService.Data;
        }
    }
}
