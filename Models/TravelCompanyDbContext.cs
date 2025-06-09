using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Travel_Company.WPF.Models;

namespace Travel_Company.WPF.Data;

public partial class TravelCompanyDbContext : DbContext
{
    public TravelCompanyDbContext()
    {
    }

    public TravelCompanyDbContext(DbContextOptions<TravelCompanyDbContext> options)
        : base(options)
    {
    }

    public DbSet<Admin> Admins { get; set; }
    public virtual DbSet<Person> Persons { get; set; }
    public virtual DbSet<Client> Clients { get; set; }
    public virtual DbSet<Passport> Passports { get; set; }
    public virtual DbSet<Country> Countries { get; set; }
    public virtual DbSet<Hotel> Hotels { get; set; }
    public virtual DbSet<Penalty> Penalties { get; set; }
    public virtual DbSet<PopulatedPlace> PopulatedPlaces { get; set; }
    public virtual DbSet<Route> Routes { get; set; }
    public virtual DbSet<RoutesPopulatedPlace> RoutesPopulatedPlaces { get; set; }
    public virtual DbSet<Street> Streets { get; set; }
    public virtual DbSet<TourGuide> TourGuides { get; set; }
    public virtual DbSet<TouristGroup> TouristGroups { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<TourOperator> TourOperators { get; set; }
    public virtual DbSet<Employee> Employees { get; set; }
    public virtual DbSet<EmployeeTouristGroup> EmployeeTouristGroups { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=127.0.0.1;Port=5432;Database=travel_company;Username=postgres;Password=1234")
                          .EnableSensitiveDataLogging()
                          .EnableDetailedErrors();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.ToTable("admin");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PersonId).HasColumnName("person_id");
            entity.Property(e => e.Username).HasMaxLength(30).HasColumnName("username");
            entity.Property(e => e.Password).HasMaxLength(255).HasColumnName("password");

            entity.HasOne(a => a.Person)
                  .WithOne(p => p.Admin)
                  .HasForeignKey<Admin>(a => a.PersonId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_admin_person");

            entity.HasIndex(a => a.Username).IsUnique();
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.ToTable("person");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FirstName).HasMaxLength(255).HasColumnName("first_name");
            entity.Property(e => e.LastName).HasMaxLength(255).HasColumnName("last_name");
            entity.Property(e => e.Patronymic).HasMaxLength(255).HasColumnName("patronymic");
            entity.Property(e => e.Birthdate).HasColumnType("date").HasColumnName("birthdate");
            entity.Property(e => e.StreetId).HasColumnName("street_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Photograph).HasColumnType("bytea").HasColumnName("photograph");

            entity.HasOne(p => p.Street)
                  .WithMany(s => s.Persons)
                  .HasForeignKey(p => p.StreetId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("fk_person_street");

            entity.HasOne(p => p.User)
                  .WithMany(u => u.Persons)
                  .HasForeignKey(p => p.UserId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("fk_person_user");

            entity.HasOne(p => p.Passport)
                  .WithOne(p => p.Person)
                  .HasForeignKey<Passport>(p => p.PersonId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_passport_person");

            entity.HasOne(p => p.Client)
                  .WithOne(c => c.Person)
                  .HasForeignKey<Client>(c => c.PersonId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_client_person");

            entity.HasOne(p => p.Employee)
                  .WithOne(e => e.Person)
                  .HasForeignKey<Employee>(e => e.PersonId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_employees_person");

            entity.HasOne(p => p.TourGuide)
                  .WithOne(tg => tg.Person)
                  .HasForeignKey<TourGuide>(tg => tg.PersonId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_tourguide_person");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("client");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PersonId).HasColumnName("person_id");
            entity.Property(e => e.Photograph).HasColumnType("bytea").HasColumnName("photograph");

            entity.HasOne(c => c.Person)
                  .WithOne(p => p.Client)
                  .HasForeignKey<Client>(c => c.PersonId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_client_person");

            entity.HasMany(c => c.TouristGroups)
                  .WithMany(tg => tg.Clients)
                  .UsingEntity<Dictionary<string, object>>(
                      "clientstouristgroups",
                      r => r.HasOne<TouristGroup>().WithMany()
                          .HasForeignKey("tourist_group_id")
                          .OnDelete(DeleteBehavior.Cascade)
                          .HasConstraintName("fk_clientstouristgroups_touristgroup"),
                      l => l.HasOne<Client>().WithMany()
                          .HasForeignKey("client_id")
                          .OnDelete(DeleteBehavior.Cascade)
                          .HasConstraintName("fk_clientstouristgroups_client"),
                      j =>
                      {
                          j.HasKey("client_id", "tourist_group_id");
                          j.ToTable("clientstouristgroups");
                          j.IndexerProperty<long>("client_id").HasColumnName("client_id");
                          j.IndexerProperty<long>("tourist_group_id").HasColumnName("tourist_group_id");
                      });

            entity.HasMany(c => c.Payments)
                  .WithOne(p => p.Client)
                  .HasForeignKey(p => p.ClientId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_payment_client");

            entity.HasMany(c => c.Penalties)
                  .WithOne(p => p.Client)
                  .HasForeignKey(p => p.ClientId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_penalty_client");
        });

        modelBuilder.Entity<Passport>(entity =>
        {
            entity.ToTable("passport");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PersonId).HasColumnName("person_id");
            entity.Property(e => e.PassportSeries).HasMaxLength(10).HasColumnName("passport_series");
            entity.Property(e => e.PassportNumber).HasMaxLength(20).HasColumnName("passport_number");
            entity.Property(e => e.PassportIssueDate).HasColumnType("date").HasColumnName("passport_issue_date");
            entity.Property(e => e.PassportIssuingAuthority).HasMaxLength(255).HasColumnName("passport_issuing_authority");

            entity.HasOne(p => p.Person)
                  .WithOne(p => p.Passport)
                  .HasForeignKey<Passport>(p => p.PersonId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_passport_person");
        });

        modelBuilder.Entity<TourGuide>(entity =>
        {
            entity.ToTable("tourguide");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PersonId).HasColumnName("person_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Salary).HasColumnType("numeric(18, 2)").HasColumnName("salary");
            entity.Property(e => e.IsFired).HasColumnName("is_fired");
            entity.Property(e => e.FiredDate).HasColumnType("date").HasColumnName("fired_date");

            entity.HasOne(tg => tg.Person)
                  .WithOne(p => p.TourGuide)
                  .HasForeignKey<TourGuide>(tg => tg.PersonId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_tourguide_person");

            entity.HasOne(tg => tg.Employee)
                  .WithMany(e => e.TourGuides)
                  .HasForeignKey(tg => tg.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_tourguide_employee");

            entity.HasMany(tg => tg.TouristGroups)
                  .WithOne(t => t.TourGuide)
                  .HasForeignKey(t => t.TourGuideId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_touristgroup_tourguide");

            entity.HasMany(tg => tg.Penalties)
                  .WithOne(p => p.TourGuide)
                  .HasForeignKey(p => p.TourGuideId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_penalty_tourguide");
        });

        modelBuilder.Entity<TouristGroup>(entity =>
        {
            entity.ToTable("touristgroup");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(255).HasColumnName("name");
            entity.Property(e => e.TourGuideId).HasColumnName("tour_guide_id");
            entity.Property(e => e.RouteId).HasColumnName("route_id");
            entity.Property(e => e.StartDatetime).HasColumnType("timestamp").HasColumnName("start_datetime");
            entity.Property(e => e.EndDatetime).HasColumnType("timestamp").HasColumnName("end_datetime");

            entity.HasOne(tg => tg.Route)
                  .WithMany(r => r.TouristGroups)
                  .HasForeignKey(tg => tg.RouteId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_touristgroup_route");

            entity.HasOne(tg => tg.TourGuide)
                  .WithMany(t => t.TouristGroups)
                  .HasForeignKey(tg => tg.TourGuideId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_touristgroup_tourguide");

            entity.HasMany(tg => tg.EmployeeTouristGroups)
                  .WithOne(etg => etg.TouristGroup)
                  .HasForeignKey(etg => etg.TouristGroupId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_employee_touristgroup_touristgroup");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("employees");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PersonId).HasColumnName("person_id");
            entity.Property(e => e.Photograph).HasColumnType("bytea").HasColumnName("photograph");
            entity.Property(e => e.IsDismissed).HasColumnName("is_dismissed").HasDefaultValue(false);
            entity.Property(e => e.DismissalDate).HasColumnType("date").HasColumnName("dismissal_date");

            entity.HasOne(e => e.Person)
                  .WithOne(p => p.Employee)
                  .HasForeignKey<Employee>(e => e.PersonId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_employees_person");

            entity.HasMany(e => e.EmployeeTouristGroups)
                  .WithOne(emp => emp.Employee)
                  .HasForeignKey(etg => etg.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_employee_touristgroup_employee");

            entity.HasMany(e => e.TourGuides)
                  .WithOne(tg => tg.Employee)
                  .HasForeignKey(tg => tg.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_tourguide_employee");
        });

        modelBuilder.Entity<EmployeeTouristGroup>(entity =>
        {
            entity.ToTable("employee_touristgroup");

            entity.HasKey(e => new { e.EmployeeId, e.TouristGroupId });

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.TouristGroupId).HasColumnName("tourist_group_id");
            entity.Property(e => e.Role).HasMaxLength(50).HasColumnName("role").HasDefaultValue("Assistant");

            entity.HasOne(etg => etg.Employee)
                  .WithMany(e => e.EmployeeTouristGroups)
                  .HasForeignKey(etg => etg.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_employee_touristgroup_employee");

            entity.HasOne(etg => etg.TouristGroup)
                  .WithMany(tg => tg.EmployeeTouristGroups)
                  .HasForeignKey(etg => etg.TouristGroupId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_employee_touristgroup_touristgroup");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Username).HasMaxLength(30).HasColumnName("username");
            entity.Property(e => e.Password).HasMaxLength(255).HasColumnName("password");

            entity.HasMany(u => u.Persons)
                  .WithOne(p => p.User)
                  .HasForeignKey(p => p.UserId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("fk_person_user");
        });

        

        

        modelBuilder.Entity<Route>(entity =>
        {
            entity.ToTable("route");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(255).HasColumnName("name");
            entity.Property(e => e.Cost).HasColumnType("numeric(18,2)").HasColumnName("cost");
            entity.Property(e => e.StartDatetime).HasColumnType("timestamp").HasColumnName("start_datetime");
            entity.Property(e => e.EndDatetime).HasColumnType("timestamp").HasColumnName("end_datetime");
            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.TourOperatorId).HasColumnName("tour_operator_id");

            entity.HasOne(r => r.Country)
                  .WithMany(c => c.Routes)
                  .HasForeignKey(r => r.CountryId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("fk_route_country");

            entity.HasOne(r => r.TourOperator)
                  .WithMany(to => to.Routes)
                  .HasForeignKey(r => r.TourOperatorId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("fk_route_tour_operator");

            entity.HasMany(r => r.TouristGroups)
                  .WithOne(tg => tg.Route)
                  .HasForeignKey(tg => tg.RouteId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_touristgroup_route");

            entity.HasMany(r => r.Payments)
                  .WithOne(p => p.Route)
                  .HasForeignKey(p => p.RouteId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("fk_payment_route");

            entity.HasMany(r => r.RoutesPopulatedPlaces)
                  .WithOne(rpp => rpp.Route)
                  .HasForeignKey(rpp => rpp.RouteId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_routespopulatedplace_route");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payment");

            entity.Property(e => e.Id).HasColumnName("payment_id");
            entity.Property(e => e.RouteId).HasColumnName("route_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.Amount).HasColumnType("numeric(18,2)").HasColumnName("amount");
            entity.Property(e => e.PaymentDate).HasColumnType("timestamp").HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50).HasColumnName("payment_method");
            entity.Property(e => e.Status).HasMaxLength(20).HasColumnName("status");
            entity.Property(e => e.Comment).HasMaxLength(500).HasColumnName("comment");

            entity.HasOne(p => p.Route)
                  .WithMany(r => r.Payments)
                  .HasForeignKey(p => p.RouteId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("fk_payment_route");

            entity.HasOne(p => p.Client)
                  .WithMany(c => c.Payments)
                  .HasForeignKey(p => p.ClientId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_payment_client");
        });

        modelBuilder.Entity<Penalty>(entity =>
        {
            entity.ToTable("penalty");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.TourGuideId).HasColumnName("tour_guide_id");
            entity.Property(e => e.Amount).HasColumnType("numeric(18,2)").HasColumnName("amount");
            entity.Property(e => e.CompensationDate).HasColumnType("date").HasColumnName("compensation_date");
            entity.Property(e => e.Reason).HasMaxLength(500).HasColumnName("reason");

            entity.HasOne(p => p.Client)
                  .WithMany(c => c.Penalties)
                  .HasForeignKey(p => p.ClientId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_penalty_client");

            entity.HasOne(p => p.TourGuide)
                  .WithMany(tg => tg.Penalties)
                  .HasForeignKey(p => p.TourGuideId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_penalty_tourguide");
        });

        modelBuilder.Entity<PopulatedPlace>(entity =>
        {
            entity.ToTable("populatedplace");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(255).HasColumnName("name");
            entity.Property(e => e.CountryId).HasColumnName("country_id");

            entity.HasOne(pp => pp.Country)
                  .WithMany(c => c.PopulatedPlaces)
                  .HasForeignKey(pp => pp.CountryId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("fk_populatedplace_country");

            entity.HasMany(pp => pp.Hotels)
                  .WithOne(h => h.PopulatedPlace)
                  .HasForeignKey(h => h.PopulatedPlaceId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("fk_hotel_populated_place");

            entity.HasMany(pp => pp.RoutesPopulatedPlaces)
                  .WithOne(rpp => rpp.PopulatedPlace)
                  .HasForeignKey(rpp => rpp.PopulatedPlaceId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("fk_routespopulatedplace_populated_place");
        });

        modelBuilder.Entity<RoutesPopulatedPlace>(entity =>
        {
            entity.ToTable("routespopulatedplace");

            entity.HasKey(e => new { e.RouteId, e.PopulatedPlaceId });

            entity.Property(e => e.RouteId).HasColumnName("route_id");
            entity.Property(e => e.PopulatedPlaceId).HasColumnName("populated_place_id");
            entity.Property(e => e.HotelId).HasColumnName("hotel_id");
            entity.Property(e => e.StayStartDatetime).HasColumnType("timestamp").HasColumnName("stay_start_datetime");
            entity.Property(e => e.StayEndDatetime).HasColumnType("timestamp").HasColumnName("stay_end_datetime");
            entity.Property(e => e.ExcursionProgram).HasMaxLength(1000).HasColumnName("excursion_program");

            entity.HasOne(rpp => rpp.Route)
                  .WithMany(r => r.RoutesPopulatedPlaces)
                  .HasForeignKey(rpp => rpp.RouteId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_routespopulatedplace_route");

            entity.HasOne(rpp => rpp.PopulatedPlace)
                  .WithMany(pp => pp.RoutesPopulatedPlaces)
                  .HasForeignKey(rpp => rpp.PopulatedPlaceId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("fk_routespopulatedplace_populatedplace");

            entity.HasOne(rpp => rpp.Hotel)
                  .WithMany(h => h.RoutesPopulatedPlaces)
                  .HasForeignKey(rpp => rpp.HotelId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("fk_routespopulatedplace_hotel");
        });

        modelBuilder.Entity<Street>(entity =>
        {
            entity.ToTable("street");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(255).HasColumnName("name");
            entity.HasMany(s => s.Persons)
                  .WithOne(p => p.Street)
                  .HasForeignKey(p => p.StreetId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("fk_person_street");
        });

        modelBuilder.Entity<TourOperator>(entity =>
        {
            entity.ToTable("touroperator");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(255).HasColumnName("name");
            entity.Property(e => e.ContactPhone).HasMaxLength(50).HasColumnName("contact_phone");
            entity.Property(e => e.Email).HasMaxLength(255).HasColumnName("email");
            entity.Property(e => e.Address).HasMaxLength(500).HasColumnName("address");

            entity.HasMany(to => to.Routes)
                  .WithOne(r => r.TourOperator)
                  .HasForeignKey(r => r.TourOperatorId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("fk_route_tour_operator");

            entity.HasMany(to => to.Hotels)
                  .WithOne(h => h.TourOperator)
                  .HasForeignKey(h => h.TourOperatorId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("fk_hotel_tourOperator");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("country");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(255).HasColumnName("name");

            entity.HasMany(c => c.PopulatedPlaces)
                  .WithOne(pp => pp.Country)
                  .HasForeignKey(pp => pp.CountryId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("fk_populatedplace_country");

            entity.HasMany(c => c.Routes)
                  .WithOne(r => r.Country)
                  .HasForeignKey(r => r.CountryId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("fk_route_country");
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.ToTable("hotel");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(255).HasColumnName("name");
            entity.Property(e => e.Class).HasMaxLength(50).HasColumnName("class");
            entity.Property(e => e.PopulatedPlaceId).HasColumnName("populated_place_id");
            entity.Property(e => e.TourOperatorId).HasColumnName("tour_operator_id");

            entity.HasOne(h => h.PopulatedPlace)
                  .WithMany(pp => pp.Hotels)
                  .HasForeignKey(h => h.PopulatedPlaceId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("fk_hotel_populated_place");

            entity.HasOne(h => h.TourOperator)
                  .WithMany(to => to.Hotels)
                  .HasForeignKey(h => h.TourOperatorId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("fk_hotel_tourOperator");

            entity.HasMany(h => h.RoutesPopulatedPlaces)
                  .WithOne(rpp => rpp.Hotel)
                  .HasForeignKey(rpp => rpp.HotelId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("fk_routespopulatedplace_hotel");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}