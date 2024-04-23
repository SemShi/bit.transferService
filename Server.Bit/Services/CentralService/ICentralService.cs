using gRPC.Common.Protos;
using Grpc.Core;

namespace Server.Bit.Services
{
    public interface ICentralService
    {
        Task<BaseResponse> PredictConsumptionOnServer(CentralServerRequest request, ServerCallContext context);
        Task<BaseResponse> GetPredictedData(CentralServerRequest request, ServerCallContext context);
    }
}
