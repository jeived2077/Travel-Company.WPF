using Travel_Company.WPF.Models;

namespace Travel_Company.WPF.Core;

public class Settings
{
    public bool IsAuthorized { get; set; } = false;
    public User? User { get; set; }
    public string UserName { get; set; } = string.Empty;
}