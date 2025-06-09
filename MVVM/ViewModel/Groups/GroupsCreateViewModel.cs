using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Resources.Localizations;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.Groups;

public class GroupsCreateViewModel : Core.ViewModel
{
    private readonly IRepository<TouristGroup, long> _groupsRepository;
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

    private TouristGroup _group = new();
    public TouristGroup Group
    {
        get => _group;
        set
        {
            _group = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<Client> _availableClients = new();
    public ObservableCollection<Client> AvailableClients
    {
        get => _availableClients;
        set
        {
            _availableClients = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<Client> _currentClients = new();
    public ObservableCollection<Client> CurrentClients
    {
        get => _currentClients;
        set
        {
            _currentClients = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanCreate));
        }
    }

    private List<TourGuide> _employees = new();
    public List<TourGuide> Employees
    {
        get => _employees;
        set
        {
            _employees = value;
            OnPropertyChanged();
        }
    }

    private List<Route> _routes = new();
    public List<Route> Routes
    {
        get => _routes;
        set
        {
            _routes = value;
            OnPropertyChanged();
        }
    }

    private Client _selectedIncludedClient = null!;
    public Client SelectedIncludedClient
    {
        get => _selectedIncludedClient;
        set
        {
            _selectedIncludedClient = value;
            OnPropertyChanged();
        }
    }

    private Client _selectedAvailableClient = null!;
    public Client SelectedAvailableClient
    {
        get => _selectedAvailableClient;
        set
        {
            _selectedAvailableClient = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand CreateCommand { get; private set; } = null!;
    public RelayCommand CancelCommand { get; private set; } = null!;
    public RelayCommand MoveItemToLeftCommand { get; private set; } = null!;
    public RelayCommand MoveItemToRightCommand { get; private set; } = null!;

    public GroupsCreateViewModel(
        IRepository<TouristGroup, long> groupsRepo,
        IRepository<TourGuide, long> employeesRepo,
        IRepository<Client, long> clientsRepo,
        IRepository<Route, long> routesRepo,
        INavigationService navigationService)
    {
        _groupsRepository = groupsRepo ?? throw new ArgumentNullException(nameof(groupsRepo));
        _clientsRepository = clientsRepo ?? throw new ArgumentNullException(nameof(clientsRepo));
        _employeesRepository = employeesRepo ?? throw new ArgumentNullException(nameof(employeesRepo));
        _routesRepository = routesRepo ?? throw new ArgumentNullException(nameof(routesRepo));
        Navigation = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        FetchAvailableClients();
        LoadEmployeesAndRoutes();

        InitializeCommands();

        Group.StartDatetime = DateTime.Now.AddDays(1).Date;
        Group.EndDatetime = DateTime.Now.AddDays(8).Date;
    }

    private void LoadEmployeesAndRoutes()
    {
        Employees = _employeesRepository.GetQuaryable()
            .Include(tg => tg.Person)
            .AsNoTracking()
            .ToList();

        Routes = _routesRepository.GetQuaryable()
            .Include(r => r.Country)
            .AsNoTracking()
            .ToList();
    }

    private void InitializeCommands()
    {
        CreateCommand = new RelayCommand(
            execute: _ => HandleCreating(),
            canExecute: _ => CanCreate);
        CancelCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<GroupsViewModel>(),
            canExecute: _ => true);
        MoveItemToLeftCommand = new RelayCommand(
            execute: _ => HandleClientAdding(),
            canExecute: _ => SelectedAvailableClient != null);
        MoveItemToRightCommand = new RelayCommand(
            execute: _ => HandleClientRemoving(),
            canExecute: _ => SelectedIncludedClient != null);
    }

    private bool CanCreate
    {
        get => !string.IsNullOrEmpty(Group.Name) && Group.TourGuideId > 0 && Group.RouteId > 0 &&
               Group.StartDatetime < Group.EndDatetime && CurrentClients.Any();
    }

    public void HandleCreating()
    {
        try
        {
            // Обновляем клиентов группы
            Group.Clients.Clear();
            foreach (var client in CurrentClients)
            {
                var dbClient = _clientsRepository.GetQuaryable().FirstOrDefault(c => c.Id == client.Id);
                if (dbClient != null)
                {
                    Group.Clients.Add(dbClient);
                }
            }

            var (isValid, errors) = Validator.ValidateTouristGroup(Group);
            if (!isValid)
            {
                MessageBox.Show(
                    $"Validation failed:\n{string.Join("\n", errors)}",
                    LocalizedStrings.Instance["InputErrorMessageBoxTitle"],
                    MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine($"Validation errors: {string.Join(", ", errors)}");
                return;
            }

            _groupsRepository.Insert(Group);
            _groupsRepository.SaveChanges();

            MessageBox.Show("Group created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Navigation.NavigateTo<GroupsViewModel>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error creating group: {ex.Message}", "Create Error", MessageBoxButton.OK, MessageBoxImage.Error);
            System.Diagnostics.Debug.WriteLine($"Exception during save: {ex}");
        }
    }

    private void HandleClientAdding()
    {
        if (SelectedAvailableClient != null)
        {
            CurrentClients.Add(SelectedAvailableClient);
            AvailableClients.Remove(SelectedAvailableClient);
            SelectedAvailableClient = null;
            OnPropertyChanged(nameof(AvailableClients));
            CommandManager.InvalidateRequerySuggested();
        }
    }

    private void HandleClientRemoving()
    {
        if (SelectedIncludedClient != null)
        {
            AvailableClients.Add(SelectedIncludedClient);
            CurrentClients.Remove(SelectedIncludedClient);
            SelectedIncludedClient = null;
            OnPropertyChanged(nameof(CurrentClients));
            CommandManager.InvalidateRequerySuggested();
        }
    }

    private void FetchAvailableClients()
    {
        var allClients = _clientsRepository.GetQuaryable()
            .Include(c => c.Person)
            .Include(c => c.TouristGroups)
            .AsNoTracking()
            .ToList();

        var employeePersonIds = _employeesRepository.GetQuaryable()
            .Select(tg => tg.PersonId)
            .ToList();
        AvailableClients = new ObservableCollection<Client>(
            allClients.Where(c => !employeePersonIds.Contains(c.PersonId) && (c.TouristGroups == null || !c.TouristGroups.Any()))
        );
        System.Diagnostics.Debug.WriteLine($"Loaded {AvailableClients.Count} available clients. First client: {AvailableClients.FirstOrDefault()?.Person?.FullName ?? "None"}");
    }
}