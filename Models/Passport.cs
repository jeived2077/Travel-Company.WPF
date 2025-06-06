using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel_Company.WPF.Models;

public partial class Passport
{
    public long Id { get; set; } // Matches the foreign key to client.id
    public string PassportSeries { get; set; } = null!;
    public string PassportNumber { get; set; } = null!;
    public DateTime PassportIssueDate { get; set; }
    public string PassportIssuingAuthority { get; set; } = null!;
    [NotMapped]
    public string FullPassportNumber => $"{PassportSeries} {PassportNumber}".Trim(); // Combines series and number
}
