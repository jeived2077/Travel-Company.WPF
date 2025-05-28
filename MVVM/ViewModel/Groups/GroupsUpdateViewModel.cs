using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Data.Dto;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Resources.Localizations;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.Groups;

public class GroupsUpdateViewModel : Core.ViewModel
{
    private readonly IRepository<TouristGroup, long> _groupsRepository;
    private readonly IRepository<TourGuide, int> _employeesRepository;
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

    private TouristGroup _group = null!;
    public TouristGroup Group
    {
        get => _group;
        set
        {
            _group = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<Client> _availableClients = null!;
    public ObservableCollection<Client> AvailableClients
    {
        get => _availableClients;
        set
        {
            _availableClients = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<Client> _currentClients = null!;
    public ObservableCollection<Client> CurrentClients
    {
        get => _currentClients;
        set
        {
            _currentClients = value;
            OnPropertyChanged();
        }
    }

    private long _selectedClientId = 0;

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

    public RelayCommand UpdateCommand { get; set; }
    public RelayCommand CancelCommand { get; set; }
    public RelayCommand MoveItemToLeftCommand { get; set; }
    public RelayCommand MoveItemToRightCommand { get; set; }

    public GroupsUpdateViewModel(
        IRepository<TouristGroup, long> groupsRepo,
        IRepository<TourGuide, int> employeesRepo,
        IRepository<Client, long> clientsRepo,
        IRepository<Route, long> routesRepo,
        INavigationService navigationService)
    {
        _groupsRepository = groupsRepo;
        _clientsRepository = clientsRepo;
        _employeesRepository = employeesRepo;
        _routesRepository = routesRepo;
        Navigation = navigationService;

        App.EventAggregator.Subscribe<TouristGroupMessage>(HandleStartupMessage);

        Employees = _employeesRepository.GetAll();
        Routes = _routesRepository.GetAll();

        UpdateCommand = new RelayCommand(
            execute: _ => HandleUpdating(),
            canExecute: _ => true);
        CancelCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<GroupsViewModel>(),
            canExecute: _ => true);
        MoveItemToLeftCommand = new RelayCommand(
            execute: _ => HandleClientAdding(),
            canExecute: _ => true);
        MoveItemToRightCommand = new RelayCommand(
            execute: _ => HandleClientRemoving(),
            canExecute: _ => true);
    }

    private void HandleClientAdding()
    {
        if (SelectedAvailableClient is not null && AvailableClients.Count > 0)
        {
            _selectedClientId = SelectedAvailableClient.Id;
            CurrentClients.Add(SelectedAvailableClient);

            var availableClients = AvailableClients
                .Where(c => c.Id != _selectedClientId)
                .ToList();
            var currentClients = CurrentClients.ToList();
            UpdateAvailableClients(availableClients);
            UpdateCurrentClients(currentClients);
        }
    }

    private void HandleClientRemoving()
    {
        if (SelectedIncludedClient is not null && CurrentClients.Count > 0)
        {
            _selectedClientId = SelectedIncludedClient.Id;
            AvailableClients.Add(SelectedIncludedClient);

            var currentClients = CurrentClients
                .Where(c => c.Id != _selectedClientId)
                .ToList();
            var availableClients = AvailableClients.ToList();
            UpdateAvailableClients(availableClients);
            UpdateCurrentClients(currentClients);
        }
    }

    private void UpdateAvailableClients(List<Client> availableClients)
    {
        AvailableClients.Clear();
        foreach (var client in availableClients)
        {
            AvailableClients.Add(client);
        }
    }

    private void UpdateCurrentClients(List<Client> currentClients)
    {
        CurrentClients.Clear();
        foreach (var client in currentClients)
        {
            CurrentClients.Add(client);
        }
    }

    private void HandleStartupMessage(TouristGroupMessage message)
    {
        Group = message.Group;
        App.EventAggregator.RemoveMessage<TouristGroupMessage>();
        CurrentClients = new ObservableCollection<Client>(Group.Clients.ToList());
        FetchAvailableClients();
    }

    private void FetchAvailableClients()
    {
        List<long> groupClientIds = Group.Clients.Select(c => c.Id).ToList();
        IQueryable<Client> availableClientsQuery = _clientsRepository
           .GetQuaryable()
           .Include(c => c.TouristGroups)
           .Where(client => !groupClientIds.Contains(client.Id));

        AvailableClients = new ObservableCollection<Client>(availableClientsQuery.ToList());
    }

    private void HandleUpdating()
    {
        if (!Validator.ValidateTouristGroup(Group))
        {
            MessageBox.Show(
                LocalizedStrings.Instance["InputErrorMessageBoxText"],
                LocalizedStrings.Instance["InputErrorMessageBoxTitle"],
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        AddUpdatedClients();
        _groupsRepository.Update(Group);
        _groupsRepository.SaveChanges();

        Navigation.NavigateTo<GroupsViewModel>();
    }

    private void AddUpdatedClients()
    {
        Group.Clients.Clear();
        foreach (var client in CurrentClients)
        {
            Group.Clients.Add(client);
        }
    }
}