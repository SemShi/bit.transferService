using gRPC.Common.Protos;
using Grpc.Core;

namespace Server.Bit.Services.ClientData
{
    public interface IClientData
    {
        public Task<DateTimeValueResponse> GetPredictionHourlyConsumption(HourlyConsumptionRequest request, ServerCallContext context);
    }
}
