using gRPC.Common.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;

namespace Server.Core.gRPC.Client
{
    public class DomainClientDataService : IDomainClientDataService
    {
        private readonly ILogger<DomainClientDataService> _logger;

        public DomainClientDataService(ILogger<DomainClientDataService> logger)
        {
            _logger = logger;
        }

        public async Task<DateTimeValueResponse> GetHourlyConsumption(HourlyConsumptionRequest request, ServerCallContext context, string serverAddress)
        {
            using var channel = GrpcChannel.ForAddress(serverAddress);
            var client = new ClientDataService.ClientDataServiceClient(channel);
            var response = await client.GetHourlyConsumptionAsync(request);
            return response;
        }

        public async Task<BaseResponse> SaveHourlyConsumption(SaveHourlyConsumptionRequest request, ServerCallContext context, string serverAddress)
        {
            using var channel = GrpcChannel.ForAddress(serverAddress);
            var client = new ClientDataService.ClientDataServiceClient(channel);
            var response = await client.SaveHourlyConsumptionAsync(request);
            return response;
        }
    }
}
