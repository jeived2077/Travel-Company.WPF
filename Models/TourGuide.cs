using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Company.WPF.Models;

public class TourGuide
{
    public long Id { get; set; }
    public long PersonId { get; set; }
    public decimal Salary { get; set; }
    public bool IsFired { get; set; }
    public DateTime? FiredDate { get; set; }

    public virtual Person Person { get; set; } = null!;
    public virtual ICollection<TouristGroup> TouristGroups { get; set; } = new List<TouristGroup>();
    public virtual ICollection<Penalty> Penalties { get; set; } = new List<Penalty>();

    // Updated to include Patronymic
    [NotMapped]
    public string FullName => Person != null ? $"{Person.FirstName} {Person.LastName} {Person.Patronymic}" : "Unknown Tour Guide";
}