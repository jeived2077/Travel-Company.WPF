using Travel_Company.WPF.Core.Enums;
using Travel_Company.WPF.Models;

namespace Travel_Company.WPF.Data.Dto;

public class CatalogItemMessage
{
    public ICatalogItem CatalogItem { get; set; } = null!;
    public CatalogType CatalogType { get; set; }
}