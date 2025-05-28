using System.Collections.Generic;
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
    private readonly IRepository<TourGuide, int> _employeesRepository = null!;

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

    private TourGuide _employee = null!;
    public TourGuide Employee
    {
        get => _employee;
        set
        {
            _employee = value;
            OnPropertyChanged();
        }
    }

    public TourGuide EmployeeOld
    {
        get;
        set;
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

    public RelayCommand UpdateEmployeeCommand { get; set; }
    public RelayCommand CancelCommand { get; set; }

    public EmployeesUpdateViewModel(
        IRepository<Street, long> streetsRepo,
        IRepository<TourGuide, int> employeesRepo,
        INavigationService navigationService)
    {
        _streetsRepository = streetsRepo;
        _employeesRepository = employeesRepo;
        Streets = _streetsRepository.GetAll();
        App.EventAggregator.Subscribe<TourGuideMessage>(HandleEmployeeMessage);

        Navigation = navigationService;
        EmployeeOld = new TourGuide
        {
            Id = Employee.Id,
            FirstName = Employee.FirstName,
            LastName = Employee.LastName,
            Birthdate = Employee.Birthdate,
            Salary = Employee.Salary,
            Patronymic = Employee.Patronymic
        };

        UpdateEmployeeCommand = new RelayCommand(
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

                _employeesRepository.Update(Employee);
                _employeesRepository.SaveChanges();

                Navigation.NavigateTo<EmployeesViewModel>();
            },
            canExecute: _ => true);

        CancelCommand = new RelayCommand(
            execute: _ => 
            {
                Employee = new TourGuide
                {
                    Id = EmployeeOld.Id,
                    FirstName = EmployeeOld.FirstName,
                    LastName = EmployeeOld.LastName,
                    Birthdate = EmployeeOld.Birthdate,
                    Salary = EmployeeOld.Salary,
                    Patronymic = EmployeeOld.Patronymic
                };
                
                OnPropertyChanged(nameof(Employee));
                var msg = new TourGuideMessage
                {
                    TourGuide = Employee,
                };
                App.EventAggregator.Publish(msg);

               Navigation.NavigateTo<EmployeesViewModel>();
             },
            canExecute: _ => true);
    }

    private void HandleEmployeeMessage(TourGuideMessage message)
    {
        Employee = message.TourGuide!;
    }
}