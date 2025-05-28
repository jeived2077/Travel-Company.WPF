using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel_Company.WPF.Models
{
    public class IncomeReport
    {
        public string Month { get; set; } // Месяц
        public decimal TotalIncome { get; set; } // Общий доход
        public decimal AverageCheck { get; set; } // Средний чек
        public int BookingCount { get; set; } // Количество бронирований
        public string BestRoute { get; set; } // Лучший маршрут
        public string BestTourGuide { get; set; } // Лучший гид
        public string Country { get; set; } // Страна назначения
    }

    public class PopularCountry
    {
        public string CountryName { get; set; }
        public int TourCount { get; set; }
    }

    public class ManagerActivity
    {
        public string ManagerName { get; set; }
        public int BookingCount { get; set; }
    }

    public class VisaRequirement
    {
        public string CountryName { get; set; }
        public string VisaType { get; set; }
        public int DurationOfStay { get; set; }
        public decimal Cost { get; set; }
    }
}
