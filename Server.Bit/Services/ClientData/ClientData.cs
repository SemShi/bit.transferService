using gRPC.Common.Protos;
using Grpc.Core;
using Server.Core.Services;

namespace Server.Bit.Services.ClientData
{
    public class ClientData : ClientDataService.ClientDataServiceBase, IClientData
    {
        private readonly Server.Core.gRPC.Client.ICentralService _centralService;
        private readonly ISqliteService _databaseService;

        public ClientData(
            Core.gRPC.Client.ICentralService centralService, 
            ISqliteService databaseService)
        {
            _centralService = centralService;
            _databaseService = databaseService;
        }

        public override async Task<DateTimeValueResponse> GetPredictionHourlyConsumption(HourlyConsumptionRequest request, ServerCallContext context)
        {
            var dbResponse = await _databaseService.GetClientData(request);
            if (!dbResponse.Success)
                return new DateTimeValueResponse();

            var result = new DateTimeValueResponse()
            {
                Result = new DateTimeValueResponse.Types.DateTimeValueData()
                {
                    DateTimeValue = { dbResponse.Data }
                }
            };

            return result;
        }
    }
}
