using gRPC.Common.Protos;
using Grpc.Core;

namespace Server.Bit.Services
{
    public class PredictData : PredictionDataService.PredictionDataServiceBase, IPredictData
    {
        public async Task<BaseResponse> SavePredictionHourlyConsumption(SaveHourlyConsumptionRequest request, ServerCallContext context)
        {

        }
    }
}
