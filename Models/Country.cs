﻿using System.Collections.Generic;

namespace Travel_Company.WPF.Models;

public partial class Country : ICatalogItem // Added implementation of ICatalogItem
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;

    public virtual ICollection<PopulatedPlace> PopulatedPlaces { get; set; } = new List<PopulatedPlace>();
    public virtual ICollection<Route> Routes { get; set; } = new List<Route>();
}
