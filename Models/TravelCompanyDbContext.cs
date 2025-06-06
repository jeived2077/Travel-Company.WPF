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

    public virtual DbSet<Person> Persons { get; set; }
    public virtual DbSet<Client> Clients { get; set; }
    public virtual DbSet<Passport> Passports { get; set; }
    public virtual DbSet<Country> Countries { get; set; }
    public virtual DbSet<Hotel> Hotels { get; set; }
    public virtual DbSet<Attraction> Attractions { get; set; }
    public virtual DbSet<Penalty> Penalties { get; set; }
    public virtual DbSet<PopulatedPlace> PopulatedPlaces { get; set; }
    public virtual DbSet<Route> Routes { get; set; }
    public virtual DbSet<RoutesPopulatedPlace> RoutesPopulatedPlaces { get; set; }
    public virtual DbSet<Street> Streets { get; set; }
    public virtual DbSet<TourGuide> TourGuides { get; set; }
    public virtual DbSet<TouristGroup> TouristGroups { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UsersAttraction> UsersAttractions { get; set; }
    public virtual DbSet<Payments> Payments { get; set; }
    public virtual DbSet<TourOperator> TourOperators { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=127.0.0.1;Port=5432;Database=travel_company;Username=postgres;Password=1234");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

            entity.HasOne(p => p.Street).WithMany(s => s.Persons)
                .HasForeignKey(p => p.StreetId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Person_Street");

            entity.HasOne(p => p.User).WithMany(u => u.Persons)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Person_User");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("client");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Photograph).HasColumnType("bytea").HasColumnName("photograph");
            entity.Property(e => e.PersonId).HasColumnName("person_id");

            entity.HasOne(c => c.Person).WithOne()
                .HasForeignKey<Client>(c => c.PersonId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Client_Person");

            entity.HasOne(c => c.Passport).WithOne()
                .HasForeignKey<Passport>(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Client_Passport");

            entity.HasMany(c => c.TouristGroups).WithMany(tg => tg.Clients)
                .UsingEntity<Dictionary<string, object>>(
                    "ClientsTouristGroups",
                    r => r.HasOne<TouristGroup>().WithMany()
                        .HasForeignKey("TouristGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_ClientsTouristGroups_TouristGroup"),
                    l => l.HasOne<Client>().WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_ClientsTouristGroups_Client"),
                    j =>
                    {
                        j.HasKey("ClientId", "TouristGroupId");
                        j.ToTable("clientstouristgroups");
                        j.IndexerProperty<long>("ClientId").HasColumnName("client_id");
                        j.IndexerProperty<long>("TouristGroupId").HasColumnName("tourist_group_id");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Password).HasMaxLength(30).HasColumnName("password");
            entity.Property(e => e.Username).HasMaxLength(30).HasColumnName("username");

            entity.HasMany(u => u.Persons)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId) // Corrected to use UserId
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Person_User");
        });

        modelBuilder.Entity<TourGuide>(entity =>
        {
            entity.ToTable("tourguide");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Salary).HasColumnType("decimal(18, 2)").HasColumnName("salary");
            entity.Property(e => e.IsFired).HasColumnName("is_fired");
            entity.Property(e => e.FiredDate).HasColumnType("date").HasColumnName("fired_date");
            entity.Property(e => e.PersonId).HasColumnName("person_id");

            entity.HasOne(tg => tg.Person).WithOne()
                .HasForeignKey<TourGuide>(tg => tg.PersonId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TourGuide_Person");

            entity.HasMany(tg => tg.TouristGroups)
                .WithOne(t => t.TourGuide)
                .HasForeignKey(t => t.TourGuideId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TouristGroup_TourGuide");

            entity.HasMany(tg => tg.Penalties)
                .WithOne(p => p.TourGuide)
                .HasForeignKey(p => p.TourGuideId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Penalty_TourGuide");
        });

        modelBuilder.Entity<Passport>(entity =>
        {
            entity.ToTable("passport");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PassportSeries).HasMaxLength(50).HasColumnName("passport_series");
            entity.Property(e => e.PassportNumber).HasMaxLength(50).HasColumnName("passport_number");
            entity.Property(e => e.PassportIssueDate).HasColumnType("date").HasColumnName("passport_issue_date");
            entity.Property(e => e.PassportIssuingAuthority).HasMaxLength(255).HasColumnName("passport_issuing_authority");
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
                .HasConstraintName("FK_Route_TourOperator");

            entity.HasMany(to => to.Hotels)
                .WithOne(h => h.TourOperator)
                .HasForeignKey(h => h.TourOperatorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Hotel_TourOperator");
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
                .HasConstraintName("FK_PopulatedPlace_Country");

            entity.HasMany(c => c.Routes)
                .WithOne(r => r.Country)
                .HasForeignKey(r => r.CountryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Route_Country");
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.ToTable("hotel");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Class).HasMaxLength(50).HasColumnName("class");
            entity.Property(e => e.Name).HasMaxLength(255).HasColumnName("name");
            entity.Property(e => e.PopulatedPlaceId).HasColumnName("populated_place_id");
            entity.Property(e => e.TourOperatorId).HasColumnName("tour_operator_id");

            entity.HasOne(h => h.PopulatedPlace)
                .WithMany(pp => pp.Hotels)
                .HasForeignKey(h => h.PopulatedPlaceId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Hotel_PopulatedPlace");

            entity.HasOne(h => h.TourOperator)
                .WithMany(to => to.Hotels)
                .HasForeignKey(h => h.TourOperatorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Hotel_TourOperator");
        });

        modelBuilder.Entity<Attraction>(entity =>
        {
            entity.ToTable("attraction");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(255).HasColumnName("name");
        });

        modelBuilder.Entity<Payments>(entity =>
        {
            entity.ToTable("payments");

            entity.Property(e => e.Id).HasColumnName("payment_id");
            entity.Property(e => e.RouteId).HasColumnName("route_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)").HasColumnName("amount");
            entity.Property(e => e.PaymentDate).HasColumnType("timestamp").HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50).HasColumnName("payment_method");
            entity.Property(e => e.Status).HasMaxLength(20).HasColumnName("status");
            entity.Property(e => e.Comment).HasMaxLength(500).HasColumnName("comment");

            entity.HasOne(d => d.Route)
                .WithMany(p => p.Payments)
                .HasForeignKey(d => d.RouteId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Payment_Route");

            entity.HasOne(d => d.Client)
                .WithMany(p => p.Payments)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Payment_Client");
        });

        modelBuilder.Entity<Penalty>(entity =>
        {
            entity.ToTable("penalty");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)").HasColumnName("amount");
            entity.Property(e => e.CompensationDate).HasColumnType("date").HasColumnName("compensation_date");
            entity.Property(e => e.Reason).HasMaxLength(500).HasColumnName("reason");
            entity.Property(e => e.TourGuideId).HasColumnName("tour_guide_id");

            entity.HasOne(d => d.Client).WithMany(p => p.Penalties)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Penalty_Client");

            entity.HasOne(d => d.TourGuide).WithMany(p => p.Penalties)
                .HasForeignKey(d => d.TourGuideId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Penalty_TourGuide");
        });

        modelBuilder.Entity<PopulatedPlace>(entity =>
        {
            entity.ToTable("populatedplace");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.Name).HasMaxLength(255).HasColumnName("name");

            entity.HasOne(d => d.Country).WithMany(p => p.PopulatedPlaces)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_PopulatedPlace_Country");
        });

        modelBuilder.Entity<Route>(entity =>
        {
            entity.ToTable("route");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cost).HasColumnType("decimal(18, 2)").HasColumnName("cost");
            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.TourOperatorId).HasColumnName("tour_operator_id");
            entity.Property(e => e.EndDatetime).HasColumnType("timestamp").HasColumnName("end_datetime");
            entity.Property(e => e.Name).HasMaxLength(255).HasColumnName("name");
            entity.Property(e => e.StartDatetime).HasColumnType("timestamp").HasColumnName("start_datetime");

            entity.HasOne(r => r.Country).WithMany(c => c.Routes)
                .HasForeignKey(r => r.CountryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Route_Country");

            entity.HasOne(r => r.TourOperator).WithMany(to => to.Routes)
                .HasForeignKey(r => r.TourOperatorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Route_TourOperator");
        });

        modelBuilder.Entity<RoutesPopulatedPlace>(entity =>
        {
            entity.ToTable("routespopulatedplace");

            entity.HasKey(e => new { e.RouteId, e.PopulatedPlaceId });

            entity.Property(e => e.RouteId).HasColumnName("route_id");
            entity.Property(e => e.PopulatedPlaceId).HasColumnName("populated_place_id");
            entity.Property(e => e.ExcursionProgram).HasMaxLength(1000).HasColumnName("excursion_program");
            entity.Property(e => e.HotelId).HasColumnName("hotel_id");
            entity.Property(e => e.StayEndDatetime).HasColumnType("timestamp").HasColumnName("stay_end_datetime");
            entity.Property(e => e.StayStartDatetime).HasColumnType("timestamp").HasColumnName("stay_start_datetime");

            entity.HasOne(d => d.Hotel).WithMany(p => p.RoutesPopulatedPlaces)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_RoutesPopulatedPlaces_Hotel");

            entity.HasOne(d => d.PopulatedPlace).WithMany(p => p.RoutesPopulatedPlaces)
                .HasForeignKey(d => d.PopulatedPlaceId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_RoutesPopulatedPlaces_PopulatedPlace");

            entity.HasOne(d => d.Route).WithMany(p => p.RoutesPopulatedPlaces)
                .HasForeignKey(d => d.RouteId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_RoutesPopulatedPlaces_Route");
        });

        modelBuilder.Entity<Street>(entity =>
        {
            entity.ToTable("street");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(255).HasColumnName("name");
        });

        modelBuilder.Entity<TouristGroup>(entity =>
        {
            entity.ToTable("touristgroup");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(255).HasColumnName("name");
            entity.Property(e => e.RouteId).HasColumnName("route_id");
            entity.Property(e => e.TourGuideId).HasColumnName("tour_guide_id");
            entity.Property(e => e.StartDatetime).HasColumnName("start_datetime").HasColumnType("timestamp");
            entity.Property(e => e.EndDatetime).HasColumnName("end_datetime").HasColumnType("timestamp"); // Explicit mapping

            entity.HasOne(tg => tg.Route).WithMany(r => r.TouristGroups)
                .HasForeignKey(tg => tg.RouteId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TouristGroup_Route");

            entity.HasOne(tg => tg.TourGuide).WithMany(t => t.TouristGroups)
                .HasForeignKey(tg => tg.TourGuideId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TouristGroup_TourGuide");
        });

        modelBuilder.Entity<UsersAttraction>(entity =>
        {
            entity.ToTable("usersattraction");

            entity.HasKey(e => new { e.UserId, e.AttractionId });

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AttractionId).HasColumnName("attraction_id");
            entity.Property(e => e.CanCreate).HasColumnName("can_create");
            entity.Property(e => e.CanDelete).HasColumnName("can_delete");
            entity.Property(e => e.CanRead).HasColumnName("can_read");
            entity.Property(e => e.CanUpdate).HasColumnName("can_update");

            entity.HasOne(d => d.Attraction).WithMany(p => p.UsersAttractions)
                .HasForeignKey(d => d.AttractionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_UsersAttractions_Attractions");

            entity.HasOne(d => d.User).WithMany(p => p.UsersAttractions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UsersAttractions_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}