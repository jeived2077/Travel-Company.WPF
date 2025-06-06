using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Data.Dto;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Resources.Localizations;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.Clients;

public class ClientsUpdateViewModel : Core.ViewModel
{
    private readonly IRepository<Street, long> _streetsRepository = null!;
    private readonly IRepository<TouristGroup, long> _groupsRepository = null!;
    private readonly IRepository<Client, long> _clientsRepository = null!;
    private readonly TravelCompanyDbContext _dbContext; // Inject DbContext

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

    private Client _client = null!;
    public Client Client
    {
        get => _client;
        set
        {
            _client = value;
            OnPropertyChanged();
        }
    }

    private List<Street> _streets = null!;
    public List<Street> Streets
    {
        get => _streets;
        set
        {
            _streets = value;
            OnPropertyChanged();
        }
    }

    private List<TouristGroup> _groups = null!;
    public List<TouristGroup> Groups
    {
        get => _groups;
        set
        {
            _groups = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand UpdateCommand { get; set; }
    public RelayCommand CancelCommand { get; set; }
    public RelayCommand ChangeProfilePictureCommand { get; set; }
    public RelayCommand RemoveProfilePictureCommand { get; set; }

    public ClientsUpdateViewModel(
        IRepository<Street, long> streetsRepo,
        IRepository<Client, long> clientsRepo,
        IRepository<TouristGroup, long> groupsRepo,
        INavigationService navigationService,
        TravelCompanyDbContext dbContext) // Add DbContext parameter
    {
        _streetsRepository = streetsRepo;
        _groupsRepository = groupsRepo;
        _clientsRepository = clientsRepo;
        _dbContext = dbContext; // Store DbContext
        Navigation = navigationService;

        Streets = _streetsRepository.GetAll() ?? new List<Street>(); // Ensure not null
        Groups = _groupsRepository.GetAll() ?? new List<TouristGroup>(); // Ensure not null

        App.EventAggregator.Subscribe<ClientMessage>(HandleStartupMessage);

        UpdateCommand = new RelayCommand(
            execute: _ => HandleUpdating(),
            canExecute: _ => true);
        CancelCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<ClientsViewModel>(),
            canExecute: _ => true);
        ChangeProfilePictureCommand = new RelayCommand(
            execute: _ => ChangeProfilePicture(),
            canExecute: _ => true);
        RemoveProfilePictureCommand = new RelayCommand(
            execute: _ => Client.Photograph = null,
            canExecute: _ => true);
    }

    private void ChangeProfilePicture()
    {
        ImageHandler.ChangeProfilePicture(Client);
    }

    private void HandleStartupMessage(ClientMessage message)
    {
        // Reload the client with related data to ensure it's tracked by the context
        Client = _clientsRepository
            .GetQuaryable()
            .Include(c => c.Person)
            .Include(c => c.Passport)
            .Include(c => c.TouristGroups)
            .FirstOrDefault(c => c.Id == message.Client.Id) ?? message.Client;
    }

    private void HandleUpdating()
    {
        try
        {
            if (!Validator.ValidateClient(Client))
            {
                string validationErrors = "Validation failed. Please check the following:\n";
                MessageBox.Show(
                    validationErrors + LocalizedStrings.Instance["InputErrorMessageBoxText"],
                    LocalizedStrings.Instance["InputErrorMessageBoxTitle"],
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Use DbContext for tracking
            var trackedClient = _dbContext.Clients
                .Include(c => c.Person)
                .Include(c => c.Passport)
                .Include(c => c.TouristGroups)
                .FirstOrDefault(c => c.Id == Client.Id);

            if (trackedClient != null)
            {
                _dbContext.Entry(trackedClient).CurrentValues.SetValues(Client);
            }
            else
            {
                _dbContext.Clients.Update(Client); // Attach and mark as modified
            }
            _dbContext.SaveChanges();

            Navigation.NavigateTo<ClientsViewModel>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating client: {ex.Message}\n{ex.InnerException?.Message}", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}