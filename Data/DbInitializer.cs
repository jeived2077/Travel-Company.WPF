using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Travel_Company.WPF.Models;

namespace Travel_Company.WPF.Data;

public static class DbInitializer
{
    public static void Seed(IServiceProvider serviceProvider)
    {
        using var serviceScope = serviceProvider.CreateScope();
        var db = serviceScope.ServiceProvider.GetService<TravelCompanyDbContext>();
        db!.Database.EnsureCreated();
        db.Database.Migrate();
        SeedAllEntities(db);
    }

    private static void SeedAllEntities(TravelCompanyDbContext db)
    {
        // Catalogs
        var countries = SeedCountries(db);
        var streets = SeedStreets(db);
        var hotels = SeedHotels(db);
        var places = SeedPopulatedPlaces(db, countries);

        // Other entities
        var employees = SeedEmployees(db, streets);
        var routes = SeedRoutes(db, countries);
        var groups = SeedTouristGroups(db, employees, routes);

        var tourists = SeedTourists(db, streets, groups);
        SeedPenalties(db, tourists, employees);
        SeedPayments(db);

        SeedRoutesPopulatedPlaces(db, routes, places, hotels);

        // Users
        var appObjects = SeedObjects(db);
        var users = SeedUsers(db);
        SeedUserRights(db, users, appObjects);
        SeedTourOperators(db);
    }

    #region Catalogs

    private static List<Country> SeedCountries(TravelCompanyDbContext db)
    {
        if (db.Countries.Any())
        {
            return db.Countries.ToList();
        }

        var countries = new List<Country>
        {
            new Country() { Name = "United Kingdom" },
            new Country() { Name = "Canada" },
            new Country() { Name = "Russia" },
            new Country() { Name = "Germany" },
            new Country() { Name = "France" },
            new Country() { Name = "Japan" },
            new Country() { Name = "Australia" },
            new Country() { Name = "Brazil" },
            new Country() { Name = "India" },
            new Country() { Name = "South Africa" },
        };
        db.AddRange(countries);
        db.SaveChanges();
        return countries;
    }

    private static List<Street> SeedStreets(TravelCompanyDbContext db)
    {
        if (db.Streets.Any())
        {
            return db.Streets.ToList();
        }

        var streets = new List<Street>
        {
            new Street() { Name = "Abbey Road" },
            new Street() { Name = "Baker Street" },
            new Street() { Name = "Buckingham Palace Road" },
            new Street() { Name = "Carlisle Street" },
            new Street() { Name = "Cavendish Square" },
            new Street() { Name = "Downing Street" },
            new Street() { Name = "Eaton Square" },
            new Street() { Name = "Fitzroy Street" },
            new Street() { Name = "Great George Street" },
            new Street() { Name = "Hanover Square" },
        };
        db.AddRange(streets);
        db.SaveChanges();
        return streets;
    }

    private static List<Hotel> SeedHotels(TravelCompanyDbContext db)
    {
        if (db.Hotels.Any())
        {
            return db.Hotels.ToList();
        }

        var hotels = new List<Hotel>
        {
            new Hotel() { Name = "The Royal Crown Hotel", Class = "Luxury" },
            new Hotel() { Name = "Windsor Manor", Class = "Budget" },
            new Hotel() { Name = "Thamesview Hotel & Spa", Class = "Budget" },
            new Hotel() { Name = "Highland Retreat", Class = "Budget" },
            new Hotel() { Name = "Coastal Haven Inn", Class = "Resort" },
            new Hotel() { Name = "Victoria Grand Hotel", Class = "Luxury" },
            new Hotel() { Name = "Greenwich Park Lodge", Class = "Budget" },
            new Hotel() { Name = "The Lakeside Retreat", Class = "Luxury" },
            new Hotel() { Name = "Cambridge Riverside Inn", Class = "Resort" },
            new Hotel() { Name = "Edinburgh Castle Hotel", Class = "Luxury" },
        };
        db.AddRange(hotels);
        db.SaveChanges();
        return hotels;
    }

