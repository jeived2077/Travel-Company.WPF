using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Company.WPF.Models
{
    public partial class EmployeeTouristGroup
    {
        public long EmployeeId { get; set; }
        public long TouristGroupId { get; set; }
        public string Role { get; set; } = "Assistant";

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey("TouristGroupId")]
        public virtual TouristGroup TouristGroup { get; set; } = null!;
    }
}