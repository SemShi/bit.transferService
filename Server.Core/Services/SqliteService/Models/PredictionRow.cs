namespace Server.Core.Services
{
    public class PredictionRow
    {
        public string Id { get; set; } = null!;
        public DateTime DateTime { get; set; }
        public double Value { get; set; }
    }
}
