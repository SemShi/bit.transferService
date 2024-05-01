using Google.Protobuf.Collections;
using gRPC.Common.Protos;
using DateTime = System.DateTime;

namespace Server.Core.Services
{
    public class PredictionRow
    {
        public string Id { get; set; } = null!;
        public DateTime DateTime { get; set; }
        public double Value { get; set; }

        public PredictionRow() { }
        public PredictionRow(DateTimeValue dt, string requestId)
        {
            Id = requestId;
            Value = dt.Value;
            DateTime = new DateTime(dt.DateTime.Date.Year, dt.DateTime.Date.Month, dt.DateTime.Date.Day, dt.DateTime.Time.Hour, dt.DateTime.Time.Minute, dt.DateTime.Time.Second);
        }

        public PredictionRow(DateTime dt, double value, string requestId)
        {
            
        }
    }
}
