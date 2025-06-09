using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Company.WPF.Models
{
    public partial class Employee
    {
        public long Id { get; set; }
        public long PersonId { get; set; }
        public byte[]? Photograph { get; set; }
        public bool IsDismissed { get; set; }
        public DateTime? DismissalDate { get; set; }

        [ForeignKey("PersonId")]
        public virtual Person Person { get; set; } = null!;

        public virtual ICollection<EmployeeTouristGroup> EmployeeTouristGroups { get; set; } = new List<EmployeeTouristGroup>();
        public virtual ICollection<TourGuide> TourGuides { get; set; } = new List<TourGuide>();
    }
}