using gRPC.Common.Protos;
using Grpc.Core;

namespace Server.Core.gRPC.Client
{
    public interface ICentralService
    {
        /// <summary>
        /// Используется в Server.Client
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <param name="serverAddress"></param>
        /// <returns></returns>
        Task<Result<BaseResponse>> SendDataToServer(CentralServerRequest request, ServerCallContext context, string serverAddress);

        /// <summary>
        /// Используется в Server.Bit
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <param name="serverAddress"></param>
        /// <returns></returns>
        Task<Result<BaseResponse>> SendDataToClient(ClientServerRequest request, ServerCallContext context, string serverAddress);

        /// <summary>
        /// Используется в Server.Bit
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <param name="serverAddress"></param>
        /// <returns></returns>
        Task<Result<BaseResponse>> SendDataToAi(PredictConsumptionRequest request, ServerCallContext context, string serverAddress);
    }
}
