using gRPC.Common.Protos;
using Grpc.Core;

namespace Server.Core.gRPC.Client
{
    public interface ICentralService
    {
        Task<BaseResponse> SendDataToServer(CentralServerRequest request, ServerCallContext context, string serverAddress);
        Task<BaseResponse> SendDataToClient(ClientServerRequest request, ServerCallContext context, string serverAddress);
        Task<DateTimeValueResponse> SendDataToAi(HourlyConsumptionRequest request, ServerCallContext context, string serverAddress);
    }
}
