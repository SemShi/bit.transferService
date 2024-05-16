using gRPC.Common.Protos;

namespace Server.Core.Services
{
    public class ClientData
    {
        public string MeteringUnitGuid { get; set; } = null!;
        public System.DateTime DateTime { get; set; }
        public double Value { get; set; }

        public ClientData() { }
        public ClientData(DateTimeValue dt, string requestMeteringUnitGuid)
        {
            MeteringUnitGuid = requestMeteringUnitGuid;
            Value = dt.Value;
            DateTime = new System.DateTime(dt.DateTime.Date.Year, dt.DateTime.Date.Month, dt.DateTime.Date.Day, dt.DateTime.Time.Hour, dt.DateTime.Time.Minute, dt.DateTime.Time.Second);
        }
    }
}
