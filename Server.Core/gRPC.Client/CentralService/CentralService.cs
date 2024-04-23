using gRPC.Common.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;

namespace Server.Core.gRPC.Client.CentralService
{
    public class CentralService : ICentralService
    {
        private readonly ILogger<CentralService> _logger;

        public CentralService(ILogger<CentralService> logger)
        {
            _logger = logger;
        }

        public async Task<BaseResponse> SendDataToServer(CentralServerRequest request, ServerCallContext context, string serverAddress)
        {
            using var channel = GrpcChannel.ForAddress(serverAddress);
            var client = new CentralServiceGrpc.CentralServiceGrpcClient(channel);
            var response = await client.PredictConsumptionOnServerAsync(request);
            return response;
        }

        public async Task<BaseResponse> SendDataToClient(ClientServerRequest request, ServerCallContext context, string serverAddress)
        {
            using var channel = GrpcChannel.ForAddress(serverAddress);
            var client = new ChildServiceGrpc.ChildServiceGrpcClient(channel);
            var response = await client.SaveConsumptionResultAsync(request);
            return response;
        }

        public async Task<DateTimeValueResponse> SendDataToAi(HourlyConsumptionRequest request, ServerCallContext context, string serverAddress)
        {
            using var channel = GrpcChannel.ForAddress(serverAddress);
            var client = new ClientDataService.ClientDataServiceClient(channel);
            var response = await client.GetHourlyConsumptionAsync(request);
            return response;
        }
    }
}
