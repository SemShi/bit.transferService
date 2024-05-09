using gRPC.Common.Protos;
using DateTime = System.DateTime;

namespace gRPC.Common
{
    public static class ProtoExtensions
    {
        public static Protos.Date AddDays(this Protos.Date date, int daysAdd)
        {
            var dt = new DateTime(date.Year, date.Month, date.Day);
            try{ dt = dt.AddDays(daysAdd); }
            catch { dt = DateTime.MinValue; }
            return new Protos.Date() { Day = dt.Day, Month = dt.Month, Year = dt.Year };
        }

        public static Protos.DateTime AddHours(this Protos.DateTime date, int hoursAdd)
        {
            var dt = new DateTime(date.Date.Year, date.Date.Month, date.Date.Day, date.Time.Hour, date.Time.Minute, date.Time.Second);
            try { dt = dt.AddHours(hoursAdd); }
            catch { dt = DateTime.MinValue; }

            return new Protos.DateTime()
            {
                Date = new Date()
                {
                    Day = dt.Day,
                    Month = dt.Month,
                    Year = dt.Year,
                },
                Time = new Time()
                {
                    Hour = dt.Hour,
                    Minute = dt.Minute,
                    Second = dt.Second
                }
            };
        }
    }
}
