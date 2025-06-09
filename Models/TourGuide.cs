using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Company.WPF.Models;

public class TourGuide
{
    public long Id { get; set; }
    public long PersonId { get; set; }
    public long EmployeeId { get; set; }
    public decimal Salary { get; set; }
    public bool IsFired { get; set; }
    public DateTime? FiredDate { get; set; }

    public Person Person { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
    public System.Collections.Generic.ICollection<TouristGroup>? TouristGroups { get; set; }
    public System.Collections.Generic.ICollection<Penalty>? Penalties { get; set; }

    [NotMapped]
    public string FullName
    {
        get
        {
            if (Person == null)
                return string.Empty;
            return $"{Person.LastName} {Person.FirstName} {Person.Patronymic}".Trim();
        }
    }

    [NotMapped]
    public string IsFiredWithText
    {
        get => IsFired ? "Yes" : "No";
    }
}