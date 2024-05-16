using Google.Protobuf.Collections;
using gRPC.Common.Protos;

namespace Server.Core.Services
{
    public interface ISqliteService
    {
        Task<Result> AddCoefficients(RequestСoefficientMinMax model);
        Task<Result<RequestСoefficientMinMax>> GetCoefficientsByMeteringPointGuid(string meteringPointGuid);

        Task<Result> SaveClientData(CentralServerRequest request);
        Task<Result<RepeatedField<DateTimeValue>>> GetClientData(HourlyConsumptionRequest request);
        Task<Result> SavePrediction(SaveHourlyConsumptionRequest request);
    }
}
