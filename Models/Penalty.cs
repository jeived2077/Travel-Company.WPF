using System;
using Travel_Company.WPF.Models;

public partial class Penalty
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public long TourGuideId { get; set; }
    public DateTime CompensationDate { get; set; }
    public decimal Amount { get; set; } // Renamed from CompensationAmount to match other contexts
    public string Reason { get; set; } = null!; // Renamed from CompensationDescription to match other contexts

    public virtual Client Client { get; set; } = null!;
    public virtual TourGuide TourGuide { get; set; } = null!;
}