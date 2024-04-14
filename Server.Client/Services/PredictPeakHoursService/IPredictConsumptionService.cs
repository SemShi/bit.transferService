using gRPC.Common.Protos;
using Grpc.Core;

namespace Server.Client.Services
{
    public interface IPredictConsumptionService
    {
        Task<BaseResponse> PredictConsumption(PredictConsumptionRequest request, ServerCallContext context);
    }
}
