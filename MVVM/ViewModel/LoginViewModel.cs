﻿using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Core.Enums;
using Travel_Company.WPF.Data.Dto;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.MVVM.ViewModel.Clients;
using Travel_Company.WPF.MVVM.ViewModel.Employees;
using Travel_Company.WPF.Resources.Localizations;
using Travel_Company.WPF.Services.Authorization;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel;

public sealed class LoginViewModel : Core.ViewModel
{
    private readonly IAuthorizationService _authorizationService;

    private string _username = App.Settings.UserName;
    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged();
        }
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
        }
    }

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

    public RelayCommand LogInCommand { get; set; }

    public LoginViewModel(
        INavigationService navigationService, IAuthorizationService authorizationService)
    {
        Navigation = navigationService;
        _authorizationService = authorizationService;

        LogInCommand = new RelayCommand(
            execute: _ => HandleAuthorization(),
            canExecute: _ => true);
    }

    private void HandleAuthorization()
    {
        var (user, role) = _authorizationService.LogIn(Username, Password);
        if (user != null)
        {
            SaveAuthorizedUserData(user, role);

            var message = new SuccessLoginMessage();
            App.EventAggregator.Publish(message);

            if (role == UserRole.Admin || role == UserRole.Employee)
            {
                Navigation.NavigateTo<EmployeesViewModel>();
            }
            else
            {
                Navigation.NavigateTo<ClientsViewModel>();
            }
        }
        else
        {
            MessageBox.Show(
                LocalizedStrings.Instance["InputErrorMessageBoxText"],
                LocalizedStrings.Instance["InputErrorMessageBoxTitle"],
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static void SaveAuthorizedUserData(User user, UserRole role)
    {
        App.Settings.User = user;
        App.Settings.UserName = user.Username;
        App.Settings.UserRole = role;
    }
}