using System;
using Travel_Company.WPF.Models;

public class Payment
{
    public long Id { get; set; }
    public long ClientId { get; set; } // Внешний ключ
    public long RouteId { get; set; } // Внешний ключ
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Status { get; set; } = null!;
    public string PaymentMethod { get; set; } = null!;
    public string? Comment { get; set; }

    public Client Client { get; set; } = null!;
    public Route Route { get; set; } = null!;
}