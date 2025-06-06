
using Microsoft.EntityFrameworkCore;
using System.Linq;
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

    public User? LogIn(string username, string password)
    {
        var user = _context.Users
            .Include(u => u.UsersAttractions) // Changed from UsersObjects to UsersAttractions
            .ThenInclude(o => o.Attraction) // Changed from Object to Attraction
            .FirstOrDefault(u => u.Username == username.ToUpper());

        return (user is not null && user.Password == password)
            ? user
            : null;
    }
}