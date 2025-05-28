using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Data.Dto;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Resources.Localizations;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.Routes;

public class RoutesUpdateViewModel : Core.ViewModel
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

    private Route _route = null!;
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

    public RelayCommand UpdateCommand { get; set; }
    public RelayCommand CancelCommand { get; set; }
    public RelayCommand NewPlaceCommand { get; set; }
    public RelayCommand EditSelectedCommand { get; set; }
    public RelayCommand RemoveSelectedCommand { get; set; }
    public RelayCommand AddPlaceCommand { get; set; }
    public RelayCommand SaveEditCommand { get; set; }
    public RelayCommand CancelChangesCommand { get; set; }

    public RoutesUpdateViewModel(
         IRepository<Country, int> countriesRepository,
         IRepository<RoutesPopulatedPlace, long> placesInRoutesRepository,
         IRepository<PopulatedPlace, long> placesRepository,
         IRepository<Route, long> routesRepo,
         IRepository<Hotel, long> hotelsRepository,
         INavigationService navigationService)
    {
        _countriesRepository = countriesRepository;
        _placesInRoutesRepository = placesInRoutesRepository;
        _placesRepository = placesRepository;
        _hotelsRepository = hotelsRepository;
        _routesRepository = routesRepo;
        Navigation = navigationService;

        App.EventAggregator.Subscribe<RouteMessage>(HandleStartupMessage);

        Countries = _countriesRepository.GetAll();
        Hotels = _hotelsRepository.GetAll();
        Places = _placesRepository.GetAll();

        UpdateCommand = new RelayCommand(
            execute: _ => HandleUpdating(),
            canExecute: _ => true);
        CancelCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<RoutesViewModel>(),
            canExecute: _ => true);

        NewPlaceCommand = new RelayCommand(
            execute: _ => HandleNewPlaceCommand(),
            canExecute: _ => true);
        EditSelectedCommand = new RelayCommand(
            execute: _ => HandleEditSelectedCommand(),
            canExecute: _ => true);
        RemoveSelectedCommand = new RelayCommand(
            execute: _ => HandlePlaceRemoving(),
            canExecute: _ => true);

        AddPlaceCommand = new RelayCommand(
            execute: _ => HandleAddPlaceCommand(),
            canExecute: _ => true);
        SaveEditCommand = new RelayCommand(
            execute: _ => HandleSaveEditCommand(),
            canExecute: _ => true);
        CancelChangesCommand = new RelayCommand(
            execute: _ => HandleCancelChangesCommand(),
            canExecute: _ => true);
    }

    private void HandleNewPlaceCommand()
    {
        PlaceToAddOrEdit = new()
        {
            StayStartDatetime = DateTime.Now,
            StayEndDatetime = DateTime.Now
        };
        UnlockPlaceFields();
    }

    private void HandleEditSelectedCommand()
    {
        if (SelectedIncludedPlace is not null && CurrentPlaces.Count > 0)
        {
            PlaceToAddOrEdit = SelectedIncludedPlace;
            _savedEditablePlaceData = SelectedIncludedPlace;
            UnlockPlaceFields();
        }
    }

    private void HandlePlaceRemoving()
    {
        if (SelectedIncludedPlace is not null && CurrentPlaces.Count > 0)
        {
            CurrentPlaces.Remove(SelectedIncludedPlace);
        }
    }

    private void HandleAddPlaceCommand()
    {
        if (!Validator.ValidatePopulatedPlaceInRoute(PlaceToAddOrEdit))
        {
            MessageBox.Show(
                LocalizedStrings.Instance["InputErrorMessageBoxText"],
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
        if (!Validator.ValidatePopulatedPlaceInRoute(PlaceToAddOrEdit))
        {
            MessageBox.Show(
                LocalizedStrings.Instance["InputErrorMessageBoxText"],
                LocalizedStrings.Instance["InputErrorMessageBoxTitle"],
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        PlaceToAddOrEdit = null!;
        LockPlaceFields();
    }

    private void HandleCancelChangesCommand()
    {
        // TODO: rollback changes
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

    private void HandleStartupMessage(RouteMessage message)
    {
        Route = message.Route;
        App.EventAggregator.RemoveMessage<RouteMessage>();
        CurrentPlaces = new ObservableCollection<RoutesPopulatedPlace>(Route.RoutesPopulatedPlaces);
    }

    private void HandleUpdating()
    {
        if (!Validator.ValidateRoute(Route))
        {
            MessageBox.Show(
                LocalizedStrings.Instance["InputErrorMessageBoxText"],
                LocalizedStrings.Instance["InputErrorMessageBoxTitle"],
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        UpdatePlacesInRoute();
        _routesRepository.Update(Route);
        _routesRepository.SaveChanges();

        Navigation.NavigateTo<RoutesViewModel>();
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