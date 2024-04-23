using gRPC.Common.Protos;
using Grpc.Core;

namespace Server.Client.Services
{
    public interface IClientService
    {
        Task<BaseResponse> SaveConsumptionResult(ClientServerRequest request, ServerCallContext context);
    }
}
