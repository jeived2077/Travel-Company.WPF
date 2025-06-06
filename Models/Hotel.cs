using System.Collections.Generic;
using Travel_Company.WPF.Models;

public partial class Hotel : ICatalogItem
{
    public long Id { get; set; }
    public string Name { get; set; } = null!; // Implements ICatalogItem
    public string Class { get; set; } = null!;
    public long? PopulatedPlaceId { get; set; }
    public long? TourOperatorId { get; set; }

    public virtual PopulatedPlace? PopulatedPlace { get; set; }
    public virtual TourOperator? TourOperator { get; set; }
    public virtual ICollection<RoutesPopulatedPlace> RoutesPopulatedPlaces { get; set; } = new List<RoutesPopulatedPlace>();
}