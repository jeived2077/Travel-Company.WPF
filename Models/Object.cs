using System.Collections.Generic;

namespace Travel_Company.WPF.Models;

public partial class Object
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<UsersObject> UsersObjects { get; set; } = new List<UsersObject>();
}
