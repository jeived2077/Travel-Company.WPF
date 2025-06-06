using System;
using System.Collections.Generic;
using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Resources.Localizations;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.Employees;

public sealed class EmployeesCreateViewModel : Core.ViewModel
{
    private readonly IRepository<Street, long> _streetsRepository = null!;
    private readonly IRepository<TourGuide, long> _employeesRepository = null!;

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

    private TourGuide _employee = new()
    {
        Person = new Person { Birthdate = DateTime.Now.AddYears(-18) } // Initialize Person with Birthdate
    };
    public TourGuide Employee
    {
        get => _employee;
        set
        {
            _employee = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Birthdate));
            OnPropertyChanged(nameof(SelectedStreetId));
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

    // Expose Birthdate as a separate property for easier binding
    public DateTime Birthdate
    {
        get => Employee.Person.Birthdate;
        set
        {
            Employee.Person.Birthdate = value;
            OnPropertyChanged();
        }
    }

    // Expose SelectedStreetId for ComboBox binding, now nullable to match Person.StreetId
    public long? SelectedStreetId
    {
        get => Employee.Person.StreetId;
        set
        {
            Employee.Person.StreetId = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand CreateEmployeeCommand { get; set; }
    public RelayCommand CancelCommand { get; set; }

    public EmployeesCreateViewModel(
        IRepository<Street, long> streetsRepo,
        IRepository<TourGuide, long> employeesRepo,
        INavigationService navigation)
    {
        _streetsRepository = streetsRepo;
        _employeesRepository = employeesRepo;
        Navigation = navigation;

        Streets = _streetsRepository.GetAll() ?? new List<Street>(); // Ensure Streets is never null

        CreateEmployeeCommand = new RelayCommand(
            execute: _ =>
            {
                if (!Validator.ValidateTourGuide(Employee))
                {
                    MessageBox.Show(
                        LocalizedStrings.Instance["InputErrorMessageBoxText"],
                        LocalizedStrings.Instance["InputErrorMessageBoxTitle"],
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _employeesRepository.Insert(Employee);
                _employeesRepository.SaveChanges();

                Navigation.NavigateTo<EmployeesViewModel>();
            },
            canExecute: _ => true);

        CancelCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<EmployeesViewModel>(),
            canExecute: _ => true);
    }
}