namespace Travel_Company.WPF.Models;

public interface ICatalogItem
{
    long Id { get; set; }
    string Name { get; } // Changed to read-only

}