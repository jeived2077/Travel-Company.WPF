using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel_Company.WPF.Models;
using System.Collections.ObjectModel;

namespace Travel_Company.WPF.MVVM.ViewModel.Chart
{

    public class IncomeReportViewModel: Core.ViewModel
    {
        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; }

        public IncomeReportViewModel(ObservableCollection<IncomeReport> reportData)
        {
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Доход",
                    Values = new ChartValues<decimal>(reportData.Select(r => r.TotalIncome))
                },
                new LineSeries
                {
                    Title = "Средний чек",
                    Values = new ChartValues<decimal>(reportData.Select(r => r.AverageCheck))
                }
            };

            Labels = reportData.Select(r => r.Month).ToList();
        }
    }
}
