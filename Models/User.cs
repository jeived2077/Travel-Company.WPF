using System.Collections.Generic;

namespace Travel_Company.WPF.Models;

public class User
{
    public long Id { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public ICollection<Person>? Persons { get; set; }
}