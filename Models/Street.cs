using System.Collections.Generic;

namespace Travel_Company.WPF.Models;

public partial class Street : ICatalogItem // Added implementation of ICatalogItem
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;

    public virtual ICollection<Person> Persons { get; set; } = new List<Person>();
}
