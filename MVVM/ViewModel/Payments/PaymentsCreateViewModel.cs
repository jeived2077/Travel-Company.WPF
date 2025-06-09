using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Resources.Localizations;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.Payments;

public class PaymentsCreateViewModel : Core.ViewModel
{
    private readonly IRepository<Payment, long> _paymentsRepository;
    private readonly IRepository<TourGuide, long> _employeesRepository;
    private readonly IRepository<Client, long> _clientsRepository;
    private readonly IRepository<Route, long> _routesRepository;

    private INavigationService _navigation = null!;
    public INavigationService Navigation
    {
        get => _navigation;
        set
        {
            _navigation = value;
            OnPropertyChanged();
        }
    }

    private Payment _payment = new();
    public Payment Payment
    {
        get => _payment;
        set
        {
            _payment = value;
            OnPropertyChanged();
        }
    }

    private List<Route> _routes = null!;
    public List<Route> Routes
    {
        get => _routes;
        set
        {
            _routes = value;
            OnPropertyChanged();
        }
    }

    private List<Client> _clients = null!;
    public List<Client> Clients
    {
        get => _clients;
        set
        {
            _clients = value;
            OnPropertyChanged();
        }
    }

    private List<TourGuide> _employees = null!;
    public List<TourGuide> Employees
    {
        get => _employees;
        set
        {
            _employees = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand CreateCommand { get; set; }
    public RelayCommand CancelCommand { get; set; }

    public PaymentsCreateViewModel(
        IRepository<Payment, long> paymentsRepo,
        IRepository<TourGuide, long> employeesRepo,
        IRepository<Client, long> clientsRepo,
        IRepository<Route, long> routeRepo,
        INavigationService navigationService)
    {
        _paymentsRepository = paymentsRepo ?? throw new ArgumentNullException(nameof(paymentsRepo));
        _routesRepository = routeRepo ?? throw new ArgumentNullException(nameof(routeRepo));
        _clientsRepository = clientsRepo ?? throw new ArgumentNullException(nameof(clientsRepo));
        _employeesRepository = employeesRepo ?? throw new ArgumentNullException(nameof(employeesRepo));
        Navigation = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        InitializeData();

        CreateCommand = new RelayCommand(
            execute: _ => HandleCreating(),
            canExecute: _ => Payment.ClientId > 0 && Payment.RouteId > 0 && !string.IsNullOrEmpty(Payment.PaymentMethod) && Payment.Amount > 0);
        CancelCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<PaymentsViewModel>(),
            canExecute: _ => true);
    }

    private void InitializeData()
    {
        // Загрузка клиентов с исключением сотрудников
        var employeePersonIds = _employeesRepository.GetQuaryable()
            .Select(tg => tg.PersonId)
            .ToList();
        Clients = _clientsRepository.GetQuaryable()
            .Include(c => c.Person)
            .AsNoTracking()
            .Where(c => !employeePersonIds.Contains(c.PersonId))
            .ToList() ?? new List<Client>();
        System.Diagnostics.Debug.WriteLine($"Loaded {Clients.Count} clients. First client: {Clients.FirstOrDefault()?.Person?.FullName ?? "None"}");

        // Загрузка маршрутов
        Routes = _routesRepository.GetQuaryable()
            .Include(r => r.Country)
            .AsNoTracking()
            .ToList() ?? new List<Route>();
        System.Diagnostics.Debug.WriteLine($"Loaded {Routes.Count} routes");

        // Загрузка сотрудников (только для отладки, не используется в UI)
        Employees = _employeesRepository.GetQuaryable()
            .Include(tg => tg.Person)
            .AsNoTracking()
            .ToList() ?? new List<TourGuide>();
        System.Diagnostics.Debug.WriteLine($"Loaded {Employees.Count} employees");

        Payment.PaymentDate = DateTime.Now;
    }

    private void HandleCreating()
    {
        // Маппинг русских статусов на английские для валидатора
        var statusMap = new Dictionary<string, string>
        {
            { "Ожидает оплаты", "Pending" },
            { "Оплачен", "Completed" },
            { "Отменен", "Failed" },
            { "Частичный возврат", "Pending" }, // Можно настроить по необходимости
            { "Возвращен", "Failed" }
        };

        if (!string.IsNullOrEmpty(Payment.Status) && statusMap.ContainsKey(Payment.Status))
        {
            Payment.Status = statusMap[Payment.Status];
        }
        else if (string.IsNullOrEmpty(Payment.Status))
        {
            Payment.Status = "Pending"; // Значение по умолчанию
        }

        var (isValid, errors) = Validator.ValidatePayment(Payment);
        if (!isValid)
        {
            MessageBox.Show(
                $"Validation failed:\n{string.Join("\n", errors)}",
                LocalizedStrings.Instance["InputErrorMessageBoxTitle"],
                MessageBoxButton.OK, MessageBoxImage.Error);
            System.Diagnostics.Debug.WriteLine($"Validation errors: {string.Join(", ", errors)}");
            return;
        }

        try
        {
            if (string.IsNullOrEmpty(Payment.PaymentMethod))
                Payment.PaymentMethod = "Ожидает оплаты";

            _paymentsRepository.Insert(Payment);
            _paymentsRepository.SaveChanges();

            MessageBox.Show("Payment created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Navigation.NavigateTo<PaymentsViewModel>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error creating payment: {ex.Message}", "Create Error", MessageBoxButton.OK, MessageBoxImage.Error);
            System.Diagnostics.Debug.WriteLine($"Exception during save: {ex}");
        }
    }
}