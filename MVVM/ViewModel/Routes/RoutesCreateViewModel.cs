using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Resources.Localizations;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.Routes;

public class RoutesCreateViewModel : Core.ViewModel
{
    private readonly IRepository<Route, long> _routesRepository;
    private readonly IRepository<Country, int> _countriesRepository;
    private readonly IRepository<PopulatedPlace, long> _placesRepository;
    private readonly IRepository<Hotel, long> _hotelsRepository;
    private readonly IRepository<RoutesPopulatedPlace, long> _placesInRoutesRepository;

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

    private Route _route = new()
    {
        StartDatetime = DateTime.Now,
        EndDatetime = DateTime.Now.AddMonths(1),
    };
    public Route Route
    {
        get => _route;
        set
        {
            _route = value;
            OnPropertyChanged();
        }
    }

    private List<Country> _countries = null!;
    public List<Country> Countries
    {
        get => _countries;
        set
        {
            _countries = value;
            OnPropertyChanged();
        }
    }

    private Country _country = null!;
    public Country Country
    {
        get => _country;
        set
        {
            _country = value;
            OnPropertyChanged();
        }
    }

    private List<Hotel> _hotels = null!;
    public List<Hotel> Hotels
    {
        get => _hotels;
        set
        {
            _hotels = value;
            OnPropertyChanged();
        }
    }

