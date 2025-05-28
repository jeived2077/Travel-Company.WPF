using System;
using System.Collections.Generic;

namespace Travel_Company.WPF.Models;

public partial class TourGuide
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Patronymic { get; set; }

    public long StreetId { get; set; }

    public DateTime Birthdate { get; set; }

    public decimal? Salary { get; set; }

    public bool IsFired { get; set; }

    public DateTime? FiredDate { get; set; }

    public virtual ICollection<Penalty> Penalties { get; set; } = new List<Penalty>();

    public virtual Street Street { get; set; } = null!;

    public virtual ICollection<TouristGroup> TouristGroups { get; set; } = new List<TouristGroup>();
}
