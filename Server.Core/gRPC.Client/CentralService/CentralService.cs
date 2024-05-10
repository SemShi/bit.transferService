using gRPC.Common.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;

namespace Server.Core.gRPC.Client
{
    public class CentralService : ICentralService
    {
        private readonly ILogger<CentralService> _logger;

        public CentralService(ILogger<CentralService> logger)
        {
            _logger = logger;
        }

        public async Task<Result<BaseResponse>> SendDataToServer(CentralServerRequest request, ServerCallContext context, string serverAddress)
        {
            _logger.LogInformation("Вызываем удаленную процедуру {1} по адресу {0}..", serverAddress, "PredictConsumptionOnServer");
            using var channel = GrpcChannel.ForAddress(serverAddress);
            var client = new CentralServiceGrpc.CentralServiceGrpcClient(channel);

            BaseResponse response;
            try
            {
                response = await client.PredictConsumptionOnServerAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при попытке обращения к методу {0}. {1}", "PredictConsumptionOnServer", ex.Message);
                return new ErrorResult<BaseResponse>("");
            }

            _logger.LogInformation("Ответ получен.");
            return new SuccessResult<BaseResponse>(response);
        }

        public async Task<Result<BaseResponse>> SendDataToClient(ClientServerRequest request, ServerCallContext context, string serverAddress)
        {
            _logger.LogInformation("Вызываем удаленную процедуру {1} по адресу {0}..", serverAddress, "SaveConsumptionResult");
            using var channel = GrpcChannel.ForAddress(serverAddress);
            var client = new ChildServiceGrpc.ChildServiceGrpcClient(channel);

            BaseResponse response;
            try
            {
                response = await client.SaveConsumptionResultAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при попытке обращения к методу {0}. {1}", "SaveConsumptionResult", ex.Message);
                return new ErrorResult<BaseResponse>("");
            }

            _logger.LogInformation("Ответ получен.");
            return new SuccessResult<BaseResponse>(response);
        }

        public async Task<Result<BaseResponse>> SendDataToAi(PredictConsumptionRequest request, ServerCallContext context, string serverAddress)
        {
            _logger.LogInformation("Вызываем удаленную процедуру {1} по адресу {0}..", serverAddress, "PredictConsumption");
            using var channel = GrpcChannel.ForAddress(serverAddress);
            var client = new ConsumptionService.ConsumptionServiceClient(channel);

            BaseResponse response;
            try
            {
                response = await client.PredictConsumptionAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при попытке обращения к методу {0}. {1}", "PredictConsumption", ex.Message);
                return new ErrorResult<BaseResponse>("");
            }

            _logger.LogInformation("Ответ получен.");
            return new SuccessResult<BaseResponse>(response);
        }
    }
}
