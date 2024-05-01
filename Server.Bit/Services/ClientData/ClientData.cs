using Google.Protobuf.Collections;
using gRPC.Common.Protos;
using Grpc.Core;
using Server.Core;
using Server.Core.Services;
using DateTime = gRPC.Common.Protos.DateTime;

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

        public async Task<DateTimeValueResponse> GetPredictionHourlyConsumption(HourlyConsumptionRequest request, ServerCallContext context)
        {
            var dbResponse = await _databaseService.GetClientData(request);
            if (!dbResponse.Success)
                return new DateTimeValueResponse();

            var resultDb = new RepeatedField<DateTimeValue>();
            foreach (var row in dbResponse.Data)
            {
                resultDb.Add(new DateTimeValue()
                {
                    DateTime = new DateTime()
                    {
                        Date = new Date()
                        {
                            Day = row.DateTime.Day,
                            Month = row.DateTime.Month,
                            Year = row.DateTime.Year,
                        },
                        Time = new Time()
                        {
                            Hour = row.DateTime.Hour,
                            Minute = row.DateTime.Minute,
                            Second = row.DateTime.Second,
                        }
                    },
                    Value = row.Value
                });
            }

            var result = new DateTimeValueResponse()
            {
                Result = new DateTimeValueResponse.Types.DateTimeValueData()
                {
                    DateTimeValue = { resultDb }
                }
            };

            return await Task.FromResult(result);
        }
    }
}