    private static List<PopulatedPlace> SeedPopulatedPlaces(
        TravelCompanyDbContext db, List<Country> countries)
    {
        if (db.PopulatedPlaces.Any())
        {
            return db.PopulatedPlaces.ToList();
        }

        var populatedPlaces = new List<PopulatedPlace>
        {
            new PopulatedPlace()
            {
                Name = "London",
                CountryId = countries.FirstOrDefault(c => c.Name == "United Kingdom")!.Id
            },
            new PopulatedPlace()
            {
                Name = "Manchester",
                CountryId = countries.FirstOrDefault(c => c.Name == "Canada")!.Id
            },
            new PopulatedPlace()
            {
                Name = "Birmingham",
                CountryId = countries.FirstOrDefault(c => c.Name == "Russia")!.Id
            },
            new PopulatedPlace()
            {
                Name = "Liverpool",
                CountryId = countries.FirstOrDefault(c => c.Name == "Germany")!.Id
            },
            new PopulatedPlace()
            {
                Name = "Glasgow",
                CountryId = countries.FirstOrDefault(c => c.Name == "France")!.Id
            },
            new PopulatedPlace()
            {
                Name = "Leeds",
                CountryId = countries.FirstOrDefault(c => c.Name == "Japan")!.Id
            },
            new PopulatedPlace()
            {
                Name = "Bristol",
                CountryId = countries.FirstOrDefault(c => c.Name == "Australia")!.Id
            },
            new PopulatedPlace()
            {
                Name = "Sheffield",
                CountryId = countries.FirstOrDefault(c => c.Name == "Brazil")!.Id
            },
            new PopulatedPlace()
            {
                Name = "Edinburgh",
                CountryId = countries.FirstOrDefault(c => c.Name == "India")!.Id
            },
            new PopulatedPlace()
            {
                Name = "Cardiff",
                CountryId = countries.FirstOrDefault(c => c.Name == "South Africa")!.Id
            },
        };
        db.AddRange(populatedPlaces);
        db.SaveChanges();
        return populatedPlaces;
    }

    #endregion

    #region Other entities

    private static List<TourGuide> SeedEmployees(TravelCompanyDbContext db, List<Street> streets)
    {
        if (db.TourGuides.Any())
        {
            return db.TourGuides.ToList();
        }

        var employees = new List<TourGuide>
        {
            new TourGuide()
            {
                FirstName = "Vladimir",
                LastName = "Lushukov",
                Patronymic = "Evgenievich",
                StreetId = streets.FirstOrDefault(s => s.Name == "Abbey Road")!.Id,
                Birthdate = AgeGenerator.GetRandomDate(),
                Salary = 25000,
                IsFired = false,
                FiredDate = null,
            },
            new TourGuide()
            {
                FirstName = "John",
                LastName = "Doe",
                Patronymic = "Smith",
                StreetId = streets.FirstOrDefault(s => s.Name == "Baker Street")!.Id,
                Birthdate = AgeGenerator.GetRandomDate(),
                Salary = 30000,
                IsFired = false,
                FiredDate = null,
            },
            new TourGuide()
            {
                FirstName = "Jane",
                LastName = "Doe",
                Patronymic = "Smith",
                StreetId = streets.FirstOrDefault(s => s.Name == "Buckingham Palace Road")!.Id,
                Birthdate = AgeGenerator.GetRandomDate(),
                Salary = 35000,
                IsFired = false,
                FiredDate = null,
            },
            new TourGuide()
            {
                FirstName = "Bob",
                LastName = "Builder",
                Patronymic = "Smith",
                StreetId = streets.FirstOrDefault(s => s.Name == "Carlisle Street")!.Id,
                Birthdate = AgeGenerator.GetRandomDate(),
                Salary = 20000,
                IsFired = true,
                FiredDate = DateTime.Now.AddYears(-5),
            },
            new TourGuide()
            {
                FirstName = "Alice",
                LastName = "Wonderland",
                Patronymic = "Smith",
                StreetId = streets.FirstOrDefault(s => s.Name == "Cavendish Square")!.Id,
                Birthdate = AgeGenerator.GetRandomDate(),
                Salary = 40000,
                IsFired = false,
                FiredDate = null,
            },
        };
        db.AddRange(employees);
        db.SaveChanges();
        return employees;
    }

