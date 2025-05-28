using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel_Company.WPF.Models
{
    public class TourOperator
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty; // Название компании-оператора

        public string ContactInfo { get; set; } = string.Empty; // Контактные данные (телефон, email)

        public string Address { get; set; } = string.Empty; // Адрес офиса

        // Связь с маршрутами
        public virtual ICollection<Route> Routes { get; set; } = new HashSet<Route>();

    }
}
