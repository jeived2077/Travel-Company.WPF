using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Company.WPF.Models;

public class Penalty
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public long TourGuideId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CompensationDate { get; set; }
    public string Reason { get; set; } = null!;

    public Client Client { get; set; } = null!;
    public TourGuide TourGuide { get; set; } = null!;
}
