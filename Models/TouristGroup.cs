using System.Collections.Generic;

namespace Travel_Company.WPF.Models;

public partial class TouristGroup
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public int TourGuideId { get; set; }

    public long RouteId { get; set; }

    public virtual Route Route { get; set; } = null!;

    public virtual TourGuide TourGuide { get; set; } = null!;

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
}
