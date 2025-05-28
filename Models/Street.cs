using System.Collections.Generic;

namespace Travel_Company.WPF.Models;

public partial class Street : ICatalogItem
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public virtual ICollection<TourGuide> TourGuides { get; set; } = new List<TourGuide>();
}
