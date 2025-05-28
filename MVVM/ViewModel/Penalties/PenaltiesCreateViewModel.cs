using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Resources.Localizations;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.Penalties;

public class PenaltiesCreateViewModel : Core.ViewModel
{
    private readonly IRepository<Penalty, long> _penaltiesRepository;
    private readonly IRepository<TourGuide, int> _employeesRepository;
    private readonly IRepository<Client, long> _clientsRepository;

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

    private Penalty _penalty = new();
    public Penalty Penalty
    {
        get => _penalty;
        set
        {
            _penalty = value;
            OnPropertyChanged();
        }
    }

    private List<Client> _clients = null!;
    public List<Client> Clients
    {
        get => _clients;
        set
        {
            _clients = value;
            OnPropertyChanged();
        }
    }

    private List<TourGuide> _employees = null!;
    public List<TourGuide> Employees
    {
        get => _employees;
        set
        {
            _employees = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand CreateCommand { get; set; }
    public RelayCommand CancelCommand { get; set; }

    public PenaltiesCreateViewModel(
        IRepository<Penalty, long> penaltiesRepo,
        IRepository<TourGuide, int> employeesRepo,
        IRepository<Client, long> clientsRepo,
        INavigationService navigationService)
    {
        _penaltiesRepository = penaltiesRepo;
        _clientsRepository = clientsRepo;
        _employeesRepository = employeesRepo;
        Navigation = navigationService;

        InitializeData();

        CreateCommand = new RelayCommand(
            execute: _ => HandleCreating(),
            canExecute: _ => true);
        CancelCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<PenaltiesViewModel>(),
            canExecute: _ => true);
    }

    private void InitializeData()
    {
        Clients = _clientsRepository.GetAll();
        Employees = _employeesRepository.GetAll();
        Penalty.CompensationDate = DateTime.Now;
    }

    private void HandleCreating()
    {
        if (!Validator.ValidatePenalty(Penalty))
        {
            MessageBox.Show(
                LocalizedStrings.Instance["InputErrorMessageBoxText"],
                LocalizedStrings.Instance["InputErrorMessageBoxTitle"],
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        _penaltiesRepository.Insert(Penalty);
        _penaltiesRepository.SaveChanges();

        Navigation.NavigateTo<PenaltiesViewModel>();
    }
}