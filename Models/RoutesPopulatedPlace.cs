using System;

namespace Travel_Company.WPF.Models;

public partial class RoutesPopulatedPlace
{
    public long RouteId { get; set; }
    public long PopulatedPlaceId { get; set; }
    public string ExcursionProgram { get; set; } = null!;
    public long? HotelId { get; set; }
    public DateTime? StayEndDatetime { get; set; }
    public DateTime? StayStartDatetime { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;
    public virtual PopulatedPlace PopulatedPlace { get; set; } = null!;
    public virtual Route Route { get; set; } = null!;
}
