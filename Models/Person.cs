using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Company.WPF.Models
{
    public partial class Person
    {
        public long Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Patronymic { get; set; } = null!;
        public DateTime Birthdate { get; set; }
        public long? StreetId { get; set; }
        public long? UserId { get; set; }
        public byte[]? Photograph { get; set; }

        [ForeignKey("StreetId")]
        public virtual Street? Street { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public virtual Passport? Passport { get; set; }
        public virtual Client? Client { get; set; }
        public virtual Employee? Employee { get; set; }
        public virtual TourGuide? TourGuide { get; set; }
        public virtual Admin? Admin { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName} {Patronymic}".Trim();
    }
}