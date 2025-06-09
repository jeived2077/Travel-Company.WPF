using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Travel_Company.WPF.Core;
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
    private readonly IRepository<Client, long> _clientsRepository; // Изменен с Guid на long

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

    public RelayCommand UpdateCommand { get; private set; } = null!;
    public RelayCommand CancelCommand { get; private set; } = null!;

    public PenaltiesUpdateViewModel(
        IRepository<Penalty, long> penaltiesRepo,
        IRepository<TourGuide, long> employeesRepo,
        IRepository<Client, long> clientsRepo, // Изменен с Guid на long
        INavigationService navigationService)
    {
        _penaltiesRepository = penaltiesRepo ?? throw new ArgumentNullException(nameof(penaltiesRepo));
        _clientsRepository = clientsRepo ?? throw new ArgumentNullException(nameof(clientsRepo));
        _employeesRepository = employeesRepo ?? throw new ArgumentNullException(nameof(employeesRepo));
        Navigation = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        Clients = _clientsRepository.GetQuaryable()
            .Include(c => c.Person)
            .AsNoTracking()
            .ToList() ?? new List<Client>();

        Employees = _employeesRepository.GetQuaryable()
            .Include(tg => tg.Person)
            .AsNoTracking()
            .ToList() ?? new List<TourGuide>();

        App.EventAggregator.Subscribe<PenaltyMessage>(HandleStartupMessage);

        InitializeCommands();
    }

    private void InitializeCommands()
    {
        UpdateCommand = new RelayCommand(
            execute: _ => HandleUpdating(),
            canExecute: _ => Validator.ValidatePenalty(Penalty).IsValid);
        CancelCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<PenaltiesViewModel>(),
            canExecute: _ => true);
    }

    private void HandleUpdating()
    {
        var (isValid, errors) = Validator.ValidatePenalty(Penalty);
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
            _penaltiesRepository.Update(Penalty);
            _penaltiesRepository.SaveChanges();

            MessageBox.Show("Penalty updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Navigation.NavigateTo<PenaltiesViewModel>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating penalty: {ex.Message}", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void HandleStartupMessage(PenaltyMessage message)
    {
        Penalty = _penaltiesRepository.GetQuaryable()
            .Include(p => p.Client).ThenInclude(c => c!.Person)
            .Include(p => p.TourGuide).ThenInclude(tg => tg!.Person)
            .FirstOrDefault(p => p.Id == message.Penalty.Id) ?? message.Penalty;
    }
}