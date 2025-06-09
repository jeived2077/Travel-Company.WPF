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
    private readonly IRepository<TourGuide, long> _tourGuidesRepository;

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

    private List<Client> _fetchedClients = new List<Client>();
    private List<Client> _clients = new List<Client>();
    public List<Client> Clients
    {
        get => _clients;
        set
        {
            _clients = value;
            OnPropertyChanged();
        }
    }

    private Client _selectedClient;
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
                .Where(c => c.Person.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    public RelayCommand NavigateToUpdatingCommand { get; set; }
    public RelayCommand NavigateToInsertingCommand { get; set; }
    public RelayCommand DeleteSelectedItemCommand { get; set; }
    public RelayCommand NavigateToPenaltiesCommand { get; set; }
    public RelayCommand ToggleColumnsCommand { get; set; }

    public ClientsViewModel(
        IRepository<Client, long> clientsRepository,
        IRepository<TourGuide, long> tourGuidesRepository,
        INavigationService navigation)
    {
        _clientsRepository = clientsRepository ?? throw new ArgumentNullException(nameof(clientsRepository));
        _tourGuidesRepository = tourGuidesRepository ?? throw new ArgumentNullException(nameof(tourGuidesRepository));
        _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

        _fetchedClients = FetchDataGridData();
        Clients = _fetchedClients;

        InitializeCommands();
        App.EventAggregator.Subscribe<ClientMessage>(HandleClientUpdated);
    }

    private List<Client> FetchDataGridData()
    {
        var tourGuidePersonIds = _tourGuidesRepository.GetQuaryable()
            .Select(tg => tg.PersonId)
            .ToList();

        var clients = _clientsRepository.GetQuaryable()
            .Include(c => c.Person).ThenInclude(p => p.Passport)
            .Include(c => c.Person).ThenInclude(p => p.Street)
            .Include(c => c.TouristGroups)
            .Include(c => c.Penalties)
            .Where(c => !tourGuidePersonIds.Contains(c.PersonId))
            .ToList();

        // Отладка загрузки паспортных данных
        System.Diagnostics.Debug.WriteLine($"Fetched {clients.Count} clients. First client passport: {clients.FirstOrDefault()?.Person?.Passport?.FullPassportNumber ?? "No passport"}");

        // Инициализация и сохранение паспорта, если он отсутствует
        foreach (var client in clients)
        {
            if (client.Person.Passport == null)
            {
                var newPassport = new Passport
                {
                    PersonId = client.PersonId,
                    PassportSeries = "N/A", // Значение по умолчанию
                    PassportNumber = "N/A", // Значение по умолчанию
                    PassportIssueDate = DateTime.Now, // Значение по умолчанию
                    PassportIssuingAuthority = "N/A" // Значение по умолчанию
                };
                client.Person.Passport = newPassport;
                _clientsRepository.GetContext().Set<Passport>().Add(newPassport); // Добавляем новый паспорт
                _clientsRepository.SaveChanges(); // Сохраняем изменения
                System.Diagnostics.Debug.WriteLine($"Initialized and saved passport for client {client.Person.FullName}: {newPassport.FullPassportNumber}");
            }
        }

        return clients;
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
        System.Diagnostics.Debug.WriteLine($"Toggled passport visibility to {IsPassportDataVisible}");
    }

    private void HandleDeleting()
    {
        if (SelectedClient != null)
        {
            _clientsRepository.Delete(SelectedClient);
            _clientsRepository.SaveChanges();

            _fetchedClients = FetchDataGridData();
            Clients = _fetchedClients;
        }
    }

    private void HandleUpdating()
    {
        if (SelectedClient != null)
        {
            var message = new ClientMessage { Client = SelectedClient };
            App.EventAggregator.Publish(message);
            Navigation.NavigateTo<ClientsUpdateViewModel>();
        }
    }

    private void HandleVisitingPenalties()
    {
        if (SelectedClient != null && SelectedClient.Penalties.Count > 0)
        {
            var message = new ClientMessage { Client = SelectedClient };
            App.EventAggregator.Publish(message);
            Navigation.NavigateTo<PenaltiesViewModel>();
        }
    }

    private void HandleClientUpdated(ClientMessage message)
    {
        _fetchedClients = FetchDataGridData();
        Clients = _fetchedClients.ToList();
        SelectedClient = _fetchedClients.FirstOrDefault(c => c.Id == message.Client.Id);
    }
}