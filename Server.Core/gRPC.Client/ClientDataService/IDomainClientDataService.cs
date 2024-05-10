using gRPC.Common.Protos;
using Grpc.Core;

namespace Server.Core.gRPC.Client
{
    public interface IDomainClientDataService
    {
        Task<Result<DateTimeValueResponse>> GetHourlyConsumption(HourlyConsumptionRequest request, ServerCallContext context, string serverAddress);
        Task<Result<BaseResponse>> SaveHourlyConsumption(SaveHourlyConsumptionRequest request, ServerCallContext context, string serverAddress);
    }
}