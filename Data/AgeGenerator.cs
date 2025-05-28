using System;

namespace Travel_Company.WPF.Data;

public static class AgeGenerator
{
    /// <summary>
    /// Generate random birthdate.
    /// </summary>
    /// <returns>Only date part without time part.</returns>
    public static DateTime GetRandomDate()
    {
        var to = DateTime.Today.AddYears(-20);
        var from = DateTime.Today.AddYears(-65);

        var range = to - from;
        var randomValue = new Random().NextDouble();
        var randomDate = from + TimeSpan.FromDays(range.TotalDays * randomValue);

        return randomDate.Date;
    }
}
