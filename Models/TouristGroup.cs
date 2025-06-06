using System;
using System.Collections.Generic;
using Travel_Company.WPF.Models;

namespace Travel_Company.WPF.Models;

public partial class TouristGroup
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public long TourGuideId { get; set; }
    public long RouteId { get; set; }
    public DateTime StartDatetime { get; set; }
    public DateTime EndDatetime { get; set; } // Ensure this property exists
    
    public virtual TourGuide TourGuide { get; set; } = null!;
    public virtual Route Route { get; set; } = null!;
    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
}