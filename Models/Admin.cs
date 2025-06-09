using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Company.WPF.Models
{
    public partial class Admin
    {
        public long Id { get; set; }
        public long PersonId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;

        [ForeignKey("PersonId")]
        public virtual Person Person { get; set; } = null!;
    }
}