using gRPC.Common.Protos;
using Grpc.Core;
using Server.Core;
using Server.Core.Services;

namespace Server.Bit.Services
{
    public class PredictData : PredictionDataService.PredictionDataServiceBase, IPredictData
    {
        private readonly ILogger<PredictData> _logger;
        private readonly ISqliteService _databaseService;
        private readonly Core.gRPC.Client.ICentralService _centralService;
        private readonly IConfiguration _cfg;

        public PredictData(
            ILogger<PredictData> logger, 
            ISqliteService databaseService, 
            Core.gRPC.Client.ICentralService centralService, 
            IConfiguration cfg)
        {
            _logger = logger;
            _databaseService = databaseService;
            _centralService = centralService;
            _cfg = cfg;
        }

        public override async Task<BaseResponse> SavePredictionHourlyConsumption(SaveHourlyConsumptionRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Вызвана процедура {0}", "SavePredictionHourlyConsumption");
            var responseDb = await _databaseService.SavePrediction(request);
            if (!responseDb.Success)
                return Helpers.GetBaseResponseError("0", ((ErrorResult)responseDb).Message);

            var clientServiceAddress =
                _cfg.GetSection("Servers").GetSection("CentralService").Value ?? string.Empty;

            if (clientServiceAddress == string.Empty)
                return Helpers.GetBaseResponseError("0", "");

            var clientRequest = new ClientServerRequest()
            {
                MeteringPointGuid = request.MeteringPointGuid,
                DateTimeValue = { request.ConsumptionData }
            };

            var sendDataToClientResponse = 
                await _centralService.SendDataToClient(clientRequest, context, clientServiceAddress);
            return sendDataToClientResponse.Data;
        }
    }
}
