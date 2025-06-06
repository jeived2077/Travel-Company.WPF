using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Travel_Company.WPF.Models;

public class Person
{
    public long Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Patronymic { get; set; } = null!;
    public DateTime Birthdate { get; set; }
    public long? StreetId { get; set; }
    public long? UserId { get; set; } // New property

    public virtual Street? Street { get; set; }
    public virtual User? User { get; set; }
    // Other navigation properties (e.g., Client, TourGuide) remain unchanged
    [NotMapped]
    public string FullName => $"{FirstName} {LastName} {Patronymic}".Trim();
}