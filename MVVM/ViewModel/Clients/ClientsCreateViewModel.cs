using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Resources.Localizations;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.Clients;

public class ClientsCreateViewModel : Core.ViewModel
{
    private readonly IRepository<Street, long> _streetsRepository;
    private readonly IRepository<TouristGroup, long> _groupsRepository;
    private readonly IRepository<Client, Guid> _clientsRepository;
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

    private Client _client = new()
    {
        Person = new Person
        {
            Birthdate = DateTime.Now.AddYears(-18),
            Passport = new Passport { PassportIssueDate = DateTime.Now.AddYears(-18) } // Гарантируем инициализацию
        },
        TouristGroups = new List<TouristGroup>(),
        Penalties = new List<Penalty>(),
        Payments = new List<Payment>()
    };
    public Client Client
    {
        get => _client;
        set
        {
            _client = value;
            OnPropertyChanged();
        }
    }

    private List<Street> _streets = new();
    public List<Street> Streets
    {
        get => _streets;
        set
        {
            _streets = value;
            OnPropertyChanged();
        }
    }

    private List<TouristGroup> _groups = new();
    public List<TouristGroup> Groups
    {
        get => _groups;
        set
        {
            _groups = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand CreateCommand { get; private set; } = null!;
    public RelayCommand CancelCommand { get; private set; } = null!;
    public RelayCommand ChangeProfilePictureCommand { get; private set; } = null!;
    public RelayCommand RemoveProfilePictureCommand { get; private set; } = null!;

    public ClientsCreateViewModel(
        IRepository<Street, long> streetsRepo,
        IRepository<Client, Guid> clientsRepo,
        IRepository<TouristGroup, long> groupsRepo,
        INavigationService navigationService)
    {
        _streetsRepository = streetsRepo ?? throw new ArgumentNullException(nameof(streetsRepo));
        _groupsRepository = groupsRepo ?? throw new ArgumentNullException(nameof(groupsRepo));
        _clientsRepository = clientsRepo ?? throw new ArgumentNullException(nameof(clientsRepo));
        _navigation = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        _ = LoadDataAsync(); // Fire and forget with proper async handling

        // Инициализация команд в конструкторе
        CreateCommand = new RelayCommand(
            execute: _ => HandleCreating(),
            canExecute: _ => Validator.ValidateClient(Client).IsValid);
        CancelCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<ClientsViewModel>(),
            canExecute: _ => true);
        ChangeProfilePictureCommand = new RelayCommand(
            execute: _ => ChangeProfilePicture(),
            canExecute: _ => true);
        RemoveProfilePictureCommand = new RelayCommand(
            execute: _ => { Client.Photograph = null; OnPropertyChanged(nameof(Client)); },
            canExecute: _ => Client.Photograph != null);
    }

    private async Task LoadDataAsync()
    {
        try
        {
            Streets = await _streetsRepository.GetQuaryable()
                .AsNoTracking()
                .ToListAsync() ?? new List<Street>();

            Groups = await _groupsRepository.GetQuaryable()
                .AsNoTracking()
                .ToListAsync() ?? new List<TouristGroup>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading data: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ChangeProfilePicture()
    {
        var photo = ImageHandler.ChangeProfilePicture(Client);
        if (photo != null && photo.Length <= 1024 * 1024) // 1MB limit
        {
            Client.Photograph = photo;
            OnPropertyChanged(nameof(Client)); // Уведомляем о изменении всего объекта
        }
        else
        {
            MessageBox.Show(
                LocalizedStrings.Instance["ImageSizeErrorMessage"] ?? "Image is too large (max 1MB) or not selected.",
                LocalizedStrings.Instance["InputErrorMessageBoxTitle"],
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void HandleCreating()
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

            if (!Client.Person.StreetId.HasValue && Streets.Any())
            {
                Client.Person.StreetId = Streets.First().Id;
            }

            _clientsRepository.Insert(Client);
            _clientsRepository.SaveChanges();

            MessageBox.Show("Client created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Navigation.NavigateTo<ClientsViewModel>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error creating client: {ex.Message}", "Create Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}