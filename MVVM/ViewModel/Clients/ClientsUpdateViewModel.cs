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
using Travel_Company.WPF.MVVM.ViewModel.Clients;
using Travel_Company.WPF.Resources.Localizations;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.Clients;

public class ClientsUpdateViewModel : Core.ViewModel
{
    private readonly IRepository<Street, long> _streetsRepository;
    private readonly IRepository<TouristGroup, long> _groupsRepository;
    private readonly IRepository<Client, Guid> _clientsRepository;
    private readonly TravelCompanyDbContext _dbContext;

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

    private List<Street> _streets = new List<Street>();
    public List<Street> Streets
    {
        get => _streets;
        set
        {
            _streets = value;
            OnPropertyChanged();
        }
    }

    private List<TouristGroup> _groups = new List<TouristGroup>();
    public List<TouristGroup> Groups
    {
        get => _groups;
        set
        {
            _groups = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand UpdateCommand { get; private set; }
    public RelayCommand CancelCommand { get; private set; }
    public RelayCommand ChangeProfilePictureCommand { get; private set; }
    public RelayCommand RemoveProfilePictureCommand { get; private set; }

    public ClientsUpdateViewModel(
        IRepository<Street, long> streetsRepo,
        IRepository<Client, Guid> clientsRepo,
        IRepository<TouristGroup, long> groupsRepo,
        INavigationService navigationService,
        TravelCompanyDbContext dbContext)
    {
        _streetsRepository = streetsRepo ?? throw new ArgumentNullException(nameof(streetsRepo));
        _groupsRepository = groupsRepo ?? throw new ArgumentNullException(nameof(groupsRepo));
        _clientsRepository = clientsRepo ?? throw new ArgumentNullException(nameof(clientsRepo));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _navigation = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        Streets = _streetsRepository.GetQuaryable().AsNoTracking().ToList() ?? new List<Street>();
        Groups = _groupsRepository.GetQuaryable().AsNoTracking().ToList() ?? new List<TouristGroup>();

        App.EventAggregator.Subscribe<ClientMessage>(HandleStartupMessage);

        InitializeCommands();
    }

    private void InitializeCommands()
    {
        UpdateCommand = new RelayCommand(
            execute: _ => HandleUpdating(),
            canExecute: _ => Client != null && Validator.ValidateClient(Client).IsValid);
        CancelCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<ClientsViewModel>(),
            canExecute: _ => true);
        ChangeProfilePictureCommand = new RelayCommand(
            execute: _ => ChangeProfilePicture(),
            canExecute: _ => true);
        RemoveProfilePictureCommand = new RelayCommand(
            execute: _ => { Client.Photograph = null; OnPropertyChanged(nameof(Client)); },
            canExecute: _ => Client?.Photograph != null);
    }

    private void ChangeProfilePicture()
    {
        var photo = ImageHandler.ChangeProfilePicture(Client);
        if (photo != null && photo.Length <= 1024 * 1024) // 1MB limit
        {
            Client.Photograph = photo;
            OnPropertyChanged(nameof(Client));
        }
        else
        {
            MessageBox.Show("Image is too large (max 1MB) or not selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void HandleStartupMessage(ClientMessage message)
    {
        Client = _clientsRepository
            .GetQuaryable() // Fixed typo: GetQuaryable -> GetQueryable
            .Include(c => c.Person).ThenInclude(p => p!.Passport)
            .Include(c => c.TouristGroups)
            .FirstOrDefault(c => c.Id == message.Client.Id) ?? message.Client;
    }

    private void HandleUpdating()
    {
        try
        {
            var (isValid, errors) = Validator.ValidateClient(Client);
            if (!isValid)
            {
                MessageBox.Show(
                    $"Validation failed:\n{string.Join("\n", errors)}",
                    LocalizedStrings.Instance["InputErrorMessageBoxTitle"],
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var trackedClient = _dbContext.Clients
                .Include(c => c.Person).ThenInclude(p => p!.Passport)
                .Include(c => c.TouristGroups)
                .FirstOrDefault(c => c.Id == Client.Id);

            if (trackedClient != null)
            {
                // Update scalar properties
                _dbContext.Entry(trackedClient).CurrentValues.SetValues(Client);

                // Update Person
                if (trackedClient.Person != null && Client.Person != null)
                {
                    _dbContext.Entry(trackedClient.Person).CurrentValues.SetValues(Client.Person);

                    // Update Passport
                    if (trackedClient.Person.Passport != null && Client.Person.Passport != null)
                    {
                        _dbContext.Entry(trackedClient.Person.Passport).CurrentValues.SetValues(Client.Person.Passport);
                    }
                }

                // Update TouristGroups (many-to-many)
                if (Client.TouristGroups != null)
                {
                    trackedClient.TouristGroups.Clear();
                    foreach (var group in Client.TouristGroups)
                    {
                        var existingGroup = _dbContext.TouristGroups.Find(group.Id);
                        if (existingGroup != null)
                            trackedClient.TouristGroups.Add(existingGroup);
                    }
                }
            }
            else
            {
                _clientsRepository.Update(Client);
            }

            _dbContext.SaveChanges();

            MessageBox.Show("Client updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Navigation.NavigateTo<ClientsViewModel>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating client: {ex.Message}\n{ex.InnerException?.Message}", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Console.WriteLine(ex.ToString());
        }
    }
}