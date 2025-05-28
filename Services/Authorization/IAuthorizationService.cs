using Travel_Company.WPF.Models;

namespace Travel_Company.WPF.Services.Authorization;

public interface IAuthorizationService
{
    User? LogIn(string username, string password);
}