using System;

namespace Travel_Company.WPF.Models;

public partial class Penalty
{
    public long Id { get; set; }

    public long ClientId { get; set; }

    public int TourGuideId { get; set; }

    public decimal CompensationAmount { get; set; }

    public string CompensationDescription { get; set; } = null!;

    public DateTime CompensationDate { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual TourGuide TourGuide { get; set; } = null!;
}
