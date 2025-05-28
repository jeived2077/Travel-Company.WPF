using System;
using System.Collections.Generic;
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
    private readonly IRepository<Street, long> _streetsRepository = null!;
    private readonly IRepository<TouristGroup, long> _groupsRepository = null!;
    private readonly IRepository<Client, long> _clientsRepository = null!;

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
        Birthdate = DateTime.Now.AddYears(-18),
        PassportIssueDate = DateTime.Now.AddYears(-18)
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

    public RelayCommand CreateCommand { get; set; }
    public RelayCommand CancelCommand { get; set; }
    public RelayCommand ChangeProfilePictureCommand { get; set; }
    public RelayCommand RemoveProfilePictureCommand { get; set; }

    public ClientsCreateViewModel(
        IRepository<Street, long> streetsRepo,
        IRepository<Client, long> clientsRepo,
        IRepository<TouristGroup, long> groupsRepo,
        INavigationService navigationService)
    {
        _streetsRepository = streetsRepo;
        _groupsRepository = groupsRepo;
        _clientsRepository = clientsRepo;
        Navigation = navigationService;

        Streets = _streetsRepository.GetAll();
        Groups = _groupsRepository.GetAll();

        CreateCommand = new RelayCommand(
            execute: _ => HandleCreating(),
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

    private void HandleCreating()
    {
        if (!Validator.ValidateClient(Client))
        {
            MessageBox.Show(
                LocalizedStrings.Instance["InputErrorMessageBoxText"],
                LocalizedStrings.Instance["InputErrorMessageBoxTitle"],
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        _clientsRepository.Insert(Client);
        _clientsRepository.SaveChanges();
        Navigation.NavigateTo<ClientsViewModel>();
    }
}