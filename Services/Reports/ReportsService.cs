using Npgsql;
using NpgsqlTypes;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Travel_Company.WPF.Models;


namespace Travel_Company.WPF.Services.Reports
{
    public class ReportService
    {
        private readonly TravelCompanyDbContext _context;
        private readonly string _connectionString = "Host=localhost;Username=postgres;Password=1234;Database=postgres";
        public ReportService(TravelCompanyDbContext context)
        {
            _context = context;
        }

        // Отчет: Доходы за период
        public ObservableCollection<IncomeReport> GetIncomeReport(DateTime startDate, DateTime endDate)
        {
            var reports = new ObservableCollection<IncomeReport>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(
        "SELECT t.Month, " +
        "t.TotalIncome, " +
        "t.AverageCheck, " +
        "t.BookingCount, " +
        "(SELECT r.\"name\" FROM public.\"Route\" r WHERE r.\"id\" = t.route_id LIMIT 1) AS BestRoute, " +
        "(SELECT c.\"name\" FROM public.\"Country\" c WHERE c.\"id\" = (SELECT r2.country_id FROM public.\"Route\" r2 WHERE r2.\"id\" = t.route_id LIMIT 1) LIMIT 1) AS Country " +
        "FROM ( " +
        "    SELECT TO_CHAR(p.payment_date, 'YYYY-MM') AS Month, " +
        "           MAX(p.route_id) AS route_id, " + 
        "           SUM(CAST(p.\"Amount\" AS DECIMAL)) AS TotalIncome, " +
        "           AVG(CAST(p.\"Amount\" AS DECIMAL)) AS AverageCheck, " +
        "           COUNT(p.payment_id) AS BookingCount " +
        "    FROM public.\"Payments\" p " +
        "    WHERE p.payment_date BETWEEN @start_date AND @end_date " +
        "    GROUP BY TO_CHAR(p.payment_date, 'YYYY-MM') " +
        ") t " +
        "ORDER BY t.Month", conn);

            cmd.Parameters.AddWithValue("@start_date", NpgsqlDbType.TimestampTz, startDate);
            cmd.Parameters.AddWithValue("@end_date", NpgsqlDbType.TimestampTz, endDate);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                reports.Add(new IncomeReport
                {
                    Month = reader.GetString(0),
                    TotalIncome = reader.GetDecimal(1),
                    AverageCheck = reader.GetDecimal(2),
                    BookingCount = reader.GetInt32(3),
                    BestRoute = reader.IsDBNull(4) ? "-" : reader.GetString(4),
                    Country = reader.IsDBNull(5) ? "-" : reader.GetString(5)
                });
            }

            return reports;
        }

        // Отчет: Популярные направления
        public List<PopularCountry> GetPopularCountries()
        {
            var countries = new List<PopularCountry>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(
                "SELECT r.\"name\", COUNT(r.\"id\") AS TourCount " +
                "FROM public.\"Route\" r " +
                "JOIN public.\"Payments\" p ON r.\"id\" = p.route_id " +
                "GROUP BY r.\"name\" " +
                "HAVING COUNT(r.\"id\") > 0 " +
                "ORDER BY TourCount DESC", conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                countries.Add(new PopularCountry
                {
                    CountryName = reader.GetString(0),
                    TourCount = reader.GetInt32(1)
                });
            }

            return countries;
        }

        // Отчет: Активность менеджеров
        public List<ManagerActivity> GetManagerActivity()
        {
            var activities = new List<ManagerActivity>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(
                "SELECT m.FullName, COUNT(b.Id) AS BookingCount " +
                "FROM Bookings b " +
                "JOIN Managers m ON b.ManagerId = m.Id " +
                "GROUP BY m.FullName " +
                "ORDER BY BookingCount DESC", conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                activities.Add(new ManagerActivity
                {
                    ManagerName = reader.GetString(0),
                    BookingCount = reader.GetInt32(1)
                });
            }

            return activities;
        }

        // Отчет: Визовые требования
        public List<VisaRequirement> GetVisaRequirements(int clientId)
        {
            var requirements = new List<VisaRequirement>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(
                "SELECT c.Name, p.VisaType, p.DurationOfStay, p.Cost " +
                "FROM Policies p " +
                "JOIN Clients cl ON p.CitizenshipCountryId = cl.CountryId " +
                "WHERE cl.Id = :client_id", conn);
            cmd.Parameters.AddWithValue(":client_id", clientId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                requirements.Add(new VisaRequirement
                {
                    CountryName = reader.GetString(0),
                    VisaType = reader.GetString(1),
                    DurationOfStay = reader.GetInt32(2),
                    Cost = reader.GetDecimal(3)
                });
            }

            return requirements;
        }

        public void ExportIncomeToPdf(Dictionary<string, string> reportData, string filePath)
        {
            using var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Times New Roman", 12);

            // Заголовок
            gfx.DrawString("Отчет: Доходы за период", font, XBrushes.Black, new XRect(50, 50, page.Width, page.Height), XStringFormats.TopLeft);

            // Данные
            int y = 80;
            foreach (var item in reportData)
            {
                gfx.DrawString($"{item.Key}: {item.Value} руб.", font, XBrushes.Black, new XPoint(50, y));
                y += 20;
            }

            document.Save(filePath);
        }

        // Экспорт популярных стран в PDF
        public void ExportPopularCountriesToPdf(Dictionary<string, string> reportData, string filePath)
        {
            using var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Times New Roman", 12);

            // Заголовок
            gfx.DrawString("Отчет: Популярные направления", font, XBrushes.Black, new XRect(50, 50, page.Width, page.Height), XStringFormats.TopLeft);

            // Данные
            int y = 80;
            foreach (var item in reportData)
            {
                gfx.DrawString($"{item.Key}: {item.Value} туров", font, XBrushes.Black, new XPoint(50, y));
                y += 20;
            }

            document.Save(filePath);
        }
    }
}
