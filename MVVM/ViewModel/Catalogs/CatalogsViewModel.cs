using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Core.Enums;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Data.Dto;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.Catalogs;

public class CatalogsViewModel : Core.ViewModel
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

    private Visibility _isClassColumnVisible = Visibility.Collapsed;
    public Visibility IsClassColumnVisible
    {
        get => _isClassColumnVisible;
        private set
        {
            _isClassColumnVisible = value;
            OnPropertyChanged();
        }
    }

    private Visibility _isCountryColumnVisible = Visibility.Collapsed;
    public Visibility IsCountryColumnVisible
    {
        get => _isCountryColumnVisible;
        private set
        {
            _isCountryColumnVisible = value;
            OnPropertyChanged();
        }
    }

    private CatalogType _catalogType = CatalogType.None;
    public CatalogType CatalogType
    {
        get => _catalogType;
        set
        {
            _catalogType = value;
            OnPropertyChanged();
        }
    }

    private ICatalogItem _selectedCatalogItem = null!;
    public ICatalogItem SelectedCatalogItem
    {
        get => _selectedCatalogItem;
        set
        {
            _selectedCatalogItem = value;
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
            FilterCatalog();
        }
    }

    private string _pageTitle = string.Empty;
    public string PageTitle
    {
        get => _pageTitle;
        set
        {
            _pageTitle = value;
            OnPropertyChanged();
            FilterCatalog();
        }
    }

    private List<ICatalogItem> _fetchedCatalogList = null!;
    private List<ICatalogItem> _catalogItems = null!;
    public List<ICatalogItem> CatalogItems
    {
        get => _catalogItems;
        set
        {
            _catalogItems = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand NavigateToUpdatingCommand { get; set; }
    public RelayCommand NavigateToInsertingCommand { get; set; }
    public RelayCommand DeleteSelectedItemCommand { get; set; }

    public CatalogsViewModel(
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

        App.EventAggregator.Subscribe<CatalogTypeMessage>(HandleCatalogTypeMessage);
        GetCatalog();
        SetCatalog();

        NavigateToUpdatingCommand = new RelayCommand(
            execute: _ => HandleUpdatingCatalogItem(),
            canExecute: _ => true);

        NavigateToInsertingCommand = new RelayCommand(
            execute: _ => HandleInsertingCatalogItem(),
            canExecute: _ => true);

        DeleteSelectedItemCommand = new RelayCommand(
            execute: _ => HandleDeletingCatalogItem(),
            canExecute: _ => true);
    }

    private void FilterCatalog()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            CatalogItems = _fetchedCatalogList;
        }
        else
        {
            CatalogItems = _fetchedCatalogList
                .Where(item => item.Name.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase))
                .ToList();
        }
    }

    private void HandleUpdatingCatalogItem()
    {
        if (SelectedCatalogItem is not null)
        {
            var message = new CatalogItemMessage
            {
                CatalogItem = _selectedCatalogItem,
                CatalogType = _catalogType,
            };
            App.EventAggregator.Publish(message);
            Navigation.NavigateTo<CatalogsUpdateViewModel>();
        }
    }

    private void HandleInsertingCatalogItem()
    {
        var message = new CatalogItemMessage { CatalogType = _catalogType };
        App.EventAggregator.Publish(message);
        Navigation.NavigateTo<CatalogsCreateViewModel>();
    }

    private void HandleDeletingCatalogItem()
    {
        if (SelectedCatalogItem is not null)
        {
            switch (_catalogType)
            {
                case CatalogType.Country:
                    _countriesRepository.Delete((SelectedCatalogItem as Country)!);
                    break;
                case CatalogType.Street:
                    _streetsRepository.Delete((SelectedCatalogItem as Street)!);
                    break;
                case CatalogType.Hotel:
                    _hotelsRepository.Delete((SelectedCatalogItem as Hotel)!);
                    break;
                case CatalogType.Place:
                    _placesRepository.Delete((SelectedCatalogItem as PopulatedPlace)!);
                    break;
            }
        }
        try
        { 
            _countriesRepository.SaveChanges();
        } 
        catch 
        {
            MessageBox.Show("Удаляемая страна используется в одном из маршрутов");
        }
        GetCatalog();
        SetCatalog();
    }

    private void HandleCatalogTypeMessage(CatalogTypeMessage message)
    {
        _catalogType = message.CatalogType;
    }

    private void GetCatalog()
    {
        switch (_catalogType)
        {
            case CatalogType.Country:
                _fetchedCatalogList = new(_countriesRepository.GetAll());
                PageTitle = "Countries";
                break;
            case CatalogType.Street:
                _fetchedCatalogList = new(_streetsRepository.GetAll());
                PageTitle = "Streets";
                break;
            case CatalogType.Hotel:
                _fetchedCatalogList = new(_hotelsRepository.GetAll());
                IsClassColumnVisible = Visibility.Visible;
                PageTitle = "Hotels";
                break;
            case CatalogType.Place:
                _fetchedCatalogList = new(_placesRepository.GetQuaryable().Include(p => p.Country));
                IsCountryColumnVisible = Visibility.Visible;
                PageTitle = "Populated Places";
                break;
        }
    }

    private void SetCatalog()
    {
        CatalogItems = _fetchedCatalogList;
    }
}