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
        var users = SeedUsers(db); // Moved up to use in Persons

        // Catalogs
        var countries = SeedCountries(db);
        var streets = SeedStreets(db);
        var hotels = SeedHotels(db);
        var places = SeedPopulatedPlaces(db, countries);

        // Other entities
        var persons = SeedPersons(db, streets, users); // Pass users
        var clients = SeedClients(db, persons);
        var passports = SeedPassports(db, clients);
        var tourGuides = SeedTourGuides(db, persons);
        var tourOperators = SeedTourOperators(db);
        var routes = SeedRoutes(db, countries, tourOperators);
        var groups = SeedTouristGroups(db, tourGuides, routes);

        SeedPenalties(db, clients, tourGuides);
        SeedPayments(db, clients, routes);
        SeedRoutesPopulatedPlaces(db, routes, places, hotels);

        // Attractions and user rights
        var attractions = SeedAttractions(db);
        SeedUserRights(db, users, attractions);
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
            new Hotel { Name = "Отель Кембридж Риверсайd", Class = "Курорт" },
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

        var populatedPlaces = new List<PopulatedPlace>
        {
            new PopulatedPlace { Name = "Москва", CountryId = countries.FirstOrDefault(c => c.Name == "Россия")!.Id },
            new PopulatedPlace { Name = "Санкт-Петербург", CountryId = countries.FirstOrDefault(c => c.Name == "Россия")!.Id },
            new PopulatedPlace { Name = "Новосибирск", CountryId = countries.FirstOrDefault(c => c.Name == "Россия")!.Id },
            new PopulatedPlace { Name = "Екатеринбург", CountryId = countries.FirstOrDefault(c => c.Name == "Россия")!.Id },
            new PopulatedPlace { Name = "Казань", CountryId = countries.FirstOrDefault(c => c.Name == "Россия")!.Id },
            new PopulatedPlace { Name = "Нижний Новгород", CountryId = countries.FirstOrDefault(c => c.Name == "Россия")!.Id },
            new PopulatedPlace { Name = "Челябинск", CountryId = countries.FirstOrDefault(c => c.Name == "Россия")!.Id },
            new PopulatedPlace { Name = "Самара", CountryId = countries.FirstOrDefault(c => c.Name == "Россия")!.Id },
            new PopulatedPlace { Name = "Омск", CountryId = countries.FirstOrDefault(c => c.Name == "Россия")!.Id },
            new PopulatedPlace { Name = "Ростов-на-Дону", CountryId = countries.FirstOrDefault(c => c.Name == "Россия")!.Id },
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

        var possiblePersons = new List<Person>
    {
        new Person { FirstName = "Владимир", LastName = "Лушуков", Patronymic = "Евгеньевич", Birthdate = AgeGenerator.GetRandomDate(), StreetId = streets.FirstOrDefault(s => s.Name == "Улица Ленина")!.Id, UserId = users.FirstOrDefault(u => u.Username == "АДМИН")!.Id },
        new Person { FirstName = "Иван", LastName = "Иванов", Patronymic = "Петрович", Birthdate = AgeGenerator.GetRandomDate(), StreetId = streets.FirstOrDefault(s => s.Name == "Проспект Мира")!.Id, UserId = users.FirstOrDefault(u => u.Username == "СОТРУДНИК")!.Id },
        new Person { FirstName = "Мария", LastName = "Петрова", Patronymic = "Александровна", Birthdate = AgeGenerator.GetRandomDate(), StreetId = streets.FirstOrDefault(s => s.Name == "Красная площадь")!.Id, UserId = null },
        new Person { FirstName = "Алексей", LastName = "Сидоров", Patronymic = "Викторович", Birthdate = AgeGenerator.GetRandomDate(), StreetId = streets.FirstOrDefault(s => s.Name == "Улица Горького")!.Id, UserId = null },
        new Person { FirstName = "Елена", LastName = "Кузнецова", Patronymic = "Сергеевна", Birthdate = AgeGenerator.GetRandomDate(), StreetId = streets.FirstOrDefault(s => s.Name == "Тверская улица")!.Id, UserId = null },
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

    private static List<Client> SeedClients(TravelCompanyDbContext db, List<Person> persons)
    {
        if (db.Clients.Any())
        {
            return db.Clients.Include(c => c.Person).ToList();
        }

        if (persons.Count < 5)
        {
            throw new InvalidOperationException($"Not enough Person entities to seed Clients. Expected at least 5, but found {persons.Count}.");
        }

        var clients = new List<Client>
    {
        new Client { PersonId = persons[0].Id, Photograph = null, Person = persons[0] },
        new Client { PersonId = persons[1].Id, Photograph = null, Person = persons[1] },
        new Client { PersonId = persons[2].Id, Photograph = null, Person = persons[2] },
        new Client { PersonId = persons[3].Id, Photograph = null, Person = persons[3] },
        new Client { PersonId = persons[4].Id, Photograph = null, Person = persons[4] },
    };

        foreach (var client in clients)
        {
            db.Clients.Add(client);
            db.SaveChanges();
        }

        return db.Clients.Include(c => c.Person).ToList();
    }

    private static List<Passport> SeedPassports(TravelCompanyDbContext db, List<Client> clients)
    {
        if (db.Passports.Any())
        {
            return db.Passports.ToList();
        }

        if (clients.Count < 5)
        {
            throw new InvalidOperationException($"Not enough Client entities to seed Passports. Expected at least 5, but found {clients.Count}.");
        }

        var passports = new List<Passport>
        {
            new Passport { Id = clients[0].Id, PassportSeries = "0311", PassportNumber = "793853", PassportIssueDate = DateTime.Now.AddYears(-18), PassportIssuingAuthority = "Паспортный стол МВД" },
            new Passport { Id = clients[1].Id, PassportSeries = "0312", PassportNumber = "803954", PassportIssueDate = DateTime.Now.AddYears(-19), PassportIssuingAuthority = "Паспортный стол МВД" },
            new Passport { Id = clients[2].Id, PassportSeries = "0313", PassportNumber = "804955", PassportIssueDate = DateTime.Now.AddYears(-23), PassportIssuingAuthority = "Паспортный стол МВД" },
            new Passport { Id = clients[3].Id, PassportSeries = "0314", PassportNumber = "805956", PassportIssueDate = DateTime.Now.AddYears(-21), PassportIssuingAuthority = "Паспортный стол МВД" },
            new Passport { Id = clients[4].Id, PassportSeries = "0315", PassportNumber = "806957", PassportIssueDate = DateTime.Now.AddYears(-21), PassportIssuingAuthority = "Паспортный стол МВД" },
        };
        db.AddRange(passports);
        db.SaveChanges();
        return passports;
    }

    private static List<TourGuide> SeedTourGuides(TravelCompanyDbContext db, List<Person> persons)
    {
        if (db.TourGuides.Any())
        {
            return db.TourGuides.ToList();
        }

        if (persons.Count < 5)
        {
            throw new InvalidOperationException($"Not enough Person entities to seed TourGuides. Expected at least 5, but found {persons.Count}.");
        }

        var tourGuides = new List<TourGuide>
        {
            new TourGuide { PersonId = persons[0].Id, Salary = 25000, IsFired = false, FiredDate = null },
            new TourGuide { PersonId = persons[1].Id, Salary = 30000, IsFired = false, FiredDate = null },
            new TourGuide { PersonId = persons[2].Id, Salary = 35000, IsFired = false, FiredDate = null },
            new TourGuide { PersonId = persons[3].Id, Salary = 20000, IsFired = true, FiredDate = DateTime.Now.AddYears(-5) },
            new TourGuide { PersonId = persons[4].Id, Salary = 40000, IsFired = false, FiredDate = null },
        };

        foreach (var tourGuide in tourGuides)
        {
            db.TourGuides.Add(tourGuide);
            db.SaveChanges();
        }

        return tourGuides;
    }

    private static List<TourOperator> SeedTourOperators(TravelCompanyDbContext db)
    {
        if (db.TourOperators.Any())
        {
            return db.TourOperators.ToList();
        }

        var tourOperators = new List<TourOperator>
        {
            new TourOperator { Name = "Пегас Туристик", ContactPhone = "+7-495-123-4567", Email = "contact@pegastouristic.ru", Address = "Москва, ул. Баумана 24а" },
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

        var routes = new List<Route>
        {
            new Route { Name = "Золотое кольцо России", Cost = 1500, StartDatetime = DateTime.Now.AddMonths(3), EndDatetime = DateTime.Now.AddMonths(4), CountryId = countries.FirstOrDefault(c => c.Name == "Россия")!.Id, TourOperatorId = tourOperators[0].Id },
            new Route { Name = "Садовый маршрут", Cost = 2000, StartDatetime = DateTime.Now.AddMonths(4), EndDatetime = DateTime.Now.AddMonths(5), CountryId = countries.FirstOrDefault(c => c.Name == "Южная Африка")!.Id, TourOperatorId = tourOperators[1].Id },
            new Route { Name = "Дорога Атлантического побережья", Cost = 2000, StartDatetime = DateTime.Now.AddMonths(7), EndDatetime = DateTime.Now.AddMonths(9), CountryId = countries.FirstOrDefault(c => c.Name == "Германия")!.Id, TourOperatorId = tourOperators[2].Id },
            new Route { Name = "Великая океанская дорога", Cost = 500, StartDatetime = DateTime.Now, EndDatetime = DateTime.Now.AddMonths(1), CountryId = countries.FirstOrDefault(c => c.Name == "Австралия")!.Id, TourOperatorId = tourOperators[3].Id },
            new Route { Name = "Тролльстиген", Cost = 1000, StartDatetime = DateTime.Now, EndDatetime = DateTime.Now.AddMonths(2), CountryId = countries.FirstOrDefault(c => c.Name == "Норвегия")!.Id, TourOperatorId = tourOperators[0].Id },
        };
        db.AddRange(routes);
        db.SaveChanges();
        return routes;
    }

    private static List<TouristGroup> SeedTouristGroups(TravelCompanyDbContext db, List<TourGuide> tourGuides, List<Route> routes)
    {
        if (db.TouristGroups.Any())
        {
            return db.TouristGroups.ToList();
        }

        var touristGroups = new List<TouristGroup>
        {
            new TouristGroup
            {
                Name = DateTime.Now.Year + " Группа 1",
                TourGuideId = tourGuides.FirstOrDefault(g => g.Person.FirstName == "Владимир")!.Id,
                RouteId = routes.FirstOrDefault(r => r.Name == "Золотое кольцо России")!.Id,
                StartDatetime = routes.FirstOrDefault(r => r.Name == "Золотое кольцо России")!.StartDatetime,
                EndDatetime = routes.FirstOrDefault(r => r.Name == "Золотое кольцо России")!.EndDatetime
            },
            new TouristGroup
            {
                Name = DateTime.Now.Year + " Группа 2",
                TourGuideId = tourGuides.FirstOrDefault(g => g.Person.FirstName == "Иван")!.Id,
                RouteId = routes.FirstOrDefault(r => r.Name == "Садовый маршрут")!.Id,
                StartDatetime = routes.FirstOrDefault(r => r.Name == "Садовый маршрут")!.StartDatetime,
                EndDatetime = routes.FirstOrDefault(r => r.Name == "Садовый маршрут")!.EndDatetime
            },
        };
        db.AddRange(touristGroups);
        db.SaveChanges();
        return touristGroups;
    }

    private static void SeedPenalties(TravelCompanyDbContext db, List<Client> clients, List<TourGuide> tourGuides)
    {
        if (db.Penalties.Any())
        {
            return;
        }

        var penalties = new List<Penalty>
        {
            new Penalty { ClientId = clients[0].Id, TourGuideId = tourGuides[0].Id, CompensationDate = DateTime.Now, Reason = "Сломанная кровать в отеле", Amount = 5700 },
        };
        db.AddRange(penalties);
        db.SaveChanges();
    }

    private static List<Payments> SeedPayments(TravelCompanyDbContext db, List<Client> clients, List<Route> routes)
    {
        if (db.Payments.Any())
        {
            return db.Payments.ToList();
        }

        var payments = new List<Payments>
        {
            new Payments { Amount = 100, RouteId = routes[0].Id, PaymentDate = DateTime.UtcNow, PaymentMethod = "Наличными", ClientId = clients[0].Id, Status = "Оплачено", Comment = "Комментарий" },
            new Payments { Amount = 300, RouteId = routes[1].Id, PaymentDate = new DateTime(2025, 4, 21), PaymentMethod = "Наличными", ClientId = clients[1].Id, Status = "Оплачено", Comment = "Комментарий" },
            new Payments { Amount = 400, RouteId = routes[2].Id, PaymentDate = new DateTime(2025, 3, 21), PaymentMethod = "Наличными", ClientId = clients[2].Id, Status = "Оплачено", Comment = "Комментарий" },
            new Payments { Amount = 200, RouteId = routes[3].Id, PaymentDate = new DateTime(2025, 5, 21), PaymentMethod = "Наличными", ClientId = clients[3].Id, Status = "Оплачено", Comment = "Комментарий" },
        };
        db.AddRange(payments);
        db.SaveChanges();
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
            new RoutesPopulatedPlace { RouteId = routes[0].Id, PopulatedPlaceId = places[0].Id, HotelId = hotels[0].Id, StayStartDatetime = DateTime.Now.AddDays(1), StayEndDatetime = DateTime.Now.AddDays(15), ExcursionProgram = "Золотое кольцо России - популярный туристический маршрут." },
            new RoutesPopulatedPlace { RouteId = routes[1].Id, PopulatedPlaceId = places[1].Id, HotelId = hotels[1].Id, StayStartDatetime = DateTime.Now.AddMonths(4), StayEndDatetime = DateTime.Now.AddMonths(5), ExcursionProgram = "Садовый маршрут вдоль побережья." },
            new RoutesPopulatedPlace { RouteId = routes[2].Id, PopulatedPlaceId = places[2].Id, HotelId = hotels[2].Id, StayStartDatetime = DateTime.Now.AddMonths(7), StayEndDatetime = DateTime.Now.AddMonths(9), ExcursionProgram = "Дорога вдоль Атлантического побережья." },
            new RoutesPopulatedPlace { RouteId = routes[3].Id, PopulatedPlaceId = places[3].Id, HotelId = hotels[3].Id, StayStartDatetime = DateTime.Now, StayEndDatetime = DateTime.Now.AddMonths(1), ExcursionProgram = "Великая океанская дорога с потрясающими видами." },
            new RoutesPopulatedPlace { RouteId = routes[4].Id, PopulatedPlaceId = places[4].Id, HotelId = hotels[4].Id, StayStartDatetime = DateTime.Now, StayEndDatetime = DateTime.Now.AddMonths(2), ExcursionProgram = "Тролльстиген - горная дорога с живописными пейзажами." },
        };
        db.AddRange(routesPopulatedPlaces);
        db.SaveChanges();
    }

    #endregion

    #region User Attraction UsersAttractions

    private static List<Attraction> SeedAttractions(TravelCompanyDbContext db)
    {
        if (db.Attractions.Any())
        {
            return db.Attractions.ToList();
        }

        var attractions = new List<Attraction>
        {
            new Attraction { Name = "Приложение Travel Company" },
            new Attraction { Name = "Каталоги" },
            new Attraction { Name = "Сотрудники" },
            new Attraction { Name = "Клиенты" },
            new Attraction { Name = "Маршруты" },
            new Attraction { Name = "Туристические группы" },
            new Attraction { Name = "Штрафы" },
        };
        db.AddRange(attractions);
        db.SaveChanges();
        return attractions;
    }

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
    }

    private static void SeedUserRights(TravelCompanyDbContext db, List<User> users, List<Attraction> attractions)
    {
        if (db.UsersAttractions.Any())
        {
            return;
        }

        var usersAttractions = new List<UsersAttraction>
        {
            // Администратор
            new UsersAttraction { UserId = users.FirstOrDefault(u => u.Username == "АДМИН")!.Id, AttractionId = attractions.FirstOrDefault(u => u.Name == "Приложение Travel Company")!.Id, CanCreate = true, CanRead = true, CanUpdate = true, CanDelete = true },
            new UsersAttraction { UserId = users.FirstOrDefault(u => u.Username == "АДМИН")!.Id, AttractionId = attractions.FirstOrDefault(u => u.Name == "Каталоги")!.Id, CanCreate = true, CanRead = true, CanUpdate = true, CanDelete = true },
            new UsersAttraction { UserId = users.FirstOrDefault(u => u.Username == "АДМИН")!.Id, AttractionId = attractions.FirstOrDefault(u => u.Name == "Сотрудники")!.Id, CanCreate = true, CanRead = true, CanUpdate = true, CanDelete = true },
            new UsersAttraction { UserId = users.FirstOrDefault(u => u.Username == "АДМИН")!.Id, AttractionId = attractions.FirstOrDefault(u => u.Name == "Клиенты")!.Id, CanCreate = true, CanRead = true, CanUpdate = true, CanDelete = true },
            new UsersAttraction { UserId = users.FirstOrDefault(u => u.Username == "АДМИН")!.Id, AttractionId = attractions.FirstOrDefault(u => u.Name == "Маршруты")!.Id, CanCreate = true, CanRead = true, CanUpdate = true, CanDelete = true },
            new UsersAttraction { UserId = users.FirstOrDefault(u => u.Username == "АДМИН")!.Id, AttractionId = attractions.FirstOrDefault(u => u.Name == "Туристические группы")!.Id, CanCreate = true, CanRead = true, CanUpdate = true, CanDelete = true },
            new UsersAttraction { UserId = users.FirstOrDefault(u => u.Username == "АДМИН")!.Id, AttractionId = attractions.FirstOrDefault(u => u.Name == "Штрафы")!.Id, CanCreate = true, CanRead = true, CanUpdate = true, CanDelete = true },

            // Сотрудник
            new UsersAttraction { UserId = users.FirstOrDefault(u => u.Username == "СОТРУДНИК")!.Id, AttractionId = attractions.FirstOrDefault(u => u.Name == "Приложение Travel Company")!.Id, CanCreate = true, CanRead = true, CanUpdate = true, CanDelete = true },
            new UsersAttraction { UserId = users.FirstOrDefault(u => u.Username == "СОТРУДНИК")!.Id, AttractionId = attractions.FirstOrDefault(u => u.Name == "Каталоги")!.Id, CanCreate = true, CanRead = true, CanUpdate = true, CanDelete = true },
            new UsersAttraction { UserId = users.FirstOrDefault(u => u.Username == "СОТРУДНИК")!.Id, AttractionId = attractions.FirstOrDefault(u => u.Name == "Сотрудники")!.Id, CanCreate = false, CanRead = true, CanUpdate = false, CanDelete = false },
            new UsersAttraction { UserId = users.FirstOrDefault(u => u.Username == "СОТРУДНИК")!.Id, AttractionId = attractions.FirstOrDefault(u => u.Name == "Клиенты")!.Id, CanCreate = true, CanRead = true, CanUpdate = true, CanDelete = true },
            new UsersAttraction { UserId = users.FirstOrDefault(u => u.Username == "СОТРУДНИК")!.Id, AttractionId = attractions.FirstOrDefault(u => u.Name == "Маршруты")!.Id, CanCreate = true, CanRead = true, CanUpdate = true, CanDelete = true },
            new UsersAttraction { UserId = users.FirstOrDefault(u => u.Username == "СОТРУДНИК")!.Id, AttractionId = attractions.FirstOrDefault(u => u.Name == "Туристические группы")!.Id, CanCreate = true, CanRead = true, CanUpdate = true, CanDelete = true },
            new UsersAttraction { UserId = users.FirstOrDefault(u => u.Username == "СОТРУДНИК")!.Id, AttractionId = attractions.FirstOrDefault(u => u.Name == "Штрафы")!.Id, CanCreate = true, CanRead = true, CanUpdate = true, CanDelete = true },
        };
        db.AddRange(usersAttractions);
        db.SaveChanges();
    }

    #endregion
}