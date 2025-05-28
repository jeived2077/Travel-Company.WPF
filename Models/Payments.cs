using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel_Company.WPF.Models
{
    public partial class Payments
    {
        public long Id { get; set; }

        public long RouteId { get; set; }

        public long ClientId { get; set; }


        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PaymentMethod { get; set; } = null!; // Например: "Кредитная карта", "Наличные"

        public string Status { get; set; } = null!; // Например: "paid", "unpaid", "pending"

        public string Comment { get; set; } = null!;

        public Client Client { get; set; } = null!;

        public virtual Route Route { get; set; } = null!;
    }
}
