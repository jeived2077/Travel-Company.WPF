using System;
using System.Collections.Generic;
using Travel_Company.WPF.Models;

public class Route
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Cost { get; set; }
    public DateTime StartDatetime { get; set; }
    public DateTime EndDatetime { get; set; }
    public long CountryId { get; set; }
    public long TourOperatorId { get; set; }
    public virtual Country Country { get; set; } = null!;
    public virtual TourOperator TourOperator { get; set; } = null!;
    public virtual List<TouristGroup> TouristGroups { get; set; } = new List<TouristGroup>();
    public virtual List<Payment> Payments { get; set; } = new List<Payment>();
    public virtual List<RoutesPopulatedPlace> RoutesPopulatedPlaces { get; set; } = new List<RoutesPopulatedPlace>();
}