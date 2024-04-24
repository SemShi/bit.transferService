using gRPC.Common.Protos;
using Grpc.Core;

namespace Server.Bit.Services
{
    public class CentralService : CentralServiceGrpc.CentralServiceGrpcBase, ICentralService
    {
        private readonly ILogger<CentralService> _logger;
        private readonly IConfiguration _cfg;
        private readonly ICentralService _centralService;

        public CentralService(ILogger<CentralService> logger, IConfiguration cfg, ICentralService centralService)
        {
            _logger = logger;
            _cfg = cfg;
            _centralService = centralService;
        }

        public override Task<BaseResponse> PredictConsumptionOnServer(CentralServerRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }
    }
}