    private static List<Route> SeedRoutes(TravelCompanyDbContext db, List<Country> countries)
    {
        if (db.Routes.Any())
        {
            return db.Routes.ToList();
        }

        var routes = new List<Route>
        {
            new Route()
            {
                Name = "Great Ocean Road",
                Cost = 500,
                StartDatetime = DateTime.Now,
                EndDatetime = DateTime.Now.AddMonths(1),
                CountryId = countries.FirstOrDefault(c => c.Name == "United Kingdom")!.Id,
            },
            new Route()
            {
                Name = "Trollstigen",
                Cost = 1000,
                StartDatetime = DateTime.Now,
                EndDatetime = DateTime.Now.AddMonths(2),
                CountryId = countries.FirstOrDefault(c => c.Name == "Canada")!.Id,
            },
            new Route()
            {
                Name = "Golden Ring of Russia",
                Cost = 1500,
                StartDatetime = DateTime.Now.AddMonths(3),
                EndDatetime = DateTime.Now.AddMonths(4),
                CountryId = countries.FirstOrDefault(c => c.Name == "Russia")!.Id,
            },
            new Route()
            {
                Name = "The Garden Route",
                Cost = 2000,
                StartDatetime = DateTime.Now.AddMonths(4),
                EndDatetime = DateTime.Now.AddMonths(5),
                CountryId = countries.FirstOrDefault(c => c.Name == "South Africa")!.Id,
            },
            new Route()
            {
                Name = "The Atlantic Road",
                Cost = 2000,
                StartDatetime = DateTime.Now.AddMonths(7),
                EndDatetime = DateTime.Now.AddMonths(9),
                CountryId = countries.FirstOrDefault(c => c.Name == "Germany")!.Id,
            },
        };
        db.AddRange(routes);
        db.SaveChanges();
        return routes;
    }

    private static List<Payments> SeedPayments(TravelCompanyDbContext db)
    {
        if (db.Payments.Any())
        {
            return db.Payments.ToList();
        }

        

        var payments = new List<Payments>
    {
        new Payments()
        {
            Amount = 100,
            RouteId = 1,
            PaymentDate = DateTime.UtcNow,
            PaymentMethod = "Наличными",
            ClientId = 4,
            Status = "Оплачено",
            Comment = "Комментарий"
        },
        new Payments()
        {
            Amount = 300,
            RouteId = 2,
            PaymentDate = new DateTime(2025, 4, 21),
            PaymentMethod = "Наличными",
            ClientId = 3,
            Status = "Оплачено",
            Comment = "Комментарий"
        },
        new Payments()
        {
            Amount = 400,
            RouteId = 3,
            PaymentDate = new DateTime(2025, 3, 21),
             ClientId = 2,
            PaymentMethod = "Наличными",
            Status = "Оплачено",
            Comment = "Комментарий"
        },
        new Payments()
        {
            Amount = 200,
            RouteId = 4,
            PaymentDate = new DateTime(2025, 5, 21),
            PaymentMethod = "Наличными",
            ClientId = 1,
            Status = "Оплачено",
            Comment = "Комментарий"
        }
    };

        db.AddRange(payments);
        db.SaveChanges();
        return payments;
    }

