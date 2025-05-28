using System.Collections.Generic;
using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Core.Enums;
using Travel_Company.WPF.Data;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Data.Dto;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Resources.Localizations;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.Catalogs;

public class CatalogsCreateViewModel : Core.ViewModel
{
    private readonly IRepository<Country, int> _countriesRepository;
    private readonly IRepository<Street, long> _streetsRepository;
    private readonly IRepository<Hotel, long> _hotelsRepository;
    private readonly IRepository<PopulatedPlace, long> _placesRepository;

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

    private CatalogType _catalogType;
    public CatalogType CatalogType
    {
        get => _catalogType;
        set
        {
            _catalogType = value;
            OnPropertyChanged();
        }
    }

    private ICatalogItem _catalogItem = null!;
    public ICatalogItem CatalogItem
    {
        get => _catalogItem;
        set
        {
            _catalogItem = value;
            OnPropertyChanged();
        }
    }

    private Visibility _isClassElementVisible = Visibility.Collapsed;
    public Visibility IsClassElementVisible
    {
        get => _isClassElementVisible;
        private set
        {
            _isClassElementVisible = value;
            OnPropertyChanged();
        }
    }

    private Visibility _isCountryNameElementVisible = Visibility.Collapsed;
    public Visibility IsCountryNameElementVisible
    {
        get => _isCountryNameElementVisible;
        private set
        {
            _isCountryNameElementVisible = value;
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

    public RelayCommand CreateCommand { get; set; }
    public RelayCommand CancelCommand { get; set; }

    public CatalogsCreateViewModel(
        IRepository<Country, int> countriesRepository,
        IRepository<Street, long> streetsRepository,
        IRepository<Hotel, long> hotelsRepository,
        IRepository<PopulatedPlace, long> placesRepository,
        INavigationService navigationService)
    {
        _countriesRepository = countriesRepository;
        _streetsRepository = streetsRepository;
        _hotelsRepository = hotelsRepository;
        _placesRepository = placesRepository;
        _navigation = navigationService;

        App.EventAggregator.Subscribe<CatalogItemMessage>(HandleCatalogItemMessage);

        CreateCommand = new RelayCommand(
            execute: _ => HandleCreation(),
            canExecute: _ => true);

        CancelCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<CatalogsViewModel>(),
            canExecute: _ => true);
    }

    private void HandleCatalogItemMessage(CatalogItemMessage message)
    {
        CatalogType = message.CatalogType;
        switch (_catalogType)
        {
            case CatalogType.Country:
                CatalogItem = new Country();
                break;
            case CatalogType.Street:
                CatalogItem = new Street();
                break;
            case CatalogType.Hotel:
                CatalogItem = new Hotel();
                IsClassElementVisible = Visibility.Visible;
                break;
            case CatalogType.Place:
                CatalogItem = new PopulatedPlace();
                IsCountryNameElementVisible = Visibility.Visible;
                Countries = _countriesRepository.GetAll();
                break;
        }
    }

    private void HandleCreation()
    {
        if (!Validator.ValidateCatalogItem(CatalogItem))
        {
            MessageBox.Show(
                LocalizedStrings.Instance["InputErrorMessageBoxText"],
                LocalizedStrings.Instance["InputErrorMessageBoxTitle"],
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        switch (_catalogType)
        {
            case CatalogType.Country:
                _countriesRepository.Insert((_catalogItem as Country)!);
                break;
            case CatalogType.Street:
                _streetsRepository.Insert((_catalogItem as Street)!);
                break;
            case CatalogType.Hotel:
                _hotelsRepository.Insert((_catalogItem as Hotel)!);
                break;
            case CatalogType.Place:
                _placesRepository.Insert((_catalogItem as PopulatedPlace)!);
                break;
        }
        _placesRepository.SaveChanges();
        Navigation.NavigateTo<CatalogsViewModel>();
    }
}