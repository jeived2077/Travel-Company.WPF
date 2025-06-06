using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Data.Dto;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Resources.Localizations;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.Penalties;

public class PenaltiesUpdateViewModel : Core.ViewModel
{
    private readonly IRepository<Penalty, long> _penaltiesRepository;
    private readonly IRepository<TourGuide, long> _employeesRepository;
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

    private Penalty _penalty = null!;
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

    public RelayCommand UpdateCommand { get; set; }
    public RelayCommand CancelCommand { get; set; }

    public PenaltiesUpdateViewModel(
        IRepository<Penalty, long> penaltiesRepo,
        IRepository<TourGuide, long> employeesRepo,
        IRepository<Client, long> clientsRepo,
        INavigationService navigationService)
    {
        _penaltiesRepository = penaltiesRepo;
        _clientsRepository = clientsRepo;
        _employeesRepository = employeesRepo;
        Navigation = navigationService;

        Clients = _clientsRepository.GetQuaryable() // Fixed typo
            .Include(c => c.Person)
            .ToList();

        Employees = _employeesRepository.GetQuaryable() // Fixed typo
            .Include(tg => tg.Person)
            .ToList();

        App.EventAggregator.Subscribe<PenaltyMessage>(HandleStartupMessage);

        UpdateCommand = new RelayCommand(
            execute: _ => HandleUpdating(),
            canExecute: _ => true);
        CancelCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<PenaltiesViewModel>(),
            canExecute: _ => true);
    }

    private void HandleUpdating()
    {
        if (!Validator.ValidatePenalty(Penalty))
        {
            MessageBox.Show(
                LocalizedStrings.Instance["InputErrorMessageBoxText"],
                LocalizedStrings.Instance["InputErrorMessageBoxTitle"],
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        _penaltiesRepository.Update(Penalty);
        _penaltiesRepository.SaveChanges();

        Navigation.NavigateTo<PenaltiesViewModel>();
    }

    private void HandleStartupMessage(PenaltyMessage message)
    {
        Penalty = _penaltiesRepository.GetQuaryable() // Fixed typo
            .Include(p => p.Client)
            .Include(p => p.TourGuide)
            .FirstOrDefault(p => p.Id == message.Penalty.Id) ?? message.Penalty;

        // Debug check to ensure Amount and Description are loaded
        if (Penalty.Amount == 0 && string.IsNullOrEmpty(Penalty.Reason))
        {
            System.Diagnostics.Debug.WriteLine($"Warning: Penalty {Penalty.Id} has Amount=0 and/or empty Description.");
        }
    }
}