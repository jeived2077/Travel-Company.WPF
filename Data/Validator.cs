using Travel_Company.WPF.Models;

namespace Travel_Company.WPF.Data;

public static class Validator
{
    public static bool ValidateCatalogItem(ICatalogItem catalogItem)
    {
        if (catalogItem is null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(catalogItem.Name))
        {
            return false;
        }

        if (catalogItem is Hotel hotel && string.IsNullOrWhiteSpace(hotel.Class))
        {
            return false;
        }

        if (catalogItem is PopulatedPlace populatedPlace && populatedPlace.Country is null)
        {
             return false;
        }

        return true;
    }

    public static bool ValidateTouristGroup(TouristGroup group)
    {
        if (group is null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(group.Name))
        {
            return false;
        }

        if (group.Route is null || group.TourGuide is null)
        {
            return false;
        }

        return true;
    }

    public static bool ValidatePenalty(Penalty penalty)
    {
        if (penalty is null)
        {
            return false;
        }

        if (penalty.CompensationAmount <= 0)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(penalty.CompensationDescription))
        {
            return false;
        }

        if (penalty.CompensationDate == default)
        {
            return false;
        }

        if (penalty.Client is null || penalty.TourGuide is null)
        {
            return false;
        }

        return true;
    }

    public static bool ValidateTourOperator(TourOperator tourOperator)
    {
        if (tourOperator is null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(tourOperator.Name))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(tourOperator.Address))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(tourOperator.ContactInfo))
        {
            return false;
        }

        return true;
    }

    public static bool ValidateRoute(Route route)
    {
        if (route is null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(route.Name))
        {
            return false;
        }

        if (route.StartDatetime == default || route.EndDatetime == default)
        {
            return false;
        }

        if (route.StartDatetime > route.EndDatetime)
        {
            return false;
        }

        if (route.Cost <= 0)
        {
            return false;
        }

        if (route.Country is null)
        {
            return false;
        }

        if (route.RoutesPopulatedPlaces is null || route.RoutesPopulatedPlaces.Count == 0)
        {
            return false;
        }

        return true;
    }

    public static bool ValidatePopulatedPlaceInRoute(RoutesPopulatedPlace place)
    {
        if (place is null)
        {
            return false;
        }

        if (place.StayStartDatetime == default || place.StayEndDatetime == default)
        {
            return false;
        }

        if (place.StayStartDatetime > place.StayEndDatetime)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(place.ExcursionProgram))
        {
            return false;
        }

        if (place.Hotel is null || place.PopulatedPlace is null)
        {
            return false;
        }

        return true;
    }

    public static bool ValidateTourGuide(TourGuide tourGuide)
    {
        if (tourGuide is null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(tourGuide.FirstName) || string.IsNullOrWhiteSpace(tourGuide.LastName))
        {
            return false;
        }

        if (tourGuide.Birthdate == default)
        {
            return false;
        }

        if (tourGuide.Salary <= 0)
        {
            return false;
        }

        if (tourGuide.IsFired && tourGuide.FiredDate == default)
        {
            return false;
        }

        if (tourGuide.Street is null)
        {
            return false;
        }

        return true;
    }

    public static bool ValidateClient(Client client)
    {
        if (client is null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(client.FirstName) || string.IsNullOrWhiteSpace(client.LastName))
        {
            return false;
        }

        if (client.Birthdate == default)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(client.PassportSeries) || string.IsNullOrWhiteSpace(client.PassportNumber))
        {
            return false;
        }

        if (client.PassportIssueDate == default)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(client.PassportIssuingAuthority))
        {
            return false;
        }

        if (client.Street is null)
        {
            return false;
        }

        return true;
    }

}