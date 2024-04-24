using gRPC.Common.Protos;
using Grpc.Core;

namespace Server.Bit.Services.ClientData
{
    public class ClientData : ClientDataService.ClientDataServiceBase, IClientData
    {
        public async Task<DateTimeValueResponse> GetPredictionHourlyConsumption(HourlyConsumptionRequest request, ServerCallContext context)
        {

        }
    }
}
