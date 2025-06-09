using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Company.WPF.Models
{
    public class Passport
    {
        public long Id { get; set; }
        public long PersonId { get; set; } // Связь с Person
        public string PassportSeries { get; set; } = null!;
        public string PassportNumber { get; set; } = null!;
        public DateTime PassportIssueDate { get; set; }
        public string PassportIssuingAuthority { get; set; } = null!;

        [ForeignKey("PersonId")]
        public virtual Person Person { get; set; } = null!;

        // Вычисляемое свойство для полного номера паспорта
        public string FullPassportNumber => $"{PassportSeries} {PassportNumber}";
    }
}