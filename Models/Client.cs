using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Company.WPF.Models;

public class Client
{
    public long Id { get; set; } 
    public long PersonId { get; set; }
    public byte[]? Photograph { get; set; }

    public Person Person { get; set; } = null!;
    public ICollection<TouristGroup> TouristGroups { get; set; } = null!;
    public ICollection<Penalty> Penalties { get; set; } = null!;
    public ICollection<Payment> Payments { get; set; } = null!;

    [NotMapped]
    public string Name => Person?.FullName ?? string.Empty; // For backward compatibility, if needed
}