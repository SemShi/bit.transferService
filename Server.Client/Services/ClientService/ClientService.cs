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

        public ClientService(
            ILogger<ClientService> logger, 
            IConfiguration cfg, 
            INormalizeDataService normalizeDataService, 
            ISqliteService databaseService, 
            IDomainClientDataService domainClientDataService)
        {
            _logger = logger;
            _cfg = cfg;
            _normalizeDataService = normalizeDataService;
            _databaseService = databaseService;
            _domainClientDataService = domainClientDataService;
        }

        public override async Task<BaseResponse> SaveConsumptionResult(ClientServerRequest request, ServerCallContext context)
        {
            var requestCoefficients = 
                await _databaseService.GetCoefficientsByRequestId(request.RequestGuid);
            var denormalizedData =
                await _normalizeDataService.Denormalize(request.DateTimeValue, requestCoefficients.Data);

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

            var responseClientSaveData =
                await _domainClientDataService.SaveHourlyConsumption(requestDb, context, clientServiceAddress);
            if (!responseClientSaveData.TaskSubmitted)
                return Helpers.GetBaseResponseError(responseClientSaveData.Error.ErrorCode, responseClientSaveData.Error.ErrorText);

            return Helpers.GetBaseResponseSuccess();
        }
    }
}
