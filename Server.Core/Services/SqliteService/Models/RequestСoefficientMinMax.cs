namespace Server.Core.Services
{
    public class RequestСoefficientMinMax
    {
        public string Id { get; set; } = Guid.Empty.ToString();
        public double Min { get; set; } = 0;
        public double Max { get; set; } = 0;

        public bool ValidateModel()
        {
            if (Id == Guid.Empty.ToString()) return false;
            if (Min == 0) return false;
            if (Max == 0) return false;
            return true;
        }
    }
}