    private List<PopulatedPlace> _places = null!;
    public List<PopulatedPlace> Places
    {
        get => _places;
        set
        {
            _places = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<RoutesPopulatedPlace> _currentPlaces = null!;
    public ObservableCollection<RoutesPopulatedPlace> CurrentPlaces
    {
        get => _currentPlaces;
        set
        {
            _currentPlaces = value;
            OnPropertyChanged();
        }
    }

    private RoutesPopulatedPlace _selectedIncludedPlace = null!;
    public RoutesPopulatedPlace SelectedIncludedPlace
    {
        get => _selectedIncludedPlace;
        set
        {
            _selectedIncludedPlace = value;
            OnPropertyChanged();
        }
    }

    private RoutesPopulatedPlace _placeToAddOrEdit = null!;
    public RoutesPopulatedPlace PlaceToAddOrEdit
    {
        get => _placeToAddOrEdit;
        set
        {
            _placeToAddOrEdit = value;
            OnPropertyChanged();
        }
    }

    private RoutesPopulatedPlace _savedEditablePlaceData = null!;

    private Visibility _isInitialCommandsVisible = Visibility.Visible;
    public Visibility IsInitialCommandsVisible
    {
        get => _isInitialCommandsVisible;
        private set
        {
            _isInitialCommandsVisible = value;
            OnPropertyChanged();
        }
    }

    private Visibility _isSecondaryCommandsVisible = Visibility.Collapsed;
    public Visibility IsSecondaryCommandsVisible
    {
        get => _isSecondaryCommandsVisible;
        private set
        {
            _isSecondaryCommandsVisible = value;
            OnPropertyChanged();
        }
    }

    private bool _isCommandButtonEnabled = true;
    public bool IsCommandButtonEnabled
    {
        get => _isCommandButtonEnabled;
        private set
        {
            _isCommandButtonEnabled = value;
            OnPropertyChanged();
        }
    }

    private bool _isInputEnabled = false;
    public bool IsInputEnabled
    {
        get => _isInputEnabled;
        private set
        {
            _isInputEnabled = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand CreateCommand { get; set; } = null!;
    public RelayCommand CancelCommand { get; set; } = null!;
    public RelayCommand NewPlaceCommand { get; set; } = null!;
    public RelayCommand EditSelectedCommand { get; set; } = null!;
    public RelayCommand RemoveSelectedCommand { get; set; } = null!;
    public RelayCommand AddPlaceCommand { get; set; } = null!;
    public RelayCommand SaveEditCommand { get; set; } = null!;
    public RelayCommand CancelChangesCommand { get; set; } = null!;

    public RoutesCreateViewModel(
        IRepository<Country, int> countriesRepository,
        IRepository<RoutesPopulatedPlace, long> placesInRoutesRepository,
        IRepository<PopulatedPlace, long> placesRepository,
        IRepository<Route, long> routesRepo,
        IRepository<Hotel, long> hotelsRepository,
        INavigationService navigationService)
    {
        _countriesRepository = countriesRepository ?? throw new ArgumentNullException(nameof(countriesRepository));
        _placesInRoutesRepository = placesInRoutesRepository ?? throw new ArgumentNullException(nameof(placesInRoutesRepository));
        _placesRepository = placesRepository ?? throw new ArgumentNullException(nameof(placesRepository));
        _hotelsRepository = hotelsRepository ?? throw new ArgumentNullException(nameof(hotelsRepository));
        _routesRepository = routesRepo ?? throw new ArgumentNullException(nameof(routesRepo));
        Navigation = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        CurrentPlaces = new ObservableCollection<RoutesPopulatedPlace>();

        Countries = _countriesRepository.GetQuaryable().AsNoTracking().ToList() ?? new List<Country>(); // Fixed typo
        Hotels = _hotelsRepository.GetQuaryable().AsNoTracking().ToList() ?? new List<Hotel>(); // Fixed typo
        Places = _placesRepository.GetQuaryable().AsNoTracking().ToList() ?? new List<PopulatedPlace>(); // Fixed typo

        InitializeCommands();
    }

    private void InitializeCommands()
    {
        CreateCommand = new RelayCommand(
            execute: _ => HandleCreating(),
            canExecute: _ => Validator.ValidateRoute(Route).IsValid);
        CancelCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<RoutesViewModel>(),
            canExecute: _ => true);

        NewPlaceCommand = new RelayCommand(
            execute: _ => HandleNewPlaceCommand(),
            canExecute: _ => true);
        EditSelectedCommand = new RelayCommand(
            execute: _ => HandleEditSelectedCommand(),
            canExecute: _ => SelectedIncludedPlace != null);
        RemoveSelectedCommand = new RelayCommand(
            execute: _ => HandlePlaceRemoving(),
            canExecute: _ => SelectedIncludedPlace != null);

        AddPlaceCommand = new RelayCommand(
            execute: _ => HandleAddPlaceCommand(),
            canExecute: _ => Validator.ValidatePopulatedPlaceInRoute(PlaceToAddOrEdit).IsValid);
        SaveEditCommand = new RelayCommand(
            execute: _ => HandleSaveEditCommand(),
            canExecute: _ => Validator.ValidatePopulatedPlaceInRoute(PlaceToAddOrEdit).IsValid);
        CancelChangesCommand = new RelayCommand(
            execute: _ => HandleCancelChangesCommand(),
            canExecute: _ => true);
    }

    private void HandleNewPlaceCommand()
    {
        PlaceToAddOrEdit = new RoutesPopulatedPlace
        {
            StayStartDatetime = DateTime.Now,
            StayEndDatetime = DateTime.Now
        };
        UnlockPlaceFields();
    }

    private void HandleEditSelectedCommand()
    {
        if (SelectedIncludedPlace != null && CurrentPlaces.Count > 0)
        {
            PlaceToAddOrEdit = SelectedIncludedPlace;
            _savedEditablePlaceData = SelectedIncludedPlace;
            UnlockPlaceFields();
        }
    }

    private void HandlePlaceRemoving()
    {
        if (SelectedIncludedPlace != null && CurrentPlaces.Count > 0)
        {
            CurrentPlaces.Remove(SelectedIncludedPlace);
        }
    }

    private void HandleAddPlaceCommand()
    {
        var (isValid, errors) = Validator.ValidatePopulatedPlaceInRoute(PlaceToAddOrEdit);
        if (!isValid)
        {
            MessageBox.Show(
                $"Validation failed:\n{string.Join("\n", errors)}",
                LocalizedStrings.Instance["InputErrorMessageBoxTitle"],
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        CurrentPlaces.Add(PlaceToAddOrEdit);
        PlaceToAddOrEdit = null!;
        LockPlaceFields();
    }

    private void HandleSaveEditCommand()
    {
        var (isValid, errors) = Validator.ValidatePopulatedPlaceInRoute(PlaceToAddOrEdit);
        if (!isValid)
        {
            MessageBox.Show(
                $"Validation failed:\n{string.Join("\n", errors)}",
                LocalizedStrings.Instance["InputErrorMessageBoxTitle"],
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        PlaceToAddOrEdit = null!;
        LockPlaceFields();
    }

    private void HandleCancelChangesCommand()
    {
        PlaceToAddOrEdit = null!;
        LockPlaceFields();
        _savedEditablePlaceData = null!;
    }

    private void UnlockPlaceFields()
    {
        IsInputEnabled = true;
        IsCommandButtonEnabled = false;
        IsInitialCommandsVisible = Visibility.Collapsed;
        IsSecondaryCommandsVisible = Visibility.Visible;
    }

    private void LockPlaceFields()
    {
        IsInputEnabled = false;
        IsCommandButtonEnabled = true;
        IsInitialCommandsVisible = Visibility.Visible;
        IsSecondaryCommandsVisible = Visibility.Collapsed;
    }

    private void HandleCreating()
    {
        var (isValid, errors) = Validator.ValidateRoute(Route);
        if (!isValid)
        {
            MessageBox.Show(
                $"Validation failed:\n{string.Join("\n", errors)}",
                LocalizedStrings.Instance["InputErrorMessageBoxTitle"],
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            UpdatePlacesInRoute();
            _routesRepository.Insert(Route);
            _routesRepository.SaveChanges();

            MessageBox.Show("Route created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Navigation.NavigateTo<RoutesViewModel>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error creating route: {ex.Message}", "Create Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdatePlacesInRoute()
    {
        Route.RoutesPopulatedPlaces.Clear();
        foreach (var place in CurrentPlaces)
        {
            Route.RoutesPopulatedPlaces.Add(place);
        }
    }
}