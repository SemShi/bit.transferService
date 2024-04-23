using gRPC.Common.Protos;
using Grpc.Core;
using Server.Core.Services;

namespace Server.Client.Services
{
    public class ClientService : ChildServiceGrpc.ChildServiceGrpcBase, IClientService
    {
        private readonly ILogger<ClientService> _logger;
        private readonly IConfiguration _cfg;
        private readonly INormalizeDataService _normalizeDataService;
        private readonly ISqliteService _databaseService;

        public ClientService(
            ILogger<ClientService> logger, 
            IConfiguration cfg, 
            INormalizeDataService normalizeDataService, 
            ISqliteService databaseService)
        {
            _logger = logger;
            _cfg = cfg;
            _normalizeDataService = normalizeDataService;
            _databaseService = databaseService;
        }

        public override Task<BaseResponse> SaveConsumptionResult(ClientServerRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }
    }
}
