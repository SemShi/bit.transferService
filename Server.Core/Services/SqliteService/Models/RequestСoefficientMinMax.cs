using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Core.Services
{
    [Table("Сoefficients")]
    public class RequestСoefficientMinMax
    {
        [Key]
        public string RequestId { get; set; } = Guid.Empty.ToString();
        public double Min { get; set; } = 0;
        public double Max { get; set; } = 0;

        public bool ValidateModel()
        {
            if (RequestId == Guid.Empty.ToString()) return false;
            if (Min == 0) return false;
            if (Max == 0) return false;
            return true;
        }
    }
}
