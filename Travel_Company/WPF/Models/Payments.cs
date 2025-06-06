using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Company.WPF.Models
{
    public partial class Payments
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey("Route")]
        public long? RouteId { get; set; }
        [ForeignKey("Client")]
        public long ClientId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        [Required]
        public string PaymentMethod { get; set; } = null!;
        [Required]
        public string Status { get; set; } = null!;
        public string Comment { get; set; } = null!;

        public virtual Route Route { get; set; } = null!;
        public virtual Client Client { get; set; } = null!;
    }
}