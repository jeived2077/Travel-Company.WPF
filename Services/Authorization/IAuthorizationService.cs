using Travel_Company.WPF.Core.Enums;
using Travel_Company.WPF.Models;

namespace Travel_Company.WPF.Services.Authorization;

public interface IAuthorizationService
{
    (User? User, UserRole Role) LogIn(string username, string password);
}