using System.Collections.Generic;

namespace Travel_Company.WPF.Models;

public partial class TourOperator
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string ContactPhone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Address { get; set; } = null!;

    public virtual ICollection<Route> Routes { get; set; } = new List<Route>();
    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
}