    private static List<TouristGroup> SeedTouristGroups(
        TravelCompanyDbContext db, List<TourGuide> tourGuides, List<Route> routes)
    {
        if (db.TouristGroups.Any())
        {
            return db.TouristGroups.ToList();
        }

        var touristGroups = new List<TouristGroup>
        {
            new TouristGroup()
            {
                Name = DateTime.Now.Year + " Group 1",
                TourGuideId = tourGuides.FirstOrDefault(g => g.FirstName == "Vladimir")!.Id,
                RouteId = routes.FirstOrDefault(r => r.Name == "Golden Ring of Russia")!.Id,
            },
            new TouristGroup()
            {
                Name = DateTime.Now.Year + " Group 2",
                TourGuideId = tourGuides.FirstOrDefault(g => g.FirstName == "John")!.Id,
                RouteId = routes.FirstOrDefault(r => r.Name == "The Garden Route")!.Id,
            },
        };
        db.AddRange(touristGroups);
        db.SaveChanges();
        return touristGroups;
    }

    private static List<Client> SeedTourists(
        TravelCompanyDbContext db, List<Street> streets, List<TouristGroup> groups)
    {
        if (db.Clients.Any())
        {
            return db.Clients.ToList();
        }

        var clients = new List<Client>
        {

            new Client()
            {
                FirstName = "Evgenii",
                LastName = "Krasnov",
                Patronymic = "Antonovich",
                StreetId = streets.FirstOrDefault(s => s.Name == "Abbey Road")!.Id,
                Birthdate = AgeGenerator.GetRandomDate(),
                TouristGroups = new List<TouristGroup>
                {
                    groups.First(g => g.Name == $"{DateTime.Now.Year} Group 1"),
                    groups.First(g => g.Name == $"{DateTime.Now.Year} Group 2"),
                },
                PassportSeries = "03 11",
                PassportNumber = "793853",
                PassportIssueDate = DateTime.Now.AddYears(-18),
                PassportIssuingAuthority = "Her Majesty’s Passport Office",
                Photograph = null,
            },
            new Client()
            {
                FirstName = "Katelyn",
                LastName = "Brock",
                Patronymic = "Stevenson",
                StreetId = streets.FirstOrDefault(s => s.Name == "Baker Street")!.Id,
                Birthdate = AgeGenerator.GetRandomDate(),
                TouristGroups = new List<TouristGroup>
                {
                    groups.First(g => g.Name == $"{DateTime.Now.Year} Group 1")
                },
                PassportSeries = "03 12",
                PassportNumber = "803954",
                PassportIssueDate = DateTime.Now.AddYears(-19),
                PassportIssuingAuthority = "Her Majesty’s Passport Office",
                Photograph = null,
            },
            new Client()
            {
                FirstName = "Sami",
                LastName = "Fleming",
                Patronymic = "Li",
                StreetId = streets.FirstOrDefault(s => s.Name == "Buckingham Palace Road")!.Id,
                Birthdate = AgeGenerator.GetRandomDate(),
                TouristGroups = new List<TouristGroup>
                {
                    groups.First(g => g.Name == $"{DateTime.Now.Year} Group 1")
                },
                PassportSeries = "03 13",
                PassportNumber = "804955",
                PassportIssueDate = DateTime.Now.AddYears(-23),
                PassportIssuingAuthority = "Ministry of Internal Affairs",
                Photograph = null,
            },
            new Client()
            {
                FirstName = "Denis",
                LastName = "Mcmillan",
                Patronymic = "Reeves",
                StreetId = streets.FirstOrDefault(s => s.Name == "Carlisle Street")!.Id,
                Birthdate = AgeGenerator.GetRandomDate(),
                TouristGroups = new List<TouristGroup>
                {
                    groups.First(g => g.Name == $"{DateTime.Now.Year} Group 2")
                },
                PassportSeries = "03 14",
                PassportNumber = "805956",
                PassportIssueDate = DateTime.Now.AddYears(-21),
                PassportIssuingAuthority = "Ministry of Internal Affairs",
                Photograph = null,
            },
            new Client()
            {
                FirstName = "Laura",
                LastName = "Barret",
                Patronymic = "Banks",
                StreetId = streets.FirstOrDefault(s => s.Name == "Cavendish Square")!.Id,
                Birthdate = AgeGenerator.GetRandomDate(),
                TouristGroups = new List<TouristGroup>
                {
                    groups.First(g => g.Name == $"{DateTime.Now.Year} Group 2")
                },
                PassportSeries = "03 15",
                PassportNumber = "806957",
                PassportIssueDate = DateTime.Now.AddYears(-21),
                PassportIssuingAuthority = "Ministry of Internal Affairs",
                Photograph = null,
            },
        };
        db.AddRange(clients);
        db.SaveChanges();
        return clients;
    }

