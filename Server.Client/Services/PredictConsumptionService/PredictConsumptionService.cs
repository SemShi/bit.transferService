using Google.Protobuf.Collections;
using gRPC.Common;
using gRPC.Common.Protos;
using Grpc.Core;
using Server.Core;
using Server.Core.gRPC.Client;
using Server.Core.Services;

namespace Server.Client.Services
{
    public class PredictConsumptionService : ConsumptionService.ConsumptionServiceBase, IPredictConsumptionService
    {
        private readonly ILogger<PredictConsumptionService> _logger;
        private readonly IConfiguration _cfg;
        private readonly IDomainClientDataService _domainClientDataService;
        private readonly INormalizeDataService _normalizeDataService;
        private readonly ISqliteService _databaseService;

        public PredictConsumptionService(
            ILogger<PredictConsumptionService> logger,
            IConfiguration cfg,
            IDomainClientDataService domainClientDataService,
            INormalizeDataService normalizeDataService, 
            ISqliteService databaseService)
        {
            _logger = logger;
            _cfg = cfg;
            _domainClientDataService = domainClientDataService;
            _normalizeDataService = normalizeDataService;
            _databaseService = databaseService;
        }

        public override async Task<BaseResponse> PredictConsumption(PredictConsumptionRequest request, ServerCallContext context)
        {
            var clientServiceAddress =
                _cfg.GetSection("Servers").GetSection("ClientService").Value ?? string.Empty;

            if (clientServiceAddress == string.Empty)
                return Helpers.GetBaseResponseError("0", "");

            var requestModel = new HourlyConsumptionRequest()
            {
                DBeg = request.StartDate.AddDays(-62),
                DEnd = request.StartDate,
                MeteringPointGuid = request.MeteringPointGuid
            };

            DateTimeValueResponse clientServiceResponse;
            try
            {
                clientServiceResponse =
                    await _domainClientDataService.GetHourlyConsumption(requestModel, context, clientServiceAddress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Helpers.GetBaseResponseError("0", "");
            }

            var coefficients = await _normalizeDataService.GetMinMaxDictionary(clientServiceResponse.Result.DateTimeValue);

            if (!coefficients.Success)
                return Helpers.GetBaseResponseError("0", ((ErrorResult<RequestСoefficientMinMax>)coefficients).Message);


            coefficients.Data.Id = request.RequestGuid;

            var dbResponse = await _databaseService.AddCoefficients(coefficients.Data);

            if (!dbResponse.Success)
                return Helpers.GetBaseResponseError("0", ((ErrorResult)dbResponse).Message);

            var normalizedData =
                await _normalizeDataService.Normalize(clientServiceResponse.Result.DateTimeValue, coefficients.Data);

            if (!normalizedData.Success)
                return Helpers.GetBaseResponseError("0", ((ErrorResult<RepeatedField<DateTimeValue>>)normalizedData).Message);

            //отправляем данные на центральный сервис

            return await Task.FromResult(new BaseResponse());
        }
    }
}
