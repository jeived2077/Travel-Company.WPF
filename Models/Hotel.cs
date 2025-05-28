using System.Collections.Generic;

namespace Travel_Company.WPF.Models;

public partial class Hotel : ICatalogItem
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Class { get; set; } = null!;

    public virtual ICollection<RoutesPopulatedPlace> RoutesPopulatedPlaces { get; set; } = new List<RoutesPopulatedPlace>();
}
