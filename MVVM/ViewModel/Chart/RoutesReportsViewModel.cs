using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel_Company.WPF.Models;

namespace Travel_Company.WPF.MVVM.ViewModel.Chart
{
    internal class RoutesReportsViewModel : Core.ViewModel
    {
        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; }

        public RoutesReportsViewModel(ObservableCollection<PopularCountry> reportData)
        {
            // Инициализация графика
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Количество туров",
                    Values = new ChartValues<int>(reportData.Select(r => r.TourCount))
                }
            };

            // Подписи по оси X (названия стран)
            Labels = reportData.Select(r => r.CountryName).ToList();
        }
    }
}

