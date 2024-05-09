using gRPC.Common.Protos;
using Grpc.Core;

namespace Server.Core.gRPC.Client
{
    public interface IDomainClientDataService
    {
        Task<DateTimeValueResponse> GetHourlyConsumption(HourlyConsumptionRequest request, ServerCallContext context, string serverAddress);
        Task<BaseResponse> SaveHourlyConsumption(SaveHourlyConsumptionRequest request, ServerCallContext context, string serverAddress);
    }
}