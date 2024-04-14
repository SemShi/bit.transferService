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
    }
}
