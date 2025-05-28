using System.CodeDom.Compiler;
using System.Globalization;
using System.Linq;
using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Core.Enums;
using Travel_Company.WPF.Data.Dto;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.MVVM.ViewModel.Catalogs;
using Travel_Company.WPF.MVVM.ViewModel.Clients;
using Travel_Company.WPF.MVVM.ViewModel.Employees;
using Travel_Company.WPF.MVVM.ViewModel.Groups;
using Travel_Company.WPF.MVVM.ViewModel.Payments;
using Travel_Company.WPF.MVVM.ViewModel.Penalties;
using Travel_Company.WPF.MVVM.ViewModel.Reports;
using Travel_Company.WPF.MVVM.ViewModel.Routes;
using Travel_Company.WPF.MVVM.ViewModel.TourOperators;
using Travel_Company.WPF.Services.Navigation;
using WPFLocalizeExtension.Engine;

namespace Travel_Company.WPF.MVVM.ViewModel;

public sealed class MainViewModel : Core.ViewModel
{
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

    private Visibility _mainMenuVisibility;
    public Visibility MainMenuVisibility
    {
        get => _mainMenuVisibility;
        set
        {
            _mainMenuVisibility = value;
            OnPropertyChanged();
        }
    }

    private Visibility _isEmployeeButtonVisible;
    public Visibility IsEmployeeButtonVisible
    {
        get => _isEmployeeButtonVisible;
        set
        {
            _isEmployeeButtonVisible = value;
            OnPropertyChanged();
        }
    }
    
    // Pages
    public RelayCommand NavigateToEmployeesCommand { get; set; } = null!;
    public RelayCommand NavigateToClientsCommand { get; set; } = null!;
    public RelayCommand NavigateToReportsCommand { get; set; } = null!;
    public RelayCommand NavigateToPaymentsCommand { get; set; } = null!;
    public RelayCommand NavigateToPenaltiesCommand { get; set; } = null!;
    public RelayCommand NavigateToTouristGroupsCommand { get; set; } = null!;
    public RelayCommand NavigateToRoutesCommand { get; set; } = null!;
    public RelayCommand NavigateToTourOperatorsCommand { get; set; } = null!;

    // Catalogs
    public RelayCommand NavigateToCountriesCommand { get; set; } = null!;
    public RelayCommand NavigateToStreetsCommand { get; set; } = null!;
    public RelayCommand NavigateToHotelsCommand { get; set; } = null!;
    public RelayCommand NavigateToPopulatedPlacesCommand { get; set; } = null!;
    // Localization
    public RelayCommand SwitchLocalizationCommand { get; set; } = null!;

    public MainViewModel(INavigationService service)
    {
        Navigation = service;
        MainMenuVisibility = Visibility.Collapsed;
        InitializePagesCommands();
        InitializeCatalogsCommands();
        SwitchLocalizationCommand = new RelayCommand(
            execute: _ =>
            {
                LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
                if (LocalizeDictionary.Instance.Culture.Name == "ru-RU")
                {
                    LocalizeDictionary.Instance.Culture = new CultureInfo("en-US");
                }
                else if (LocalizeDictionary.Instance.Culture.Name == "en-US")
                {
                    LocalizeDictionary.Instance.Culture = new CultureInfo("ru-RU");
                }
            },
            canExecute: _ => true);

        if (!App.Settings.IsAuthorized)
        {
            App.EventAggregator.Subscribe<SuccessLoginMessage>(HandleStartupMessage);
        }
    }

    private void HandleStartupMessage(SuccessLoginMessage message)
    {
        App.Settings.IsAuthorized = true;
        App.EventAggregator.RemoveMessage<SuccessLoginMessage>();
        MainMenuVisibility = Visibility.Visible;
        SetupMainWindowBehavior();
    }

    private void SetupMainWindowBehavior()
    {
        var rights = App.Settings.User!.UsersObjects.First(u => u.Object.Name == "Employees");
        if (!rights.CanRead)
        {
            IsEmployeeButtonVisible = Visibility.Collapsed;
        }
    }

    private void InitializePagesCommands()
    {
        NavigateToEmployeesCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<EmployeesViewModel>(),
            canExecute: _ => true);
        NavigateToClientsCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<ClientsViewModel>(),
            canExecute: _ => true);
        NavigateToReportsCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<ReportsViewModel>(),
            canExecute: _ => true);
        NavigateToPenaltiesCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<PenaltiesViewModel>(),
            canExecute: _ => true);
        NavigateToTouristGroupsCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<GroupsViewModel>(),
            canExecute: _ => true);
        NavigateToRoutesCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<RoutesViewModel>(),

            canExecute: _ => true);
        NavigateToPaymentsCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<PaymentsViewModel>(),
            canExecute: _ => true
            );
        NavigateToTourOperatorsCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<TourOperatorsViewModel>(),
            canExecute: _ => true
            );
    }

    private void InitializeCatalogsCommands()
    {
        NavigateToCountriesCommand = CreateNavigationCommand(CatalogType.Country);
        NavigateToStreetsCommand = CreateNavigationCommand(CatalogType.Street);
        NavigateToHotelsCommand = CreateNavigationCommand(CatalogType.Hotel);
        NavigateToPopulatedPlacesCommand = CreateNavigationCommand(CatalogType.Place);
    }

    private RelayCommand CreateNavigationCommand(CatalogType catalogType)
    {
        return new RelayCommand(
            execute: _ =>
            {
                var message = new CatalogTypeMessage { CatalogType = catalogType };
                App.EventAggregator.Publish(message);
                Navigation.NavigateTo<CatalogsViewModel>();
            },
            canExecute: _ => true);
    }
}