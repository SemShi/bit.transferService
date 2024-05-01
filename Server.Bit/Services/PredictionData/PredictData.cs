using gRPC.Common.Protos;
using Grpc.Core;
using Server.Core;
using Server.Core.Services;
using DateTime = System.DateTime;

namespace Server.Bit.Services
{
    public class PredictData : PredictionDataService.PredictionDataServiceBase, IPredictData
    {
        private readonly ILogger<PredictData> _logger;
        private readonly ISqliteService _databaseService;

        public PredictData(ILogger<PredictData> logger, ISqliteService databaseService)
        {
            _logger = logger;
            _databaseService = databaseService;
        }

        public async Task<BaseResponse> SavePredictionHourlyConsumption(SaveHourlyConsumptionRequest request, ServerCallContext context)
        {
            //How to sava data without requestId? Wtf
            // var rows = request.ConsumptionData.Select(row => new PredictionRow(row.DateTime, row.Value, request.)
            // {
            //     DateTime = new DateTime()
            // }).ToList();
            return Helpers.GetBaseResponseSuccess();
        }
    }
}
