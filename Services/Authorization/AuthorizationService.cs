using Microsoft.EntityFrameworkCore;
using System.Linq;
using Travel_Company.WPF.Core.Enums;
using Travel_Company.WPF.Data;
using Travel_Company.WPF.Models;

namespace Travel_Company.WPF.Services.Authorization;

public class AuthorizationService : IAuthorizationService
{
    private readonly TravelCompanyDbContext _context;

    public AuthorizationService(TravelCompanyDbContext context)
    {
        _context = context;
    }

    public (User? User, UserRole Role) LogIn(string username, string password)
    {
        var user = _context.Users
            .Include(u => u.Persons)
            .ThenInclude(p => p.Admin)
            .Include(u => u.Persons)
            .ThenInclude(p => p.Employee)
            .Include(u => u.Persons)
            .ThenInclude(p => p.Client)
            .FirstOrDefault(u => u.Username == username.ToUpper());

        if (user == null || user.Password != password)
        {
            return (null, UserRole.None);
        }

        UserRole role = UserRole.None;
        if (user.Persons?.Any(p => p.Admin != null) == true)
        {
            role = UserRole.Admin;
        }
        else if (user.Persons?.Any(p => p.Employee != null && !p.Employee.IsDismissed) == true)
        {
            role = UserRole.Employee;
        }
        else if (user.Persons?.Any(p => p.Client != null) == true)
        {
            role = UserRole.Client;
        }

        return (user, role);
    }
}