using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Data.Dto;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.Employees;

public sealed class EmployeesViewModel : Core.ViewModel
{
    private readonly IRepository<TourGuide, long> _employeesRepository; // Changed int to long

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

    private List<TourGuide> _fetchedEmployees = null!;
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

    private TourGuide _selectedTourGuide = null!;
    public TourGuide SelectedTourGuide
    {
        get => _selectedTourGuide;
        set
        {
            _selectedTourGuide = value;
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
            FilterEmployees();
        }
    }

    private Visibility _isShowFiredEmployeesButtonVisible = Visibility.Visible;
    public Visibility IsShowFiredEmployeesButtonVisible
    {
        get => _isShowFiredEmployeesButtonVisible;
        set
        {
            _isShowFiredEmployeesButtonVisible = value;
            OnPropertyChanged();
        }
    }

    private Visibility _isHideFiredEmployeesButtonVisible = Visibility.Collapsed;
    public Visibility IsHideFiredEmployeesButtonVisible
    {
        get => _isHideFiredEmployeesButtonVisible;
        set
        {
            _isHideFiredEmployeesButtonVisible = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand NavigateToEmployeesUpdateCommand { get; set; } = null!;
    public RelayCommand NavigateToEmployeesInsertCommand { get; set; } = null!;
    public RelayCommand FireSelectedEmployeeCommand { get; set; } = null!;
    public RelayCommand DeleteSelectedEmployeeCommand { get; set; } = null!;
    public RelayCommand ShowFiredEmployeesCommand { get; set; } = null!;
    public RelayCommand HideFiredEmployeesCommand { get; set; } = null!;

    public EmployeesViewModel(IRepository<TourGuide, long> repository, INavigationService navigationService) // Changed int to long
    {
        _employeesRepository = repository;
        Navigation = navigationService;
        UpdateWithNotFiredEmployees();
        InitializeCommands();

        App.EventAggregator.Subscribe<TourGuideMessage>(HandleEmployeeMessage);
    }

    private void FilterEmployees()
    {
        if (string.IsNullOrWhiteSpace(SearchText) && IsHideFiredEmployeesButtonVisible == Visibility.Collapsed)
        {
            Employees = _fetchedEmployees
                .Where(e => !e.IsFired) // Simplified comparison
                .ToList();
        }
        if (!string.IsNullOrWhiteSpace(SearchText) && IsHideFiredEmployeesButtonVisible == Visibility.Collapsed)
        {
            Employees = _fetchedEmployees
                .Where(e => (e.Person.FirstName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                             e.Person.LastName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) && !e.IsFired)
                .ToList();
        }
        if (string.IsNullOrWhiteSpace(SearchText) && IsHideFiredEmployeesButtonVisible == Visibility.Visible)
        {
            Employees = _fetchedEmployees
                .ToList();
        }
        if (!string.IsNullOrWhiteSpace(SearchText) && IsHideFiredEmployeesButtonVisible == Visibility.Visible)
        {
            Employees = _fetchedEmployees
                .Where(e => e.Person.FirstName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                            e.Person.LastName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    private void InitializeCommands()
    {
        NavigateToEmployeesUpdateCommand = new RelayCommand(
            execute: _ =>
            {
                if (SelectedTourGuide is not null)
                {
                    var message = new TourGuideMessage { TourGuide = SelectedTourGuide };
                    App.EventAggregator.Publish(message);
                    Navigation.NavigateTo<EmployeesUpdateViewModel>();
                }
            },
            canExecute: _ => true);

        NavigateToEmployeesInsertCommand = new RelayCommand(
                   execute: _ =>
                   {
                       Navigation.NavigateTo<EmployeesCreateViewModel>();
                   },
                   canExecute: _ => true);

        FireSelectedEmployeeCommand = new RelayCommand(
            execute: _ =>
            {
                if (SelectedTourGuide is not null)
                {
                    SelectedTourGuide.IsFired = true;
                    SelectedTourGuide.FiredDate = DateTime.Now;
                    _employeesRepository.Update(SelectedTourGuide); // Added explicit Update call
                    _employeesRepository.SaveChanges();

                    if (IsHideFiredEmployeesButtonVisible == Visibility.Collapsed)
                    {
                        UpdateWithNotFiredEmployees();
                    }
                    else
                    {
                        UpdateWithAllEmployees();
                    }
                }
            },
            canExecute: _ => true);

        DeleteSelectedEmployeeCommand = new RelayCommand(
            execute: _ =>
            {
                if (SelectedTourGuide is not null && SelectedTourGuide.IsFired &&
                    SelectedTourGuide.FiredDate < DateTime.Now.AddYears(-5))
                {
                    _employeesRepository.Delete(SelectedTourGuide);
                    _employeesRepository.SaveChanges();
                    UpdateWithNotFiredEmployees();
                }
            },
            canExecute: _ => true);

        ShowFiredEmployeesCommand = new RelayCommand(
            execute: _ =>
            {
                IsShowFiredEmployeesButtonVisible = Visibility.Collapsed;
                IsHideFiredEmployeesButtonVisible = Visibility.Visible;
                UpdateWithAllEmployees();
            },
            canExecute: _ => true);

        HideFiredEmployeesCommand = new RelayCommand(
            execute: _ =>
            {
                IsShowFiredEmployeesButtonVisible = Visibility.Visible;
                IsHideFiredEmployeesButtonVisible = Visibility.Collapsed;
                UpdateWithNotFiredEmployees();
            },
            canExecute: _ => true);
    }

    private void UpdateWithAllEmployees()
    {
        _fetchedEmployees = _employeesRepository
            .GetQuaryable() // Fixed typo
            .Include(e => e.Person)
            .ThenInclude(p => p.Street)
            .ToList();
        Employees = _fetchedEmployees;
    }

    private void UpdateWithNotFiredEmployees()
    {
        _fetchedEmployees = _employeesRepository
            .GetQuaryable() // Fixed typo
            .Include(e => e.Person)
            .ThenInclude(p => p.Street)
            .Where(e => !e.IsFired)
            .ToList();
        Employees = _fetchedEmployees;
    }

    private void HandleEmployeeMessage(TourGuideMessage message)
    {
        var employee = Employees.FirstOrDefault(x => x.Id == message.TourGuide.Id);
        if (employee != null)
        {
            employee.Salary = message.TourGuide.Salary;
            employee.IsFired = message.TourGuide.IsFired;
            employee.FiredDate = message.TourGuide.FiredDate;
            employee.Person.FirstName = message.TourGuide.Person.FirstName;
            employee.Person.LastName = message.TourGuide.Person.LastName;
            employee.Person.Patronymic = message.TourGuide.Person.Patronymic;
            employee.Person.Birthdate = message.TourGuide.Person.Birthdate;
            employee.Person.StreetId = message.TourGuide.Person.StreetId;
        }

        _fetchedEmployees = _employeesRepository
            .GetQuaryable() // Fixed typo
            .Include(e => e.Person)
            .ThenInclude(p => p.Street)
            .ToList();
        Employees = _fetchedEmployees;
        OnPropertyChanged(nameof(Employees));
    }
}