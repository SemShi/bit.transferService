using Google.Protobuf.Collections;
using gRPC.Common.Protos;

namespace Server.Core.Services
{
    public interface ISqliteService
    {
        Task<Result> AddCoefficients(RequestСoefficientMinMax model);
        Task<Result<RequestСoefficientMinMax>> GetCoefficientsByRequestId(string requestId);

        Task<Result> SaveClientData(CentralServerRequest request);
        Task<Result<RepeatedField<DateTimeValue>>> GetClientData(HourlyConsumptionRequest request);
        Task<Result> SavePrediction(SaveHourlyConsumptionRequest request);
    }
}
