using gRPC.Common.Protos;
using Grpc.Core;
using Server.Core;
using Server.Core.gRPC.Client;
using Server.Core.Services;

namespace Server.Client.Services
{
    public class ClientService : ChildServiceGrpc.ChildServiceGrpcBase, IClientService
    {
        private readonly ILogger<ClientService> _logger;
        private readonly IConfiguration _cfg;
        private readonly INormalizeDataService _normalizeDataService;
        private readonly ISqliteService _databaseService;
        private readonly IDomainClientDataService _domainClientDataService;
        protected readonly IWebHostEnvironment _hostEnvironment;

        public ClientService(
            ILogger<ClientService> logger, 
            IConfiguration cfg, 
            INormalizeDataService normalizeDataService, 
            ISqliteService databaseService, 
            IDomainClientDataService domainClientDataService, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _cfg = cfg;
            _normalizeDataService = normalizeDataService;
            _databaseService = databaseService;
            _domainClientDataService = domainClientDataService;
            _hostEnvironment = hostEnvironment;
        }

        public override async Task<BaseResponse> SaveConsumptionResult(ClientServerRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Вызвана процедура {0}", "SaveConsumptionResult");
            var requestCoefficients = 
                await _databaseService.GetCoefficientsByMeteringPointGuid(request.MeteringPointGuid);
            var denormalizedData =
                await _normalizeDataService.Denormalize(request.DateTimeValue, requestCoefficients.Data);

            var jsonStr = System.Text.Json.JsonSerializer.Serialize(denormalizedData.Data);

            var requestDb = new SaveHourlyConsumptionRequest()
            {
                ConsumptionData = { denormalizedData.Data },
                MeteringPointGuid = request.MeteringPointGuid,
            };
            
            var responseDb = await _databaseService.SavePrediction(requestDb);
            if (!responseDb.Success)
                return Helpers.GetBaseResponseError("0", "");

            var clientServiceAddress =
                _cfg.GetSection("Servers").GetSection("ClientService").Value ?? string.Empty;
            if (clientServiceAddress == string.Empty)
                return Helpers.GetBaseResponseError("0", "");

            var responseClientSaveData = _hostEnvironment.IsProduction() ?
                await _domainClientDataService.SaveHourlyConsumption(requestDb, context, clientServiceAddress) : new SuccessResult<BaseResponse>(Helpers.GetBaseResponseSuccess());
            if (!responseClientSaveData.Data.TaskSubmitted)
                return Helpers.GetBaseResponseError(responseClientSaveData.Data.Error.ErrorCode, responseClientSaveData.Data.Error.ErrorText);

            return Helpers.GetBaseResponseSuccess();
        }
    }
}