    private static void SeedPenalties(
        TravelCompanyDbContext db, List<Client> clients, List<TourGuide> employees)
    {
        if (db.Penalties.Any())
        {
            return;
        }

        var penalties = new List<Penalty>
        {
            new Penalty()
            {
                ClientId = clients.First().Id,
                TourGuideId = employees.First().Id,
                CompensationDate = DateTime.Now,
                CompensationDescription = "Broken bed in the hotel",
                CompensationAmount = 5700,
            },
        };
        db.AddRange(penalties);
        db.SaveChanges();
    }

    private static void SeedRoutesPopulatedPlaces(
        TravelCompanyDbContext db, List<Route> routes, List<PopulatedPlace> places, List<Hotel> hotels)
    {
        if (db.RoutesPopulatedPlaces.Any())
        {
            return;
        }

        var routesPopulatedPlaces = new List<RoutesPopulatedPlace>
        {
            new RoutesPopulatedPlace()
            {
                RouteId = routes.FirstOrDefault(r => r.Name == "Great Ocean Road")!.Id,
                PopulatedPlaceId = places.FirstOrDefault(p => p.Name == "London")!.Id,
                HotelId = hotels.FirstOrDefault(h => h.Name == "The Royal Crown Hotel")!.Id,
                StayStartDatetime = DateTime.Now.AddDays(1),
                StayEndDatetime = DateTime.Now.AddDays(15),
                ExcursionProgram = "An 8.3-kilometer road.",
            },
            new RoutesPopulatedPlace()
            {
                RouteId = routes.FirstOrDefault(r => r.Name == "Great Ocean Road")!.Id,
                PopulatedPlaceId = places.FirstOrDefault(p => p.Name == "Manchester")!.Id,
                HotelId = hotels.FirstOrDefault(h => h.Name == "Windsor Manor")!.Id,
                StayStartDatetime = DateTime.Now.AddDays(16),
                StayEndDatetime = DateTime.Now.AddDays(27),
                ExcursionProgram = "Beutiful plains.",
            },
            new RoutesPopulatedPlace()
            {
                RouteId = routes.FirstOrDefault(r => r.Name == "Trollstigen")!.Id,
                PopulatedPlaceId = places.FirstOrDefault(p => p.Name == "Manchester")!.Id,
                HotelId = hotels.FirstOrDefault(h => h.Name == "Windsor Manor")!.Id,
                StayStartDatetime = DateTime.Now.AddDays(1),
                StayEndDatetime = DateTime.Now.AddMonths(2),
                ExcursionProgram = "A mountainous road.",
            },
            new RoutesPopulatedPlace()
            {
                RouteId = routes.FirstOrDefault(r => r.Name == "Golden Ring of Russia")!.Id,
                PopulatedPlaceId = places.FirstOrDefault(p => p.Name == "Birmingham")!.Id,
                HotelId = hotels.FirstOrDefault(h => h.Name == "Thamesview Hotel & Spa")!.Id,
                StayStartDatetime = DateTime.Now.AddMonths(3),
                StayEndDatetime = DateTime.Now.AddMonths(4),
                ExcursionProgram = "The Golden Ring of Russia is the main and most popular" +
                " tourist route around provincial cities of central European Russia.",
            },
            new RoutesPopulatedPlace()
            {
                RouteId = routes.FirstOrDefault(r => r.Name == "The Garden Route")!.Id,
                PopulatedPlaceId = places.FirstOrDefault(p => p.Name == "Liverpool")!.Id,
                HotelId = hotels.FirstOrDefault(h => h.Name == "Thamesview Hotel & Spa")!.Id,
                StayStartDatetime = DateTime.Now.AddMonths(4),
                StayEndDatetime = DateTime.Now.AddMonths(5),
                ExcursionProgram = "Located on the south-western coast of South Africa, The Garden" +
                " Route is a 300-kilometer-long scenic route between Mossel Bay and Storms River," +
                " passing through a range of breathtaking landscapes such as lush forests, pristine" +
                " beaches, towering mountains, and tranquil lagoons.",
            },
        };
        db.AddRange(routesPopulatedPlaces);
        db.SaveChanges();
    }

