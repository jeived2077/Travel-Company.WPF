using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.MVVM.ViewModel.Chart;

namespace Travel_Company.WPF.MVVM.View.Chart
{
    public partial class IncomeReportsChart : Window
    {
        public IncomeReportsChart(ObservableCollection<IncomeReport> reportData)
        {
            InitializeComponent();
            DataContext = new IncomeReportViewModel(reportData);
        }
    }
}
