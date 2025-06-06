using System.Collections.Generic;

namespace Travel_Company.WPF.Models;

public partial class User
{
    public long Id { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;

    public virtual ICollection<Person> Persons { get; set; } = new List<Person>(); // Added navigation property
    public virtual ICollection<UsersAttraction> UsersAttractions { get; set; } = new List<UsersAttraction>();
}