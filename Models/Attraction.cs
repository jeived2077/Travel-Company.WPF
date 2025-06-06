using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel_Company.WPF.Models
{
    public partial class Attraction
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<UsersAttraction> UsersAttractions { get; set; } = new List<UsersAttraction>();
    }
}
