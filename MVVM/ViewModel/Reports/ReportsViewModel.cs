using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.MVVM.View.Chart;
using Travel_Company.WPF.Services.Navigation;
using Travel_Company.WPF.Services.Reports;

namespace Travel_Company.WPF.MVVM.ViewModel.Reports
{
    public class ReportsViewModel : Core.ViewModel
    {
        private readonly ReportService _reportService;
        private INavigationService _navigation;

        private DateTime _startDate = DateTime.UtcNow;
        private DateTime _endDate = DateTime.UtcNow;

        private ObservableCollection<IncomeReport> _incomeReports;
        private ObservableCollection<PopularCountry> _popularPlacesReports;

        public RelayCommand GenerateReportCommand { get; set; } = null!;
        public RelayCommand ExportToPdfCommand { get; set; } = null!;
        public RelayCommand ShowChartCommand { get; set; } = null!;


        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
            } 
        }
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
            }
        }
        public INavigationService Navigation
        {
            get => _navigation;
            set
            {
                _navigation = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<IncomeReport> IncomeReports
        {
            get => _incomeReports;
            private set
            {
                _incomeReports = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<IncomeReport> Reports
        {
            get => _incomeReports;
            private set
            {
                _incomeReports = value;
                OnPropertyChanged();
            }
        }

       
        public ObservableCollection<PopularCountry> PopularPlacesReports
        {
            get => _popularPlacesReports;
            private set
            {
                _popularPlacesReports = value;
                OnPropertyChanged();
            }
        }

        public ReportsViewModel(ReportService reportService, INavigationService navigation)
        {
            ReportData = new ObservableCollection<ReportItem>();
            IncomeReports = new ObservableCollection<IncomeReport>();
            PopularPlacesReports = new ObservableCollection<PopularCountry>();
            _reportService = reportService;
            _navigation = navigation;
            GenerateReportCommand = new RelayCommand(execute: _ => GenerateReport(),
            canExecute: _ => true);
            ExportToPdfCommand = new RelayCommand(execute: _ => ExportToPdf(),
            canExecute: _ => true);
            ShowChartCommand = new RelayCommand(execute: _ => ShowChart(),
            canExecute: _ => true);
            ReportTypes = new ObservableCollection<ReportType>
            {
                new ReportType { Title = "Доходы за период", ReportTypeKey = "IncomeReport" },
                new ReportType { Title = "Популярные направления", ReportTypeKey = "PopularCountries" }
            };
            SelectedReportType = ReportTypes.FirstOrDefault();

        }


        private ReportType _selectedReportType;
        public ReportType SelectedReportType
        {
            get => _selectedReportType;
            set
            {
                _selectedReportType = value;
                OnPropertyChanged(); // Уведомление о изменении
                GenerateReportCommand?.Execute(null); // Автогенерация отчета
            }
        }

        public ObservableCollection<ReportType> ReportTypes { get; set; }

        private ObservableCollection<ReportItem> _reportData;
        public ObservableCollection<ReportItem> ReportData
        {
            get => _reportData;
            private set
            {
                _reportData = value;
                OnPropertyChanged();
            }
        }

        private void GenerateReport()
        {
            if (SelectedReportType.ReportTypeKey == "IncomeReport")
            {
                IncomeReports.Clear();
                var data = _reportService.GetIncomeReport(_startDate, _endDate);
                foreach (var item in data)
                {
                    IncomeReports.Add(item);
                }
            }

            if (SelectedReportType.ReportTypeKey == "PopularCountries")
            {
                PopularPlacesReports.Clear();

                var data = _reportService.GetPopularCountries();
                foreach (var item in data)
                {
                    PopularPlacesReports.Add(item);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void ExportToPdf()
        {
            //if (SelectedReportType == null) return;

            //string filePath = "Reports/Report_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";
            //switch (SelectedReportType.ReportTypeKey)
            //{
            //    case "IncomeReport":
            //        _reportService.ExportIncomeToPdf(ReportData, filePath);
            //        MessageBox.Show($"Отчет сохранен в {filePath}");
            //        break;
            //    case "PopularCountries":
            //        _reportService.ExportPopularCountriesToPdf(ReportData, filePath);
            //        MessageBox.Show($"Отчет сохранен в {filePath}");
            //        break;
            //}
        }

        private void ShowChart()
        {
            if (SelectedReportType.ReportTypeKey == "IncomeReport" && (IncomeReports == null || IncomeReports.Count == 0)
                || SelectedReportType.ReportTypeKey == "PopularCountries" && (PopularPlacesReports == null || PopularPlacesReports.Count == 0))
            {
                MessageBox.Show("Нет данных для отображения графика");
                return;
            }

            if (SelectedReportType.ReportTypeKey == "IncomeReport")
            {
                var chartWindow = new IncomeReportsChart(IncomeReports);
                chartWindow.Show();
            }

            if (SelectedReportType.ReportTypeKey == "PopularCountries")
            {
                var chartWindow = new RoutesChartWindow(PopularPlacesReports);
                chartWindow.Show();
            }
        }

    }

    public class ReportType: ObservableObject
    {
        public string Title { get; set; }        // Название отчета (например, "Доходы за период")
        public string ReportTypeKey { get; set; } // Ключ для идентификации отчета (например, "IncomeReport")
    }

    public class ReportItem
    {
        public string Parameter { get; set; }
        public string Value { get; set; }
    }
}
