using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Data.Dto;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Resources.Localizations;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.Groups;

public class GroupsUpdateViewModel : Core.ViewModel
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

    private TouristGroup _group = null!;
    public TouristGroup Group
    {
        get => _group;
        set
        {
            _group = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanUpdate)); // Обновляем CanUpdate при изменении Group
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
            OnPropertyChanged(nameof(CanUpdate)); // Обновляем CanUpdate при изменении CurrentClients
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

    public RelayCommand UpdateCommand { get; set; }
    public RelayCommand CancelCommand { get; set; }
    public RelayCommand MoveItemToLeftCommand { get; set; }
    public RelayCommand MoveItemToRightCommand { get; set; }

    // Свойство CanUpdate для определения активности кнопки
    public bool CanUpdate
    {
        get
        {
            return !string.IsNullOrEmpty(Group?.Name) &&
                   Group?.TourGuideId > 0 &&
                   Group?.RouteId > 0 &&
                   Group?.StartDatetime < Group?.EndDatetime &&
                   CurrentClients?.Any() == true;
        }
    }

    public GroupsUpdateViewModel(
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

        App.EventAggregator.Subscribe<TouristGroupMessage>(HandleStartupMessage);

        Employees = _employeesRepository.GetQuaryable()
            .Include(tg => tg.Person)
            .AsNoTracking()
            .ToList();

        Routes = _routesRepository.GetQuaryable()
            .Include(r => r.Country)
            .AsNoTracking()
            .ToList();

        UpdateCommand = new RelayCommand(
            execute: _ => HandleUpdating(),
            canExecute: _ => CanUpdate);
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

    private void HandleClientAdding()
    {
        if (SelectedAvailableClient != null)
        {
            CurrentClients.Add(SelectedAvailableClient);
            AvailableClients.Remove(SelectedAvailableClient);
            SelectedAvailableClient = null;
            OnPropertyChanged(nameof(AvailableClients));
            OnPropertyChanged(nameof(CanUpdate)); // Обновляем CanUpdate
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
            OnPropertyChanged(nameof(CanUpdate)); // Обновляем CanUpdate
            CommandManager.InvalidateRequerySuggested();
        }
    }

    private void HandleStartupMessage(TouristGroupMessage message)
    {
        Group = _groupsRepository.GetQuaryable()
            .Include(g => g.Clients).ThenInclude(c => c.Person)
            .Include(g => g.Route)
            .Include(g => g.TourGuide).ThenInclude(tg => tg.Person)
            .AsNoTracking()
            .FirstOrDefault(g => g.Id == message.Group.Id) ?? message.Group;

        App.EventAggregator.RemoveMessage<TouristGroupMessage>();
        CurrentClients = new ObservableCollection<Client>(Group.Clients.ToList());
        FetchAvailableClients();
    }

    private void FetchAvailableClients()
    {
        List<long> groupClientIds = Group.Clients.Select(c => c.Id).ToList();
        var allClients = _clientsRepository.GetQuaryable()
            .Include(c => c.Person)
            .Include(c => c.TouristGroups)
            .AsNoTracking()
            .ToList();

        var employeePersonIds = _employeesRepository.GetQuaryable()
            .Select(tg => tg.PersonId)
            .ToList();
        AvailableClients = new ObservableCollection<Client>(
            allClients.Where(c => !groupClientIds.Contains(c.Id) && !employeePersonIds.Contains(c.PersonId))
        );
        System.Diagnostics.Debug.WriteLine($"Loaded {AvailableClients.Count} available clients in UpdateViewModel. First client: {AvailableClients.FirstOrDefault()?.Person?.FullName ?? "None"}");
    }

    public void HandleUpdating()
    {
        try
        {
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

            _groupsRepository.Update(Group);
            _groupsRepository.SaveChanges();

            MessageBox.Show(
                LocalizedStrings.Instance["GroupUpdatedSuccessMessage"] ?? "Group updated successfully!",
                "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
            Navigation.NavigateTo<GroupsViewModel>();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                LocalizedStrings.Instance["GroupUpdateErrorMessage"] ?? $"Error updating group: {ex.Message}",
                LocalizedStrings.Instance["UpdateErrorMessageBoxTitle"] ?? "Update Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            System.Diagnostics.Debug.WriteLine($"Exception during save: {ex}");
        }
    }

    private void UpdateGroupClients()
    {
        var originalGroup = _groupsRepository.GetQuaryable()
            .Include(g => g.Clients)
            .FirstOrDefault(g => g.Id == Group.Id);

        if (originalGroup == null)
        {
            throw new InvalidOperationException("Group not found in the database.");
        }

        var updatedClientIds = CurrentClients.Select(c => c.Id).ToList();
        var originalClientIds = originalGroup.Clients.Select(c => c.Id).ToList();

        var clientsToRemove = originalGroup.Clients
            .Where(c => !updatedClientIds.Contains(c.Id))
            .ToList();
        foreach (var client in clientsToRemove)
        {
            originalGroup.Clients.Remove(client);
        }

        var clientsToAdd = _clientsRepository.GetQuaryable()
            .Where(c => updatedClientIds.Except(originalClientIds).Contains(c.Id))
            .ToList();
        foreach (var client in clientsToAdd)
        {
            originalGroup.Clients.Add(client);
        }

        originalGroup.Name = Group.Name;
        originalGroup.TourGuideId = Group.TourGuideId;
        originalGroup.RouteId = Group.RouteId;
        originalGroup.StartDatetime = Group.StartDatetime;
        originalGroup.EndDatetime = Group.EndDatetime;

        Group = originalGroup;
    }
}