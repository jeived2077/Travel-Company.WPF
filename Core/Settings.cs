using Travel_Company.WPF.Core.Enums;
using Travel_Company.WPF.Models;

namespace Travel_Company.WPF.Core; // Adjust namespace as needed

public class AppSettings
{
    public User? User { get; set; }
    public string UserName { get; set; } = string.Empty;
    public bool IsAuthorized { get; set; }
    public UserRole UserRole { get; set; } = UserRole.None;
}