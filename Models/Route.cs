using System;
using System.Collections.Generic;
using Travel_Company.WPF.Models;

public partial class Route
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal? Cost { get; set; }
    public long CountryId { get; set; }
    public long? TourOperatorId { get; set; } // Added to match SeedRoutes and DbContext
    public DateTime StartDatetime { get; set; }
    public DateTime EndDatetime { get; set; }

    public virtual Country Country { get; set; } = null!;
    public virtual TourOperator? TourOperator { get; set; } // Added to match DbContext
    public virtual ICollection<RoutesPopulatedPlace> RoutesPopulatedPlaces { get; set; } = new List<RoutesPopulatedPlace>();
    public virtual ICollection<TouristGroup> TouristGroups { get; set; } = new List<TouristGroup>();
    public virtual ICollection<Payments> Payments { get; set; } = new List<Payments>(); // Added to match DbContext
}