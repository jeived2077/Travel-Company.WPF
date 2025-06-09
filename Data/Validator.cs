using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Travel_Company.WPF.Models;

namespace Travel_Company.WPF.Core;

public static class Validator
{
    // Validates an ICatalogItem (used in CatalogsCreateViewModel/UpdateViewModel)
    public static (bool IsValid, List<string> Errors) ValidateCatalogItem(ICatalogItem item)
    {
        var errors = new List<string>();
        if (item == null)
        {
            errors.Add("Catalog item cannot be null.");
            return (false, errors);
        }

        if (string.IsNullOrWhiteSpace(item.Name))
            errors.Add("Name is required.");

        return (errors.Count == 0, errors);
    }

    // Validates a TourGuide (used in EmployeesCreateViewModel/UpdateViewModel)
    public static (bool IsValid, List<string> Errors) ValidateTourGuide(TourGuide employee)
    {
        var errors = new List<string>();
        if (employee == null)
        {
            errors.Add("Tour guide cannot be null.");
            return (false, errors);
        }

        if (employee.Person == null)
            errors.Add("Person information is required.");

        // Validate Person properties
        if (employee.Person != null)
        {
            var person = employee.Person;
            if (string.IsNullOrWhiteSpace(person.FirstName))
                errors.Add("First name is required.");
            if (string.IsNullOrWhiteSpace(person.LastName))
                errors.Add("Last name is required.");
            if (string.IsNullOrWhiteSpace(person.Patronymic))
                errors.Add("Patronymic is required.");
            if (!person.StreetId.HasValue || person.StreetId <= 0)
                errors.Add("Valid street is required.");
            if (person.Birthdate == default)
                errors.Add("Birth date is required.");
            else if (person.Birthdate > DateTime.Now.AddYears(-18))
                errors.Add("Tour guide must be at least 18 years old.");
        }

        // Validate TourGuide properties
        if (employee.Salary <= 0)
            errors.Add("Salary must be greater than zero.");

        // If IsFired is true, FiredDate must be provided
        if (employee.IsFired && !employee.FiredDate.HasValue)
            errors.Add("Fired date is required when tour guide is marked as fired.");

        return (errors.Count == 0, errors);
    }

    // Validates a Client (used in ClientsCreateViewModel/UpdateViewModel)
    public static (bool IsValid, List<string> Errors) ValidateClient(Client client)
    {
        var errors = new List<string>();
        if (client == null)
        {
            errors.Add("Client cannot be null.");
            return (false, errors);
        }

        if (client.Person == null)
            errors.Add("Person information is required.");
        if (client.Person?.Passport == null)
            errors.Add("Passport information is required.");

        // Validate Person properties
        if (client.Person != null)
        {
            var person = client.Person;
            if (string.IsNullOrWhiteSpace(person.FirstName))
                errors.Add("First name is required.");
            if (string.IsNullOrWhiteSpace(person.LastName))
                errors.Add("Last name is required.");
            if (string.IsNullOrWhiteSpace(person.Patronymic))
                errors.Add("Patronymic is required.");
            if (!person.StreetId.HasValue || person.StreetId <= 0)
                errors.Add("Valid street is required.");
            if (person.Birthdate == default)
                errors.Add("Birth date is required.");
            else if (person.Birthdate > DateTime.Now.AddYears(-18))
                errors.Add("Client must be at least 18 years old.");
        }

        // Validate Passport properties
        if (client.Person?.Passport != null)
        {
            var passport = client.Person.Passport;
            if (string.IsNullOrWhiteSpace(passport.PassportSeries))
                errors.Add("Passport series is required.");
            if (string.IsNullOrWhiteSpace(passport.PassportNumber))
                errors.Add("Passport number is required.");
            if (string.IsNullOrWhiteSpace(passport.PassportIssuingAuthority))
                errors.Add("Passport issuing authority is required.");
            if (passport.PassportIssueDate == default)
                errors.Add("Passport issue date is required.");
            else if (passport.PassportIssueDate > DateTime.Now)
                errors.Add("Passport issue date cannot be in the future.");
        }

        return (errors.Count == 0, errors);
    }

    // Validates a Route (used in RoutesCreateViewModel/UpdateViewModel)
    public static (bool IsValid, List<string> Errors) ValidateRoute(Route route)
    {
        var errors = new List<string>();
        if (route == null)
        {
            errors.Add("Route cannot be null.");
            return (false, errors);
        }

        if (string.IsNullOrWhiteSpace(route.Name))
            errors.Add("Route name is required.");
        if (route.CountryId <= 0)
            errors.Add("Valid country is required.");
        if (route.TourOperatorId <= 0)
            errors.Add("Valid tour operator is required.");

        if (route.StartDatetime == default)
            errors.Add("Start date is required.");
        if (route.EndDatetime == default)
            errors.Add("End date is required.");
        else if (route.StartDatetime < DateTime.Now)
            errors.Add("Start date must be in the future.");
        else if (route.EndDatetime <= route.StartDatetime)
            errors.Add("End date must be after start date.");

        if (route.Cost <= 0)
            errors.Add("Cost must be greater than zero.");

        if (route.RoutesPopulatedPlaces == null || route.RoutesPopulatedPlaces.Count == 0)
            errors.Add("At least one populated place is required in the route.");

        return (errors.Count == 0, errors);
    }

