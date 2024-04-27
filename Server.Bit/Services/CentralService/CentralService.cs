using gRPC.Common.Protos;
using Grpc.Core;
using Server.Core;
using Server.Core.Services;

namespace Server.Bit.Services
{
    public class CentralService : CentralServiceGrpc.CentralServiceGrpcBase, ICentralService
    {
        private readonly ILogger<CentralService> _logger;
        private readonly IConfiguration _cfg;
        private readonly ICentralService _centralService;
        private readonly ISqliteService _databaseService;

        public CentralService(
            ILogger<CentralService> logger, 
            IConfiguration cfg, 
            ICentralService centralService, 
            ISqliteService databaseService)
        {
            _logger = logger;
            _cfg = cfg;
            _centralService = centralService;
            _databaseService = databaseService;
        }

        public override async Task<BaseResponse> PredictConsumptionOnServer(CentralServerRequest request, ServerCallContext context)
        {
            //Тут проверку лицензии воткнуть

            if(!request.DateTimeValue.Any())
                return Helpers.GetBaseResponseError("0", "Пустой набор данных");

            var dbResponse = _databaseService.
        }
    }
}
