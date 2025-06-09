using System;
using System.Collections.Generic;
using Travel_Company.WPF.Models;

public class TouristGroup
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public long TourGuideId { get; set; }
    public long RouteId { get; set; } // Fixed: Corrected from RoutId
    public DateTime StartDatetime { get; set; }
    public DateTime EndDatetime { get; set; }

    public virtual TourGuide TourGuide { get; set; } = null!;
    public virtual Route Route { get; set; } = null!;
    public virtual List<Client> Clients { get; set; } = new List<Client>();
    public virtual List<EmployeeTouristGroup> EmployeeTouristGroups { get; set; } = new List<EmployeeTouristGroup>();
}