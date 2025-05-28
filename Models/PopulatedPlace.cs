using System.Collections.Generic;

namespace Travel_Company.WPF.Models;

public partial class PopulatedPlace : ICatalogItem
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public int CountryId { get; set; }

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<RoutesPopulatedPlace> RoutesPopulatedPlaces { get; set; } = new List<RoutesPopulatedPlace>();
}
