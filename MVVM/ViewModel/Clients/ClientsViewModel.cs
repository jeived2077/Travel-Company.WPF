using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Data.Dto;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.MVVM.ViewModel.Penalties;
using Travel_Company.WPF.Resources.Localizations;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.Clients;

public sealed class ClientsViewModel : Core.ViewModel
{
    private readonly IRepository<Client, long> _clientsRepository;
    private readonly IRepository<TourGuide, long> _tourGuidesRepository; // Added to filter out employees

    private INavigationService _navigation;
    public INavigationService Navigation
    {
        get => _navigation;
        set
        {
            _navigation = value;
            OnPropertyChanged();
        }
    }

    private List<Client> _fetchedClients;
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

    private Client _selectedClient = null!;
    public Client SelectedClient
    {
        get => _selectedClient;
        set
        {
            _selectedClient = value;
            OnPropertyChanged();
        }
    }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            FilterItems();
        }
    }

    private Visibility _isPassportDataVisible = Visibility.Hidden;
    public Visibility IsPassportDataVisible
    {
        get => _isPassportDataVisible;
        set
        {
            _isPassportDataVisible = value;
            OnPropertyChanged();
        }
    }

    private string _toggleButtonContent = LocalizedStrings.Instance["TextShowPassports"];
    public string ToggleButtonContent
    {
        get => _toggleButtonContent;
        set
        {
            _toggleButtonContent = value;
            OnPropertyChanged();
        }
    }

    private void FilterItems()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Clients = _fetchedClients.ToList();
        }
        else
        {
            Clients = _fetchedClients
                .Where(c => c.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    public RelayCommand NavigateToUpdatingCommand { get; set; } = null!;
    public RelayCommand NavigateToInsertingCommand { get; set; } = null!;
    public RelayCommand DeleteSelectedItemCommand { get; set; } = null!;
    public RelayCommand NavigateToPenaltiesCommand { get; set; } = null!;
    public RelayCommand ToggleColumnsCommand { get; set; } = null!;

    public ClientsViewModel(
        IRepository<Client, long> clientsRepository,
        IRepository<TourGuide, long> tourGuidesRepository, // Added dependency
        INavigationService navigation)
    {
        _clientsRepository = clientsRepository;
        _tourGuidesRepository = tourGuidesRepository; // Initialize
        _navigation = navigation;

        _fetchedClients = FetchDataGridData();
        Clients = _fetchedClients;

        InitializeCommands();
    }

    private List<Client> FetchDataGridData()
    {
        // Get all Person IDs that are associated with TourGuides
        var tourGuidePersonIds = _tourGuidesRepository.GetQuaryable()
            .Select(tg => tg.PersonId)
            .ToList();

        // Fetch Clients, excluding those whose PersonId is in tourGuidePersonIds
        return _clientsRepository.GetQuaryable()
            .Include(c => c.Person)
                .ThenInclude(p => p.Street)
            .Include(c => c.Passport)
            .Include(c => c.TouristGroups)
            .Include(c => c.Penalties)
            .Where(c => !tourGuidePersonIds.Contains(c.PersonId)) // Exclude TourGuides
            .ToList();
    }

    private void InitializeCommands()
    {
        NavigateToUpdatingCommand = new RelayCommand(
            execute: _ => HandleUpdating(),
            canExecute: _ => SelectedClient != null);
        NavigateToInsertingCommand = new RelayCommand(
           execute: _ => Navigation.NavigateTo<ClientsCreateViewModel>(),
           canExecute: _ => true);
        DeleteSelectedItemCommand = new RelayCommand(
            execute: _ => HandleDeleting(),
            canExecute: _ => SelectedClient != null);
        NavigateToPenaltiesCommand = new RelayCommand(
            execute: _ => HandleVisitingPenalties(),
            canExecute: _ => SelectedClient != null && SelectedClient.Penalties.Count > 0);
        ToggleColumnsCommand = new RelayCommand(
            execute: _ => HandleColumnToggling(),
            canExecute: _ => true);
    }

    private void HandleColumnToggling()
    {
        IsPassportDataVisible = (IsPassportDataVisible == Visibility.Hidden)
            ? Visibility.Visible
            : Visibility.Hidden;
        ToggleButtonContent = IsPassportDataVisible == Visibility.Hidden
            ? LocalizedStrings.Instance["TextShowPassports"]
            : LocalizedStrings.Instance["TextHidePassports"];
    }

    private void HandleDeleting()
    {
        if (SelectedClient is not null)
        {
            _clientsRepository.Delete(SelectedClient);
            _clientsRepository.SaveChanges();

            _fetchedClients = FetchDataGridData();
            Clients = _fetchedClients;
        }
    }

    private void HandleUpdating()
    {
        if (SelectedClient is not null)
        {
            var message = new ClientMessage { Client = SelectedClient };
            App.EventAggregator.Publish(message);
            Navigation.NavigateTo<ClientsUpdateViewModel>();
        }
    }

    private void HandleVisitingPenalties()
    {
        if (SelectedClient is not null && SelectedClient.Penalties.Count > 0)
        {
            var message = new ClientMessage { Client = SelectedClient };
            App.EventAggregator.Publish(message);
            Navigation.NavigateTo<PenaltiesViewModel>();
        }
    }
}