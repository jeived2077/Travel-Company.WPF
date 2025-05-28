using System;
using System.Collections.Generic;
using Travel_Company.WPF.Core;

namespace Travel_Company.WPF.Models;

public partial class Client : ObservableObject
{
    public long Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Patronymic { get; set; }

    public DateTime Birthdate { get; set; }

    public long StreetId { get; set; }

    public string PassportSeries { get; set; } = null!;

    public string PassportNumber { get; set; } = null!;

    public DateTime PassportIssueDate { get; set; }

    public string PassportIssuingAuthority { get; set; } = null!;

    private byte[]? _photograph;
    public byte[]? Photograph
    {
        get => _photograph;
        set
        {
            _photograph = value;
            OnPropertyChanged();
        }
    }

    public virtual ICollection<Penalty> Penalties { get; set; } = new List<Penalty>();

    public virtual ICollection<Payments> Payments { get; set; } = new List<Payments>();


    public virtual Street Street { get; set; } = null!;

    public virtual ICollection<TouristGroup> TouristGroups { get; set; } = new List<TouristGroup>();
}
