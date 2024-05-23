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
        private readonly Server.Core.gRPC.Client.ICentralService _centralService;
        private readonly ISqliteService _databaseService;
        protected readonly IWebHostEnvironment _hostEnvironment;

        public CentralService(
            ILogger<CentralService> logger, 
            IConfiguration cfg, 
            Server.Core.gRPC.Client.ICentralService centralService, 
            ISqliteService databaseService, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _cfg = cfg;
            _centralService = centralService;
            _databaseService = databaseService;
            _hostEnvironment = hostEnvironment;
        }

        public override async Task<BaseResponse> PredictConsumptionOnServer(CentralServerRequest request, ServerCallContext context)
        {
            //Тут проверку лицензии воткнуть
            _logger.LogInformation("Вызвана процедура {0}", "PredictConsumptionOnServer");
            if (!request.DateTimeValue.Any())
                return Helpers.GetBaseResponseError("0", "Пустой набор данных");
            
            var dbResponse = await _databaseService.SaveClientData(request);
            if (!dbResponse.Success)
                return Helpers.GetBaseResponseError("0", "");

            var newRequest = new PredictConsumptionRequest()
            {
                RequestGuid = request.RequestGuid,
                StartDate = request.StartDate,
                MeteringPointGuid = request.MeteringPointGuid
            };
            
            var aiServiceAddress =
                _cfg.GetSection("Servers").GetSection("AIService").Value ?? string.Empty;

            if (aiServiceAddress == string.Empty)
                return Helpers.GetBaseResponseError("0", "");

            var aiResponse = _hostEnvironment.IsProduction() ? await _centralService.SendDataToAi(newRequest, context, aiServiceAddress) : new SuccessResult<BaseResponse>(Helpers.GetBaseResponseSuccess());

            if (!aiResponse.Data.TaskSubmitted)
                return Helpers.GetBaseResponseError(aiResponse.Data.Error.ErrorCode, aiResponse.Data.Error.ErrorText);
            
            return Helpers.GetBaseResponseSuccess();
        }
    }
}
