
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Travel_Company.WPF.Models;

namespace Travel_Company.WPF.Services.Authorization;

public class AuthorizationService : IAuthorizationService
{
    private readonly TravelCompanyDbContext _context;

    public AuthorizationService(TravelCompanyDbContext context)
    {
        _context = context;
    }

    public User? LogIn(string username, string password)
    {
        var user = _context.Users
            .Include(u => u.UsersObjects)
            .ThenInclude(o => o.Object)
            .FirstOrDefault(u => u.Username == username.ToUpper());

        return (user is not null && user.Password == password)
            ? user
            : null;
    }
}