using gRPC.Common.Protos;
using DateTime = gRPC.Common.Protos.DateTime;

namespace Server.Core.Services
{
    public class PredictedRow
    {
        public string RequestId { get; set; } = null!;
        public System.DateTime DateTime { get; set; }
        public double Value { get; set; }

        public PredictedRow() { }
        public PredictedRow(DateTimeValue dt, string requestRequestId)
        {
            RequestId = requestRequestId;
            Value = dt.Value;
            DateTime = new System.DateTime(dt.DateTime.Date.Year, dt.DateTime.Date.Month, dt.DateTime.Date.Day, dt.DateTime.Time.Hour, dt.DateTime.Time.Minute, dt.DateTime.Time.Second);
        }
    }
}
