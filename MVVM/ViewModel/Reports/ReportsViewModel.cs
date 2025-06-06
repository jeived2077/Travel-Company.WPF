using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
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

        private DateTime _startDate = DateTime.UtcNow.AddMonths(-1); // Default to last month
        private DateTime _endDate = DateTime.UtcNow;

        private ObservableCollection<IncomeReport> _incomeReports = new ObservableCollection<IncomeReport>();
        private ObservableCollection<PopularCountry> _popularPlacesReports = new ObservableCollection<PopularCountry>();

        public RelayCommand GenerateReportCommand { get; set; }
        public RelayCommand ExportToPdfCommand { get; set; }
        public RelayCommand ShowChartCommand { get; set; }

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

        public ObservableCollection<PopularCountry> PopularPlacesReports
        {
            get => _popularPlacesReports;
            private set
            {
                _popularPlacesReports = value;
                OnPropertyChanged();
            }
        }

        private Visibility _isChartButtonVisible = Visibility.Collapsed;
        public Visibility IsChartButtonVisible
        {
            get => _isChartButtonVisible;
            set
            {
                _isChartButtonVisible = value;
                OnPropertyChanged();
            }
        }

        private Visibility _isExportButtonVisible = Visibility.Collapsed;
        public Visibility IsExportButtonVisible
        {
            get => _isExportButtonVisible;
            set
            {
                _isExportButtonVisible = value;
                OnPropertyChanged();
            }
        }

        public ReportsViewModel(ReportService reportService, INavigationService navigation)
        {
            _reportService = reportService;
            _navigation = navigation;
            GenerateReportCommand = new RelayCommand(execute: _ => GenerateReport(), canExecute: _ => true);
            ExportToPdfCommand = new RelayCommand(execute: _ => ExportToPdf(), canExecute: _ => true);
            ShowChartCommand = new RelayCommand(execute: _ => ShowChart(), canExecute: _ => true);
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
                OnPropertyChanged();
                UpdateButtonVisibility();
                GenerateReportCommand?.Execute(null); // Автогенерация отчета
            }
        }

        public ObservableCollection<ReportType> ReportTypes { get; set; }

        private void UpdateButtonVisibility()
        {
            bool hasData = SelectedReportType?.ReportTypeKey == "IncomeReport" && IncomeReports.Any()
                        || SelectedReportType?.ReportTypeKey == "PopularCountries" && PopularPlacesReports.Any();
            IsChartButtonVisible = hasData ? Visibility.Visible : Visibility.Collapsed;
            IsExportButtonVisible = hasData ? Visibility.Visible : Visibility.Collapsed;
        }

        private void GenerateReport()
        {
            if (SelectedReportType.ReportTypeKey == "IncomeReport")
            {
                IncomeReports.Clear();
                var data = _reportService.GetIncomeReport(StartDate, EndDate);
                foreach (var item in data)
                {
                    IncomeReports.Add(item);
                }
                UpdateButtonVisibility();
            }
            else if (SelectedReportType.ReportTypeKey == "PopularCountries")
            {
                PopularPlacesReports.Clear();
                var data = _reportService.GetPopularCountries();
                foreach (var item in data)
                {
                    PopularPlacesReports.Add(new PopularCountry { CountryName = item.CountryName, TourCount = item.TourCount });
                }
                UpdateButtonVisibility();
            }
        }

        private void ExportToPdf()
        {
            if (SelectedReportType == null) return;

            string filePath = $"Reports/Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            try
            {
                switch (SelectedReportType.ReportTypeKey)
                {
                    case "IncomeReport":
                        _reportService.ExportIncomeToPdf(IncomeReports, filePath);
                        MessageBox.Show($"Отчет сохранен в {filePath}");
                        break;
                    case "PopularCountries":
                        _reportService.ExportPopularCountriesToPdf(PopularPlacesReports.ToList(), filePath);
                        MessageBox.Show($"Отчет сохранен в {filePath}");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}");
            }
        }

        private void ShowChart()
        {
            if (SelectedReportType.ReportTypeKey == "IncomeReport" && (!IncomeReports?.Any() ?? true)
                || SelectedReportType.ReportTypeKey == "PopularCountries" && (!PopularPlacesReports?.Any() ?? true))
            {
                MessageBox.Show("Нет данных для отображения графика");
                return;
            }

            if (SelectedReportType.ReportTypeKey == "IncomeReport")
            {
                var chartWindow = new IncomeReportsChart(IncomeReports);
                chartWindow.Show();
            }
            else if (SelectedReportType.ReportTypeKey == "PopularCountries")
            {
                var chartWindow = new RoutesChartWindow(PopularPlacesReports); // Removed .ToList()
                chartWindow.Show();
            }
        }
    }

    public class ReportType : ObservableObject
    {
        public string Title { get; set; }        // Название отчета (например, "Доходы за период")
        public string ReportTypeKey { get; set; } // Ключ для идентификации отчета (например, "IncomeReport")
    }
}