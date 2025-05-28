using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Data.Dto;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.Routes;

public class RoutesViewModel : Core.ViewModel
{
    private readonly IRepository<Route, long> _routesRepository;

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

    private List<Route> _fetchedRoutes;
    private List<Route> _routes = null!;
    public List<Route> Routes
    {
        get => _routes;
        set
        {
            _routes = value;
            OnPropertyChanged();
        }
    }

    private Route _selectedItem = null!;
    public Route SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
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

    private void FilterItems()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Routes = _fetchedRoutes.ToList();
            return;
        }

        Routes = _fetchedRoutes
            .Where(c => c.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public RelayCommand NavigateToUpdatingCommand { get; set; } = null!;
    public RelayCommand NavigateToInsertingCommand { get; set; } = null!;
    public RelayCommand DeleteSelectedItemCommand { get; set; } = null!;

    public RoutesViewModel(IRepository<Route, long> routesRepository, INavigationService navigation)
    {
        _routesRepository = routesRepository;
        _navigation = navigation;

        _fetchedRoutes = FetchDataGridData();
        Routes = _fetchedRoutes;

        InitializeCommands();
    }

    private List<Route> FetchDataGridData() => _routesRepository
        .GetQuaryable()
        .Include(r => r.Country)
        .Include(r => r.TouristGroups)
        .Include(r => r.RoutesPopulatedPlaces)
        .ToList();

    private void InitializeCommands()
    {
        NavigateToUpdatingCommand = new RelayCommand(
            execute: _ => HandleUpdating(),
            canExecute: _ => true);
        NavigateToInsertingCommand = new RelayCommand(
           execute: _ => Navigation.NavigateTo<RoutesCreateViewModel>(),
           canExecute: _ => true);
        DeleteSelectedItemCommand = new RelayCommand(
            execute: _ => HandleDeleting(),
            canExecute: _ => true);
    }
    private void HandleUpdating()
    {
        if (SelectedItem is not null)
        {
            var message = new RouteMessage { Route = SelectedItem };
            App.EventAggregator.Publish(message);
            Navigation.NavigateTo<RoutesUpdateViewModel>();
        }
    }

    private void HandleDeleting()
    {
        if (SelectedItem is not null)
        {
            _routesRepository.Delete(SelectedItem);
            _routesRepository.SaveChanges();

            _fetchedRoutes = FetchDataGridData();
            Routes = _fetchedRoutes;
        }
    }
}