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

        public async Task<Result<DateTimeValueResponse>> GetHourlyConsumption(HourlyConsumptionRequest request, ServerCallContext context, string serverAddress)
        {
            _logger.LogInformation("Вызываем удаленную процедуру {1} по адресу {0}..", serverAddress, "GetHourlyConsumption");
            using var channel = GrpcChannel.ForAddress(serverAddress);
            var client = new ClientDataService.ClientDataServiceClient(channel);

            DateTimeValueResponse response;
            try
            {
                response = await client.GetHourlyConsumptionAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при попытке обращения к методу {0}. {1}", "GetHourlyConsumption", ex.Message);
                return new ErrorResult<DateTimeValueResponse>("");
            }

            _logger.LogInformation("Ответ получен.");
            return new SuccessResult<DateTimeValueResponse>(response);
        }

        public async Task<Result<BaseResponse>> SaveHourlyConsumption(SaveHourlyConsumptionRequest request, ServerCallContext context, string serverAddress)
        {
            _logger.LogInformation("Вызываем удаленную процедуру {1} по адресу {0}..", serverAddress, "SaveHourlyConsumption");
            using var channel = GrpcChannel.ForAddress(serverAddress);
            var client = new ClientDataService.ClientDataServiceClient(channel);

            BaseResponse response;
            try
            {
                response = await client.SaveHourlyConsumptionAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при попытке обращения к методу {0}. {1}", "SaveHourlyConsumption", ex.Message);
                return new ErrorResult<BaseResponse>("");
            }

            _logger.LogInformation("Ответ получен.");
            return new SuccessResult<BaseResponse>(response);
        }
    }
}