    #endregion

    private static void SeedTourOperators(TravelCompanyDbContext db)
    {
        if (db.TourOperators.Any())
        {
            return;
        }

        var tourOperators = new List<TourOperator>()
        {
            new TourOperator()
            {
                Name="Pegas touristic",
                ContactInfo="pegastouristic.com",
                Address="Moscow, Baumana street 24a",
            },
            new TourOperator()
            {
                Name="AIS",
                ContactInfo="ais.com",
                Address="Moscow, Baumana street 24a",
            },
            new TourOperator()
            {
                Name="Uzum touristic",
                ContactInfo="uzum.com",
                Address="Moscow, Baumana street 24a",
            },
            new TourOperator()
            {
                Name="Avia touristic",
                ContactInfo="avia.com",
                Address="Moscow, Baumana street 24a",
            }
        };

        db.AddRange(tourOperators);
        db.SaveChangesAsync();

    }

    #region User Object UsersObjects

    private static List<Models.Object> SeedObjects(TravelCompanyDbContext db)
    {
        if (db.Objects.Any())
        {
            return db.Objects.ToList();
        }

        var appObjects = new List<Models.Object>
        {
            new Models.Object()
            {
                Name = "Travel Company App",
            },
            new Models.Object()
            {
                Name = "Catalogs",
            },
            new Models.Object()
            {
                Name = "Employees",
            },
            new Models.Object()
            {
                Name = "Clients",
            },
            new Models.Object()
            {
                Name = "Routes",
            },
            new Models.Object()
            {
                Name = "Tourist Groups",
            },
            new Models.Object()
            {
                Name = "Penalties",
            },
        };
        db.AddRange(appObjects);
        db.SaveChanges();
        return appObjects;
    }

    private static List<User> SeedUsers(TravelCompanyDbContext db)
    {
        if (db.Users.Any())
        {
            return db.Users.ToList();
        }

        var users = new List<User>
        {
            new User()
            {
                Username = "ADMIN",
                Password = "qwerty123",
            },
            new User()
            {
                Username = "EMPLOYEE",
                Password = "qwerty123",
            },
        };
        db.AddRange(users);
        db.SaveChanges();
        return users;
    }

