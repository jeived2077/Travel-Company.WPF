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

namespace Travel_Company.WPF.MVVM.ViewModel.Employees;

public sealed class EmployeesUpdateViewModel : Core.ViewModel
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

    private TourGuide _employee = new TourGuide { Person = new Person() }; // Initialize to avoid null reference
    public TourGuide Employee
    {
        get => _employee;
        set
        {
            _employee = value;
            OnPropertyChanged();
        }
    }

    public TourGuide EmployeeOld { get; set; } = new TourGuide { Person = new Person() }; // Initialize with Person

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

    public RelayCommand UpdateEmployeeCommand { get; set; }
    public RelayCommand CancelCommand { get; set; }

    public EmployeesUpdateViewModel(
        IRepository<Street, long> streetsRepo,
        IRepository<TourGuide, long> employeesRepo,
        INavigationService navigationService)
    {
        _streetsRepository = streetsRepo;
        _employeesRepository = employeesRepo;
        Navigation = navigationService;

        Streets = _streetsRepository.GetAll().ToList(); // Добавлено .ToList()

        App.EventAggregator.Subscribe<TourGuideMessage>(HandleEmployeeMessage);

        UpdateEmployeeCommand = new RelayCommand(
    execute: _ =>
    {
        var validationResult = Validator.ValidateTourGuide(Employee);
        if (!validationResult.IsValid)
        {
            MessageBox.Show(
                string.Join("\n", validationResult.Errors),
                LocalizedStrings.Instance["InputErrorMessageBoxTitle"],
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        _employeesRepository.Update(Employee);
        _employeesRepository.SaveChanges();

        var msg = new TourGuideMessage { TourGuide = Employee };
        App.EventAggregator.Publish(msg);
        Navigation.NavigateTo<EmployeesViewModel>();
    },
    canExecute: _ => true);

        CancelCommand = new RelayCommand(
            execute: _ =>
            {
                Employee = new TourGuide
                {
                    Id = EmployeeOld.Id,
                    Salary = EmployeeOld.Salary,
                    IsFired = EmployeeOld.IsFired,
                    FiredDate = EmployeeOld.FiredDate,
                    Person = new Person
                    {
                        Id = EmployeeOld.Person.Id,
                        FirstName = EmployeeOld.Person.FirstName,
                        LastName = EmployeeOld.Person.LastName,
                        Patronymic = EmployeeOld.Person.Patronymic,
                        Birthdate = EmployeeOld.Person.Birthdate,
                        StreetId = EmployeeOld.Person.StreetId
                    }
                };

                OnPropertyChanged(nameof(Employee));
                var msg = new TourGuideMessage { TourGuide = Employee };
                App.EventAggregator.Publish(msg);
                Navigation.NavigateTo<EmployeesViewModel>();
            },
            canExecute: _ => true);
    }

    private void HandleEmployeeMessage(TourGuideMessage message)
    {
        Employee = message.TourGuide;
        // Deep copy for EmployeeOld to preserve original data
        EmployeeOld = new TourGuide
        {
            Id = Employee.Id,
            Salary = Employee.Salary,
            IsFired = Employee.IsFired,
            FiredDate = Employee.FiredDate,
            Person = new Person
            {
                Id = Employee.Person.Id,
                FirstName = Employee.Person.FirstName,
                LastName = Employee.Person.LastName,
                Patronymic = Employee.Person.Patronymic,
                Birthdate = Employee.Person.Birthdate,
                StreetId = Employee.Person.StreetId
            }
        };
    }
}