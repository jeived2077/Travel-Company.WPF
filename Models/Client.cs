using Microsoft.EntityFrameworkCore; // Required for [NotMapped]
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Resources.Components;

namespace Travel_Company.WPF.Models;

public partial class Client : ICatalogItem
{
    public long Id { get; set; }
    public long PersonId { get; set; } // Explicit foreign key to Person

    public byte[]? Photograph { get; set; }

    public virtual Person Person { get; set; } = null!;
    public virtual Passport Passport { get; set; } = null!;
    public virtual ICollection<Penalty> Penalties { get; set; } = new List<Penalty>();
    public virtual ICollection<Payments> Payments { get; set; } = new List<Payments>();
    public virtual ICollection<TouristGroup> TouristGroups { get; set; } = new List<TouristGroup>();

    [NotMapped]
    public string FullName => Person != null ? $"{Person.FirstName} {Person.LastName} {Person.Patronymic}" : "Unknown Client";

    // Implement ICatalogItem.Name as a read-only property
    public string Name => FullName; // Delegates to FullName for consistency
}