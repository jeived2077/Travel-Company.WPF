using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel_Company.WPF.Models
{
    
    public partial class UsersAttraction
    {
        public long UserId { get; set; }
        public long AttractionId { get; set; }
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }

        public virtual Attraction Attraction { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
