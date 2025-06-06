using System;
using System.Text.RegularExpressions;
using Travel_Company.WPF.Models;

public static class Validator
{
    // Validates an ICatalogItem (used in CatalogsCreateViewModel)
    public static bool ValidateCatalogItem(ICatalogItem item)
    {
        if (item == null)
            return false;

        return !string.IsNullOrWhiteSpace(item.Name);
    }

    // Validates a TourGuide (used in EmployeesCreateViewModel)
    public static bool ValidateTourGuide(TourGuide employee)
    {
        if (employee == null || employee.Person == null)
            return false;

        // Validate Person properties
        var person = employee.Person;
        if (string.IsNullOrWhiteSpace(person.FirstName) ||
            string.IsNullOrWhiteSpace(person.LastName) ||
            string.IsNullOrWhiteSpace(person.Patronymic) ||
            !person.StreetId.HasValue || // StreetId is nullable (long?)
            person.Birthdate == default || // Updated to use default for DateTime check
            person.Birthdate > DateTime.Now.AddYears(-18)) // Ensure age >= 18
        {
            return false;
        }

        // Validate TourGuide properties
        if (employee.Salary <= 0)
            return false;

        // If IsFired is true, FiredDate must be provided
        if (employee.IsFired && !employee.FiredDate.HasValue)
            return false;

        return true;
    }

    // Validates a Client (for use in a potential ClientsCreateViewModel)
    public static bool ValidateClient(Client client)
    {
        if (client == null || client.Person == null || client.Passport == null)
            return false;

        // Validate Person properties
        var person = client.Person;
        if (string.IsNullOrWhiteSpace(person.FirstName) ||
            string.IsNullOrWhiteSpace(person.LastName) ||
            string.IsNullOrWhiteSpace(person.Patronymic) ||
            !person.StreetId.HasValue || // StreetId is nullable (long?)
            person.Birthdate == default || // Updated to use default for DateTime check
            person.Birthdate > DateTime.Now.AddYears(-18)) // Ensure age >= 18
        {
            return false;
        }

        // Validate Passport properties
        var passport = client.Passport;
        if (string.IsNullOrWhiteSpace(passport.PassportSeries) ||
            string.IsNullOrWhiteSpace(passport.PassportNumber) ||
            string.IsNullOrWhiteSpace(passport.PassportIssuingAuthority) ||
            passport.PassportIssueDate == default ||
            passport.PassportIssueDate > DateTime.Now)
        {
            return false;
        }

        return true;
    }

    // Validates a Route (used in RoutesCreateViewModel)
    public static bool ValidateRoute(Route route)
    {
        if (route == null)
            return false;

        // Check required fields
        if (string.IsNullOrWhiteSpace(route.Name) ||
            route.CountryId == 0 || // Check for default value
            route.TourOperatorId == 0) // Check for default value (nullable, but should be set)
        {
            return false;
        }

        // Validate date range
        if (route.StartDatetime == default || route.EndDatetime == default ||
            route.StartDatetime < DateTime.Now || // Start date should be in the future
            route.EndDatetime <= route.StartDatetime) // End date must be after start date
        {
            return false;
        }

        // Validate price (using Cost instead of Price)
        if (route.Cost <= 0)
            return false;

        // Ensure at least one place in the route (via RoutesPopulatedPlaces)
        if (route.RoutesPopulatedPlaces == null || route.RoutesPopulatedPlaces.Count == 0)
            return false;

        return true;
    }

    // Validates a RoutesPopulatedPlace (used in RoutesCreateViewModel)
    public static bool ValidatePopulatedPlaceInRoute(RoutesPopulatedPlace place)
    {
        if (place == null)
            return false;

        // Check required fields
        if (place.PopulatedPlaceId == 0 || // Check for default value
            !place.HotelId.HasValue) // HotelId is nullable, ensure it's set
        {
            return false;
        }

        // Validate date range
        if (place.StayStartDatetime == default || place.StayEndDatetime == default ||
            place.StayEndDatetime <= place.StayStartDatetime) // End date must be after start date
        {
            return false;
        }

        return true;
    }

    public static bool ValidateTouristGroup(TouristGroup group)
    {
        if (group == null)
            return false;

        if (group.RouteId == 0 || group.TourGuideId == 0)
            return false;

        if (group.StartDatetime == default || group.EndDatetime == default ||
            group.EndDatetime <= group.StartDatetime)
            return false;

        if (group.Clients == null || group.Clients.Count == 0)
            return false;

        return true;
    }

    public static bool ValidateTourOperator(TourOperator tourOperator)
    {
        if (tourOperator == null)
            return false;

        // Check required fields
        if (string.IsNullOrWhiteSpace(tourOperator.Name) ||
            string.IsNullOrWhiteSpace(tourOperator.ContactPhone) ||
            string.IsNullOrWhiteSpace(tourOperator.Email))
        {
            return false;
        }

        // Validate email format (basic regex)
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!emailRegex.IsMatch(tourOperator.Email))
            return false;

        // Validate phone format (basic regex: digits, optional spaces, dashes, or parentheses)
        var phoneRegex = new Regex(@"^\+?[\d\s\-\(\)]{7,15}$");
        if (!phoneRegex.IsMatch(tourOperator.ContactPhone))
            return false;

        return true;
    }

    public static bool ValidatePenalty(Penalty penalty)
    {
        if (penalty == null)
            return false;

        // Check required fields
        if (penalty.ClientId == 0 || penalty.TourGuideId == 0)
            return false;

        // Validate CompensationDate
        if (penalty.CompensationDate == default || penalty.CompensationDate > DateTime.Now)
            return false;

        // Validate Amount
        if (penalty.Amount <= 0)
            return false;

        // Validate Reason
        if (string.IsNullOrWhiteSpace(penalty.Reason))
            return false;

        return true;
    }
}