    private static void SeedUserRights(
        TravelCompanyDbContext db, List<User> users, List<Models.Object> appObjects)
    {
        if (db.UsersObjects.Any())
        {
            return;
        }

        var usersObjects = new List<UsersObject>
        {
            // Admin
            new UsersObject()
            {
                UserId = users.FirstOrDefault(u => u.Username == "ADMIN")!.Id,
                ObjectId = appObjects.FirstOrDefault(u => u.Name == "Travel Company App")!.Id,
                CanCreate = true,
                CanRead = true,
                CanUpdate = true,
                CanDelete = true,
            },
            new UsersObject()
            {
                UserId = users.FirstOrDefault(u => u.Username == "ADMIN")!.Id,
                ObjectId = appObjects.FirstOrDefault(u => u.Name == "Catalogs")!.Id,
                CanCreate = true,
                CanRead = true,
                CanUpdate = true,
                CanDelete = true,
            },
            new UsersObject()
            {
                UserId = users.FirstOrDefault(u => u.Username == "ADMIN")!.Id,
                ObjectId = appObjects.FirstOrDefault(u => u.Name == "Employees")!.Id,
                CanCreate = true,
                CanRead = true,
                CanUpdate = true,
                CanDelete = true,
            },
            new UsersObject()
            {
                UserId = users.FirstOrDefault(u => u.Username == "ADMIN")!.Id,
                ObjectId = appObjects.FirstOrDefault(u => u.Name == "Clients")!.Id,
                CanCreate = true,
                CanRead = true,
                CanUpdate = true,
                CanDelete = true,
            },
            new UsersObject()
            {
                UserId = users.FirstOrDefault(u => u.Username == "ADMIN")!.Id,
                ObjectId = appObjects.FirstOrDefault(u => u.Name == "Routes")!.Id,
                CanCreate = true,
                CanRead = true,
                CanUpdate = true,
                CanDelete = true,
            },
            new UsersObject()
            {
                UserId = users.FirstOrDefault(u => u.Username == "ADMIN")!.Id,
                ObjectId = appObjects.FirstOrDefault(u => u.Name == "Tourist Groups")!.Id,
                CanCreate = true,
                CanRead = true,
                CanUpdate = true,
                CanDelete = true,
            },
            new UsersObject()
            {
                UserId = users.FirstOrDefault(u => u.Username == "ADMIN")!.Id,
                ObjectId = appObjects.FirstOrDefault(u => u.Name == "Penalties")!.Id,
                CanCreate = true,
                CanRead = true,
                CanUpdate = true,
                CanDelete = true,
            },

            // Employee
            new UsersObject()
            {
                UserId = users.FirstOrDefault(u => u.Username == "EMPLOYEE")!.Id,
                ObjectId = appObjects.FirstOrDefault(u => u.Name == "Travel Company App")!.Id,
                CanCreate = true,
                CanRead = true,
                CanUpdate = true,
                CanDelete = true,
            },
            new UsersObject()
            {
                UserId = users.FirstOrDefault(u => u.Username == "EMPLOYEE")!.Id,
                ObjectId = appObjects.FirstOrDefault(u => u.Name == "Catalogs")!.Id,
                CanCreate = true,
                CanRead = true,
                CanUpdate = true,
                CanDelete = true,
            },
            new UsersObject()
            {
                UserId = users.FirstOrDefault(u => u.Username == "EMPLOYEE")!.Id,
                ObjectId = appObjects.FirstOrDefault(u => u.Name == "Employees")!.Id,
                CanCreate = false,
                CanRead = false,
                CanUpdate = false,
                CanDelete = false,
            },
            new UsersObject()
            {
                UserId = users.FirstOrDefault(u => u.Username == "EMPLOYEE")!.Id,
                ObjectId = appObjects.FirstOrDefault(u => u.Name == "Clients")!.Id,
                CanCreate = true,
                CanRead = true,
                CanUpdate = true,
                CanDelete = true,
            },
            new UsersObject()
            {
                UserId = users.FirstOrDefault(u => u.Username == "EMPLOYEE")!.Id,
                ObjectId = appObjects.FirstOrDefault(u => u.Name == "Routes")!.Id,
                CanCreate = true,
                CanRead = true,
                CanUpdate = true,
                CanDelete = true,
            },
            new UsersObject()
            {
                UserId = users.FirstOrDefault(u => u.Username == "EMPLOYEE")!.Id,
                ObjectId = appObjects.FirstOrDefault(u => u.Name == "Tourist Groups")!.Id,
                CanCreate = true,
                CanRead = true,
                CanUpdate = true,
                CanDelete = true,
            },
            new UsersObject()
            {
                UserId = users.FirstOrDefault(u => u.Username == "EMPLOYEE")!.Id,
                ObjectId = appObjects.FirstOrDefault(u => u.Name == "Penalties")!.Id,
                CanCreate = true,
                CanRead = true,
                CanUpdate = true,
                CanDelete = true,
            },
        };
        db.AddRange(usersObjects);
        db.SaveChanges();
    }

    #endregion
}