    // Validates a RoutesPopulatedPlace (used in RoutesCreateViewModel/UpdateViewModel)
    public static (bool IsValid, List<string> Errors) ValidatePopulatedPlaceInRoute(RoutesPopulatedPlace place)
    {
        var errors = new List<string>();
        if (place == null)
        {
            errors.Add("Populated place in route cannot be null.");
            return (false, errors);
        }

        if (place.PopulatedPlaceId <= 0)
            errors.Add("Valid populated place is required.");
        if (!place.HotelId.HasValue || place.HotelId <= 0)
            errors.Add("Valid hotel is required.");

        if (place.StayStartDatetime == default)
            errors.Add("Stay start date is required.");
        if (place.StayEndDatetime == default)
            errors.Add("Stay end date is required.");
        else if (place.StayEndDatetime <= place.StayStartDatetime)
            errors.Add("Stay end date must be after stay start date.");

        return (errors.Count == 0, errors);
    }

    // Validates a TouristGroup (used in GroupsCreateViewModel/UpdateViewModel)
    public static (bool IsValid, List<string> Errors) ValidateTouristGroup(TouristGroup group)
    {
        var errors = new List<string>();
        if (group == null)
        {
            errors.Add("Tourist group cannot be null.");
            return (false, errors);
        }

        if (group.RouteId <= 0)
            errors.Add("Valid route is required.");
        if (group.TourGuideId <= 0)
            errors.Add("Valid tour guide is required.");

        if (group.StartDatetime == default)
            errors.Add("Start date is required.");
        if (group.EndDatetime == default)
            errors.Add("End date is required.");
        else if (group.EndDatetime <= group.StartDatetime)
            errors.Add("End date must be after start date.");

        if (group.Clients == null || group.Clients.Count == 0)
            errors.Add("At least one client is required in the tourist group.");

        return (errors.Count == 0, errors);
    }

    // Validates a TourOperator (used in TourOperatorsCreateViewModel/UpdateViewModel)
    public static (bool IsValid, List<string> Errors) ValidateTourOperator(TourOperator tourOperator)
    {
        var errors = new List<string>();
        if (tourOperator == null)
        {
            errors.Add("Tour operator cannot be null.");
            return (false, errors);
        }

        if (string.IsNullOrWhiteSpace(tourOperator.Name))
            errors.Add("Name is required.");
        if (string.IsNullOrWhiteSpace(tourOperator.ContactPhone))
            errors.Add("Contact phone is required.");
        if (string.IsNullOrWhiteSpace(tourOperator.Email))
            errors.Add("Email is required.");

        if (!string.IsNullOrWhiteSpace(tourOperator.Email))
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(tourOperator.Email))
                errors.Add("Invalid email format.");
        }

        if (!string.IsNullOrWhiteSpace(tourOperator.ContactPhone))
        {
            var phoneRegex = new Regex(@"^\+?[\d\s\-\(\)]{7,15}$");
            if (!phoneRegex.IsMatch(tourOperator.ContactPhone))
                errors.Add("Invalid phone number format.");
        }

        return (errors.Count == 0, errors);
    }

    // Validates a Penalty (used in PenaltiesCreateViewModel/UpdateViewModel)
    public static (bool IsValid, List<string> Errors) ValidatePenalty(Penalty penalty)
    {
        var errors = new List<string>();
        if (penalty == null)
        {
            errors.Add("Penalty cannot be null.");
            return (false, errors);
        }

        if (penalty.ClientId <= 0)
            errors.Add("Valid client is required.");
        if (penalty.TourGuideId <= 0)
            errors.Add("Valid tour guide is required.");

        if (penalty.CompensationDate == default)
            errors.Add("Compensation date is required.");
        else if (penalty.CompensationDate > DateTime.Now)
            errors.Add("Compensation date cannot be in the future.");

        if (penalty.Amount <= 0)
            errors.Add("Amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(penalty.Reason))
            errors.Add("Reason is required.");

        return (errors.Count == 0, errors);
    }

    // Validates a Payment (used in PaymentsCreateViewModel/UpdateViewModel)
    public static (bool IsValid, List<string> Errors) ValidatePayment(Payment payment)
    {
        var errors = new List<string>();
        if (payment == null)
        {
            errors.Add("Payment cannot be null.");
            return (false, errors);
        }

        if (payment.ClientId <= 0)
            errors.Add("Valid client is required.");
        if (payment.RouteId <= 0)
            errors.Add("Valid route is required.");

        if (payment.PaymentDate == default)
            errors.Add("Payment date is required.");
        else if (payment.PaymentDate > DateTime.Now)
            errors.Add("Payment date cannot be in the future.");

        if (payment.Amount <= 0)
            errors.Add("Payment amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(payment.Status))
            errors.Add("Payment status is required.");
        else if (!new[] { "Pending", "Completed", "Failed" }.Contains(payment.Status))
            errors.Add("Invalid payment status. Must be 'Pending', 'Completed', or 'Failed'.");

        if (string.IsNullOrWhiteSpace(payment.PaymentMethod))
            errors.Add("Payment method is required.");

        return (errors.Count == 0, errors);
    }
}