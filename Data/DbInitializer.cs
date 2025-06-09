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
        // Users (must be seeded first due to Person dependency)
        var users = SeedUsers(db);

        // Catalogs
        var countries = SeedCountries(db);
        var streets = SeedStreets(db);
        var hotels = SeedHotels(db);
        var places = SeedPopulatedPlaces(db, countries);

        // Other entities
        var persons = SeedPersons(db, streets, users);
        var admins = SeedAdmins(db, persons);
        var clients = SeedClients(db, persons);
        var passports = SeedPassports(db, persons);
        var employees = SeedEmployees(db, persons);
        var tourGuides = SeedTourGuides(db, persons, employees);
        var tourOperators = SeedTourOperators(db);
        var routes = SeedRoutes(db, countries, tourOperators);
        var groups = SeedTouristGroups(db, tourGuides, routes);
        SeedClientsTouristGroups(db, clients, groups);

        SeedPenalties(db, clients, tourGuides);
        SeedPayments(db, clients, routes);
        SeedRoutesPopulatedPlaces(db, routes, places, hotels);

        // Attractions and user rights
        
        
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
            new Country { Name = "Великобритания" },
            new Country { Name = "Канада" },
            new Country { Name = "Россия" },
            new Country { Name = "Германия" },
            new Country { Name = "Франция" },
            new Country { Name = "Япония" },
            new Country { Name = "Австралия" },
            new Country { Name = "Бразилия" },
            new Country { Name = "Индия" },
            new Country { Name = "Южная Африка" },
            new Country { Name = "Норвегия" },
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
            new Street { Name = "Улица Ленина" },
            new Street { Name = "Проспект Мира" },
            new Street { Name = "Красная площадь" },
            new Street { Name = "Улица Горького" },
            new Street { Name = "Тверская улица" },
            new Street { Name = "Арбат" },
            new Street { Name = "Садовая улица" },
            new Street { Name = "Новослободская улица" },
            new Street { Name = "Большая Никитская улица" },
            new Street { Name = "Малая Дмитровка" },
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
            new Hotel { Name = "Гостиница Королевский дворец", Class = "Люкс" },
            new Hotel { Name = "Усадьба Виндзор", Class = "Эконом" },
            new Hotel { Name = "Отель Темза и Спа", Class = "Эконом" },
            new Hotel { Name = "Горный приют", Class = "Эконом" },
            new Hotel { Name = "Побережье Гавань", Class = "Курорт" },
            new Hotel { Name = "Гранд-отель Виктория", Class = "Люкс" },
            new Hotel { Name = "Лодж Гринвич Парк", Class = "Эконом" },
            new Hotel { Name = "Озерный приют", Class = "Люкс" },
            new Hotel { Name = "Отель Кембридж Риверсайд", Class = "Курорт" },
            new Hotel { Name = "Отель Эдинбургский замок", Class = "Люкс" },
        };
        db.AddRange(hotels);
        db.SaveChanges();
        return hotels;
    }

    private static List<PopulatedPlace> SeedPopulatedPlaces(TravelCompanyDbContext db, List<Country> countries)
    {
        if (db.PopulatedPlaces.Any())
        {
            return db.PopulatedPlaces.ToList();
        }

        var russia = countries.FirstOrDefault(c => c.Name == "Россия") ?? throw new InvalidOperationException("Country 'Россия' not found.");
        var populatedPlaces = new List<PopulatedPlace>
        {
            new PopulatedPlace { Name = "Москва", CountryId = russia.Id },
            new PopulatedPlace { Name = "Санкт-Петербург", CountryId = russia.Id },
            new PopulatedPlace { Name = "Новосибирск", CountryId = russia.Id },
            new PopulatedPlace { Name = "Екатеринбург", CountryId = russia.Id },
            new PopulatedPlace { Name = "Казань", CountryId = russia.Id },
            new PopulatedPlace { Name = "Нижний Новгород", CountryId = russia.Id },
            new PopulatedPlace { Name = "Челябинск", CountryId = russia.Id },
            new PopulatedPlace { Name = "Самара", CountryId = russia.Id },
            new PopulatedPlace { Name = "Омск", CountryId = russia.Id },
            new PopulatedPlace { Name = "Ростов-на-Дону", CountryId = russia.Id },
        };
        db.AddRange(populatedPlaces);
        db.SaveChanges();
        return populatedPlaces;
    }

    #endregion

    #region Other Entities

    private static List<Person> SeedPersons(TravelCompanyDbContext db, List<Street> streets, List<User> users)
    {
        var existingPersons = db.Persons.ToList();
        if (existingPersons.Count >= 5)
        {
            return existingPersons;
        }

        var personsToAdd = new List<Person>();
        var requiredPersons = 5 - existingPersons.Count;

        var adminUser = users.FirstOrDefault(u => u.Username == "АДМИН") ?? throw new InvalidOperationException("User 'АДМИН' not found.");
        var employeeUser = users.FirstOrDefault(u => u.Username == "СОТРУДНИК") ?? throw new InvalidOperationException("User 'СОТРУДНИК' not found.");
        var leninaStreet = streets.FirstOrDefault(s => s.Name == "Улица Ленина") ?? throw new InvalidOperationException("Street 'Улица Ленина' not found.");
        var miraStreet = streets.FirstOrDefault(s => s.Name == "Проспект Мира") ?? throw new InvalidOperationException("Street 'Проспект Мира' not found.");
        var krasnayaStreet = streets.FirstOrDefault(s => s.Name == "Красная площадь") ?? throw new InvalidOperationException("Street 'Красная площадь' not found.");
        var gorkogoStreet = streets.FirstOrDefault(s => s.Name == "Улица Горького") ?? throw new InvalidOperationException("Street 'Улица Горького' not found.");
        var tverskayaStreet = streets.FirstOrDefault(s => s.Name == "Тверская улица") ?? throw new InvalidOperationException("Street 'Тверская улица' not found.");

        var possiblePersons = new List<Person>
        {
            new Person { FirstName = "Владимир", LastName = "Лушуков", Patronymic = "Евгеньевич", Birthdate = DateTime.Now.AddYears(-30), StreetId = leninaStreet.Id, UserId = adminUser.Id },
            new Person { FirstName = "Иван", LastName = "Иванов", Patronymic = "Петрович", Birthdate = DateTime.Now.AddYears(-25), StreetId = miraStreet.Id, UserId = employeeUser.Id },
            new Person { FirstName = "Мария", LastName = "Петрова", Patronymic = "Александровна", Birthdate = DateTime.Now.AddYears(-28), StreetId = krasnayaStreet.Id, UserId = null },
            new Person { FirstName = "Алексей", LastName = "Сидоров", Patronymic = "Викторович", Birthdate = DateTime.Now.AddYears(-35), StreetId = gorkogoStreet.Id, UserId = null },
            new Person { FirstName = "Елена", LastName = "Кузнецова", Patronymic = "Сергеевна", Birthdate = DateTime.Now.AddYears(-40), StreetId = tverskayaStreet.Id, UserId = null },
        };

        personsToAdd.AddRange(possiblePersons.Take(requiredPersons));
        if (personsToAdd.Any())
        {
            db.AddRange(personsToAdd);
            db.SaveChanges();
            existingPersons.AddRange(personsToAdd);
        }

        return existingPersons;
    }

    private static List<Admin> SeedAdmins(TravelCompanyDbContext db, List<Person> persons)
    {
        if (db.Admins.Any())
        {
            return db.Admins.AsNoTracking().ToList(); // Use AsNoTracking to avoid navigation property loading
        }

        if (persons.Count < 1)
        {
            throw new InvalidOperationException($"Not enough Person entities to seed Admins. Expected at least 1, but found {persons.Count}.");
        }

        // Validate PersonId exists
        if (!db.Persons.Any(p => p.Id == persons[0].Id))
        {
            throw new InvalidOperationException($"Person with Id {persons[0].Id} does not exist.");
        }

        var admins = new List<Admin>
    {
        new Admin { PersonId = persons[0].Id, Username = "admin", Password = "hashedpassword123" }
    };
        db.AddRange(admins);
        db.SaveChanges();
        return admins;
    }

    private static List<Client> SeedClients(TravelCompanyDbContext db, List<Person> persons)
    {
        if (db.Clients.Any())
        {
            return db.Clients.Include(c => c.Person).ToList(); // Error likely occurs here
        }
        if (persons.Count < 5)
        {
            throw new InvalidOperationException($"Not enough Person entities to seed Clients. Expected at least 5, but found {persons.Count}.");
        }
        var clients = new List<Client>
    {
        new Client { PersonId = persons[0].Id, Photograph = null },
        new Client { PersonId = persons[1].Id, Photograph = null },
        new Client { PersonId = persons[2].Id, Photograph = null },
        new Client { PersonId = persons[3].Id, Photograph = null },
        new Client { PersonId = persons[4].Id, Photograph = null },
    };
        db.Clients.AddRange(clients);
        db.SaveChanges();
        return db.Clients.Include(c => c.Person).ToList();
    }

    private static List<Passport> SeedPassports(TravelCompanyDbContext db, List<Person> persons)
    {
        if (db.Passports.Any())
        {
            return db.Passports.ToList();
        }

        if (persons.Count < 5)
        {
            throw new InvalidOperationException($"Not enough Person entities to seed Passports. Expected at least 5, but found {persons.Count}.");
        }

        var passports = new List<Passport>
        {
            new Passport { PersonId = persons[0].Id, PassportSeries = "0311", PassportNumber = "793853", PassportIssueDate = DateTime.Now.AddYears(-18), PassportIssuingAuthority = "Паспортный стол МВД" },
            new Passport { PersonId = persons[1].Id, PassportSeries = "0312", PassportNumber = "803946", PassportIssueDate = DateTime.Now.AddYears(-19), PassportIssuingAuthority = "Паспортный стол МВД" },
            new Passport { PersonId = persons[2].Id, PassportSeries = "0313", PassportNumber = "804957", PassportIssueDate = DateTime.Now.AddYears(-21), PassportIssuingAuthority = "Паспортный стол МВД" },
            new Passport { PersonId = persons[3].Id, PassportSeries = "0314", PassportNumber = "805956", PassportIssueDate = DateTime.Now.AddYears(-21), PassportIssuingAuthority = "Паспортный стол МВД" },
            new Passport { PersonId = persons[4].Id, PassportSeries = "0315", PassportNumber = "906957", PassportIssueDate = DateTime.Now.AddYears(-20), PassportIssuingAuthority = "Паспортный стол МВД" },
        };
        db.AddRange(passports);
        db.SaveChanges();
        return passports;
    }

    private static List<Employee> SeedEmployees(TravelCompanyDbContext db, List<Person> persons)
    {
        if (db.Employees.Any())
        {
            return db.Employees.Include(e => e.Person).ToList();
        }

        if (persons.Count < 5)
        {
            throw new InvalidOperationException($"Not enough Person entities to seed Employees. Expected at least 5, but found {persons.Count}.");
        }

        var employees = new List<Employee>
        {
            new Employee { PersonId = persons[0].Id, IsDismissed = false },
            new Employee { PersonId = persons[1].Id, IsDismissed = false },
            new Employee { PersonId = persons[2].Id, IsDismissed = false },
            new Employee { PersonId = persons[3].Id, IsDismissed = true, DismissalDate = DateTime.Now.AddYears(-5) },
            new Employee { PersonId = persons[4].Id, IsDismissed = false },
        };

        db.Employees.AddRange(employees);
        db.SaveChanges();

        return db.Employees.Include(e => e.Person).ToList();
    }

    private static List<TourGuide> SeedTourGuides(TravelCompanyDbContext db, List<Person> persons, List<Employee> employees)
    {
        if (db.TourGuides.Any())
        {
            return db.TourGuides.Include(tg => tg.Person).Include(tg => tg.Employee).ToList();
        }

        if (persons.Count < 5 || employees.Count < 5)
        {
            throw new InvalidOperationException($"Not enough Person or Employee entities to seed TourGuides. Expected at least 5, found {persons.Count} Persons and {employees.Count} Employees.");
        }

        var tourGuides = new List<TourGuide>();
        for (int i = 0; i < 5; i++)
        {
            // Ensure PersonId exists in Person table and EmployeeId exists in Employee table
            if (!db.Persons.Any(p => p.Id == persons[i].Id) || !db.Employees.Any(e => e.Id == employees[i].Id))
            {
                throw new InvalidOperationException($"Invalid PersonId {persons[i].Id} or EmployeeId {employees[i].Id} for TourGuide at index {i}.");
            }

            tourGuides.Add(new TourGuide
            {
                PersonId = persons[i].Id,
                EmployeeId = employees[i].Id,
                Salary = 25000 + (i * 5000),
                IsFired = i == 3, // Mark the 4th tour guide as fired for variety
                FiredDate = i == 3 ? DateTime.Now.AddYears(-5) : null
            });
        }

        db.TourGuides.AddRange(tourGuides);
        db.SaveChanges();

        return db.TourGuides.Include(tg => tg.Person).Include(tg => tg.Employee).ToList();
    }

    private static void SeedClientsTouristGroups(TravelCompanyDbContext db, List<Client> clients, List<TouristGroup> touristGroups)
    {
        // Check if any relationships exist in the junction table
        if (db.Set<Dictionary<string, object>>("clientstouristgroups").Any())
        {
            return;
        }

        // Ensure clients and tourist groups are tracked by the context
        clients = db.Clients.Where(c => clients.Select(cl => cl.Id).Contains(c.Id)).ToList();
        touristGroups = db.TouristGroups.Where(tg => touristGroups.Select(t => t.Id).Contains(tg.Id)).ToList();

        // Establish relationships using navigation properties
        clients[0].TouristGroups.Add(touristGroups[0]); // Client 0 joins Group 0
        clients[1].TouristGroups.Add(touristGroups[1]); // Client 1 joins Group 1
        clients[2].TouristGroups.Add(touristGroups[0]); // Client 2 joins Group 0

        // Save changes to persist the relationships
        db.SaveChanges();
    }

    private static List<TourOperator> SeedTourOperators(TravelCompanyDbContext db)
    {
        if (db.TourOperators.Any())
        {
            return db.TourOperators.ToList();
        }

        var tourOperators = new List<TourOperator>
        {
            new TourOperator { Name = "Пегас Туристик", ContactPhone = "+7-495-123-4567", Email = "contact@pegastour.ru", Address = "Москва, ул. Баумана 24а" },
            new TourOperator { Name = "АИС", ContactPhone = "+7-495-234-5678", Email = "contact@ais.ru", Address = "Москва, ул. Баумана 24а" },
            new TourOperator { Name = "Узум Туристик", ContactPhone = "+7-495-345-6789", Email = "contact@uzum.ru", Address = "Москва, ул. Баумана 24а" },
            new TourOperator { Name = "Авиа Туристик", ContactPhone = "+7-495-456-7890", Email = "contact@avia.ru", Address = "Москва, ул. Баумана 24а" },
        };
        db.AddRange(tourOperators);
        db.SaveChanges();
        return tourOperators;
    }

    private static List<Route> SeedRoutes(TravelCompanyDbContext db, List<Country> countries, List<TourOperator> tourOperators)
    {
        if (db.Routes.Any())
        {
            return db.Routes.ToList();
        }

        var russia = countries.FirstOrDefault(c => c.Name == "Россия") ?? throw new InvalidOperationException("Country 'Россия' not found.");
        var southAfrica = countries.FirstOrDefault(c => c.Name == "Южная Африка") ?? throw new InvalidOperationException("Country 'Южная Африка' not found.");
        var germany = countries.FirstOrDefault(c => c.Name == "Германия") ?? throw new InvalidOperationException("Country 'Германия' not found.");
        var australia = countries.FirstOrDefault(c => c.Name == "Австралия") ?? throw new InvalidOperationException("Country 'Австралия' not found.");
        var norway = countries.FirstOrDefault(c => c.Name == "Норвегия") ?? throw new InvalidOperationException("Country 'Норвегия' not found.");

        var routes = new List<Route>
        {
            new Route { Name = "Золотое кольцо России", Cost = 15000, StartDatetime = DateTime.Now.AddMonths(3), EndDatetime = DateTime.Now.AddMonths(3).AddDays(7), CountryId = russia.Id, TourOperatorId = tourOperators[0].Id },
            new Route { Name = "Садовый маршрут", Cost = 20000, StartDatetime = DateTime.Now.AddMonths(4), EndDatetime = DateTime.Now.AddMonths(4).AddDays(7), CountryId = southAfrica.Id, TourOperatorId = tourOperators[1].Id },
            new Route { Name = "Дорога Атлантического побережья", Cost = 25000, StartDatetime = DateTime.Now.AddMonths(7), EndDatetime = DateTime.Now.AddMonths(7).AddDays(10), CountryId = germany.Id, TourOperatorId = tourOperators[0].Id },
            new Route { Name = "Великая океанская дорога", Cost = 5000, StartDatetime = DateTime.Now.AddMonths(1), EndDatetime = DateTime.Now.AddMonths(1).AddDays(5), CountryId = australia.Id, TourOperatorId = tourOperators[2].Id },
            new Route { Name = "Тролльстиген", Cost = 10000, StartDatetime = DateTime.Now.AddMonths(2), EndDatetime = DateTime.Now.AddMonths(2).AddDays(7), CountryId = norway.Id, TourOperatorId = tourOperators[3].Id }, // Fixed: Corrected "Тролльт" to "Тролльстиген"
        };
        db.AddRange(routes);
        db.SaveChanges();
        return routes;
    }

    private static List<TouristGroup> SeedTouristGroups(TravelCompanyDbContext db, List<TourGuide> tourGuides, List<Route> routes)
    {
        if (db.TouristGroups.Any())
        {
            return db.TouristGroups.Include(tg => tg.TourGuide).Include(tg => tg.Route).ToList();
        }

        var vladimirGuide = tourGuides.FirstOrDefault(g => g.Person.FirstName == "Владимир") ?? tourGuides.FirstOrDefault() ?? throw new InvalidOperationException("No TourGuide available.");
        var ivanGuide = tourGuides.FirstOrDefault(g => g.Person.FirstName == "Иван") ?? tourGuides.Skip(1).FirstOrDefault() ?? throw new InvalidOperationException("No second TourGuide available.");
        var goldenRingRoute = routes.FirstOrDefault(r => r.Name == "Золотое кольцо России") ?? throw new InvalidOperationException("Route 'Золотое кольцо России' not found.");
        var gardenRoute = routes.FirstOrDefault(r => r.Name == "Садовый маршрут") ?? throw new InvalidOperationException("Route 'Садовый маршрут' not found.");

        var touristGroups = new List<TouristGroup>
    {
        new TouristGroup
        {
            Name = $"{DateTime.Now.Year} Группа 1",
            TourGuideId = vladimirGuide.Id,
            RouteId = goldenRingRoute.Id,
            StartDatetime = goldenRingRoute.StartDatetime,
            EndDatetime = goldenRingRoute.EndDatetime
        },
        new TouristGroup
        {
            Name = $"{DateTime.Now.Year} Группа 2",
            TourGuideId = ivanGuide.Id,
            RouteId = gardenRoute.Id,
            StartDatetime = gardenRoute.StartDatetime,
            EndDatetime = gardenRoute.EndDatetime
        },
    };
        db.AddRange(touristGroups);
        db.SaveChanges();
        return db.TouristGroups.Include(tg => tg.TourGuide).Include(tg => tg.Route).ToList();
    }

    private static void SeedPenalties(TravelCompanyDbContext db, List<Client> clients, List<TourGuide> tourGuides)
    {
        if (db.Penalties.Any())
        {
            return;
        }

        if (clients.Count < 1 || tourGuides.Count < 2)
        {
            throw new InvalidOperationException($"Not enough Clients or TourGuides to seed Penalties. Found {clients.Count} Clients and {tourGuides.Count} TourGuides.");
        }

        var penalties = new List<Penalty>
    {
        new Penalty
        {
            ClientId = clients[0].Id,
            TourGuideId = tourGuides[1].Id,
            CompensationDate = DateTime.Now,
            Amount = 5700,
            Reason = "Сломанная кровать в отеле"
        },
    };
        db.AddRange(penalties);
        db.SaveChanges();
    }

    private static List<Payment> SeedPayments(TravelCompanyDbContext db, List<Client> clients, List<Route> routes)
    {
        if (db.Payments.Any())
        {
            return db.Payments.ToList();
        }

        if (clients.Count < 4 || routes.Count < 4)
        {
            throw new InvalidOperationException($"Not enough Clients or Routes to seed Payments. Found {clients.Count} Clients and {routes.Count} Routes.");
        }

        var payments = new List<Payment>
    {
        new Payment { Amount = 10000, RouteId = routes[0].Id, PaymentDate = DateTime.UtcNow.AddDays(1), PaymentMethod = "Наличными", ClientId = clients[0].Id, Status = "Оплачено", Comment = "Оплата за тур" },
        new Payment { Amount = 3000, RouteId = routes[1].Id, PaymentDate = new DateTime(2025, 4, 10), PaymentMethod = "Картой", ClientId = clients[1].Id, Status = "Оплачено", Comment = "Оплата за тур" },
        new Payment { Amount = 4000, RouteId = routes[2].Id, PaymentDate = new DateTime(2025, 3, 15), PaymentMethod = "Наличными", ClientId = clients[2].Id, Status = "Оплачено", Comment = "Оплата за тур" },
        new Payment { Amount = 2000, RouteId = routes[3].Id, PaymentDate = new DateTime(2025, 5, 20), PaymentMethod = "Картой", ClientId = clients[3].Id, Status = "Оплачено", Comment = "Оплата за тур" },
    };

        try
        {
            db.AddRange(payments);
            db.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding Payments: {ex.Message}");
            throw;
        }

        return payments;
    }

    private static void SeedRoutesPopulatedPlaces(TravelCompanyDbContext db, List<Route> routes, List<PopulatedPlace> places, List<Hotel> hotels)
    {
        if (db.RoutesPopulatedPlaces.Any())
        {
            return;
        }

        var routesPopulatedPlaces = new List<RoutesPopulatedPlace>
        {
            new RoutesPopulatedPlace { RouteId = routes[0].Id, PopulatedPlaceId = places[0].Id, HotelId = hotels[0].Id, StayStartDatetime = DateTime.Now.AddDays(1), StayEndDatetime = DateTime.Now.AddDays(3), ExcursionProgram = "Золотое кольцо России - экскурсии по Москве" },
            new RoutesPopulatedPlace { RouteId = routes[1].Id, PopulatedPlaceId = places[1].Id, HotelId = hotels[1].Id, StayStartDatetime = DateTime.Now.AddDays(4), StayEndDatetime = DateTime.Now.AddDays(5), ExcursionProgram = "Садовый маршрут по Санкт-Петербургу" },
            new RoutesPopulatedPlace { RouteId = routes[2].Id, PopulatedPlaceId = places[2].Id, HotelId = hotels[2].Id, StayStartDatetime = DateTime.Now.AddDays(7), StayEndDatetime = DateTime.Now.AddDays(9), ExcursionProgram = "Дорога Атлантического побережья" },
            new RoutesPopulatedPlace { RouteId = routes[3].Id, PopulatedPlaceId = places[3].Id, HotelId = hotels[3].Id, StayStartDatetime = DateTime.Now.AddDays(1), StayEndDatetime = DateTime.Now.AddDays(5), ExcursionProgram = "Великая океанская дорога" },
            new RoutesPopulatedPlace { RouteId = routes[4].Id, PopulatedPlaceId = places[4].Id, HotelId = hotels[4].Id, StayStartDatetime = DateTime.Now.AddDays(1), StayEndDatetime = DateTime.Now.AddDays(7), ExcursionProgram = "Тролльстиген - горные маршруты" },
        };
        db.AddRange(routesPopulatedPlaces);
        db.SaveChanges();
    }

    #region User Attraction Rights

    

    private static List<User> SeedUsers(TravelCompanyDbContext db)
    {
        if (db.Users.Any())
        {
            return db.Users.ToList();
        }

        var users = new List<User>
        {
            new User { Username = "АДМИН", Password = "qwerty123" },
            new User { Username = "СОТРУДНИК", Password = "qwerty123" },
        };
        db.AddRange(users);
        db.SaveChanges();
        return users;
        #endregion
    }



    #endregion
}