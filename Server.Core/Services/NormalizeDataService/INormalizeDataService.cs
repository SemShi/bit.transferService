using Google.Protobuf.Collections;
using gRPC.Common.Protos;
using Grpc.Core;

namespace Server.Core.Services
{
    public interface INormalizeDataService
    {
        public Task<Result<RequestСoefficientMinMax>> GetMinMaxDictionary(RepeatedField<DateTimeValue> data);
        public Task<Result<RepeatedField<DateTimeValue>>> Normalize(RepeatedField<DateTimeValue> data, RequestСoefficientMinMax parameters);
        public Task<Result<RepeatedField<DateTimeValue>>> Denormalize(RepeatedField<DateTimeValue> data, RequestСoefficientMinMax parameters);
    }
}
