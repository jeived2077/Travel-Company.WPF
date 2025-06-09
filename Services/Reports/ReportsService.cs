using Microsoft.EntityFrameworkCore;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Travel_Company.WPF.Data;
using Travel_Company.WPF.Models;

namespace Travel_Company.WPF.Services.Reports
{
    public class ReportService
    {
        private readonly TravelCompanyDbContext _context;

        public ReportService(TravelCompanyDbContext context)
        {
            _context = context;
        }

        // Отчет: Доходы за период
        public ObservableCollection<IncomeReport> GetIncomeReport(DateTime startDate, DateTime endDate)
        {
            var reports = new ObservableCollection<IncomeReport>();

            var incomeData = _context.TouristGroups
                .Include(tg => tg.Route)
                    .ThenInclude(r => r.Country)
                .Include(tg => tg.TourGuide)
                    .ThenInclude(tg => tg.Person)
                .Where(tg => tg.StartDatetime >= startDate && tg.StartDatetime <= endDate)
                .AsEnumerable() // Переключаемся на клиентскую обработку
                .GroupBy(tg => new { Month = tg.StartDatetime.ToString("yyyy-MM") })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    TotalIncome = g.Sum(tg => tg.Route.Cost), // Убрано ??, так как Cost не nullable
                    AverageCheck = g.Average(tg => tg.Route.Cost), // Убрано ??, так как Cost не nullable
                    BookingCount = g.Count(),
                    BestRouteId = g.GroupBy(tg => tg.RouteId)
                                   .OrderByDescending(rg => rg.Count())
                                   .Select(rg => rg.Key)
                                   .FirstOrDefault(),
                    BestTourGuideId = g.GroupBy(tg => tg.TourGuideId)
                                       .OrderByDescending(tgg => tgg.Count())
                                       .Select(tgg => tgg.Key)
                                       .FirstOrDefault()
                })
                .ToList();

            foreach (var data in incomeData)
            {
                var bestRoute = _context.Routes
                    .Include(r => r.Country)
                    .FirstOrDefault(r => r.Id == data.BestRouteId);
                var bestTourGuide = _context.TourGuides
                    .Include(tg => tg.Person)
                    .FirstOrDefault(tg => tg.Id == data.BestTourGuideId);

                reports.Add(new IncomeReport
                {
                    Month = data.Month,
                    TotalIncome = data.TotalIncome,
                    AverageCheck = data.AverageCheck,
                    BookingCount = data.BookingCount,
                    BestRoute = bestRoute?.Name ?? "-",
                    BestTourGuide = bestTourGuide?.Person.FullName ?? "-",
                    Country = bestRoute?.Country?.Name ?? "-"
                });
            }

            return reports;
        }

        // Отчет: Популярные направления
        public List<PopularCountry> GetPopularCountries()
        {
            return _context.TouristGroups
                .Include(tg => tg.Route)
                .GroupBy(tg => tg.Route)
                .Select(g => new PopularCountry
                {
                    CountryName = g.Key.Name,
                    TourCount = g.Count()
                })
                .Where(pc => pc.TourCount > 0)
                .OrderByDescending(pc => pc.TourCount)
                .ToList();
        }

        // Экспорт доходов в PDF
        public void ExportIncomeToPdf(ObservableCollection<IncomeReport> incomeReports, string filePath)
        {
            using var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Times New Roman", 12);

            gfx.DrawString("Отчет: Доходы за период", font, XBrushes.Black, new XRect(50, 50, page.Width, page.Height), XStringFormats.TopLeft);

            int y = 80;
            foreach (var report in incomeReports)
            {
                gfx.DrawString($"Месяц: {report.Month}", font, XBrushes.Black, new XPoint(50, y)); y += 20;
                gfx.DrawString($"Общий доход: {report.TotalIncome} руб.", font, XBrushes.Black, new XPoint(50, y)); y += 20;
                gfx.DrawString($"Средний чек: {report.AverageCheck} руб.", font, XBrushes.Black, new XPoint(50, y)); y += 20;
                gfx.DrawString($"Количество бронирований: {report.BookingCount}", font, XBrushes.Black, new XPoint(50, y)); y += 20;
                gfx.DrawString($"Лучший маршрут: {report.BestRoute}", font, XBrushes.Black, new XPoint(50, y)); y += 20;
                gfx.DrawString($"Лучший гид: {report.BestTourGuide}", font, XBrushes.Black, new XPoint(50, y)); y += 20;
                gfx.DrawString($"Страна: {report.Country}", font, XBrushes.Black, new XPoint(50, y)); y += 40;
            }

            document.Save(filePath);
        }

        // Экспорт популярных стран в PDF
        public void ExportPopularCountriesToPdf(List<PopularCountry> popularCountries, string filePath)
        {
            using var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Times New Roman", 12);

            gfx.DrawString("Отчет: Популярные направления", font, XBrushes.Black, new XRect(50, 50, page.Width, page.Height), XStringFormats.TopLeft);

            int y = 80;
            foreach (var country in popularCountries)
            {
                gfx.DrawString($"Маршрут: {country.CountryName}", font, XBrushes.Black, new XPoint(50, y)); y += 20;
                gfx.DrawString($"Количество туров: {country.TourCount}", font, XBrushes.Black, new XPoint(50, y)); y += 40;
            }

            document.Save(filePath);
        }
    }
}