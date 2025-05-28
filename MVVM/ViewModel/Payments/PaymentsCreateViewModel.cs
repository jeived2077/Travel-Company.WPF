using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.MVVM.ViewModel.Penalties;
using Travel_Company.WPF.Resources.Localizations;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.Payments
{
    public class PaymentsCreateViewModel: Core.ViewModel
    {
        private readonly IRepository<Models.Payments, long> _paymentsRepository;
        private readonly IRepository<TourGuide, int> _employeesRepository;
        private readonly IRepository<Client, long> _clientsRepository;
        private readonly IRepository<Route, long> _routesRepository;


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

        private Models.Payments _payment = new();
        public Models.Payments Payment
        {
            get => _payment;
            set
            {
                _payment = value;
                OnPropertyChanged();
            }
        }

        private List<Route> _routes = null!;
        public List<Route> Routes
        {
            get => _routes;
            set
            {
                _routes = value;
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

        public PaymentsCreateViewModel(
            IRepository<Models.Payments, long> paymentsRepo,
            IRepository<TourGuide, int> employeesRepo,
            IRepository<Client, long> clientsRepo,
            IRepository<Route, long> routeRepo,

            INavigationService navigationService)
        {
            _paymentsRepository = paymentsRepo;
            _routesRepository = routeRepo;
            _clientsRepository = clientsRepo;
            _employeesRepository = employeesRepo;
            Navigation = navigationService;

            InitializeData();

            CreateCommand = new RelayCommand(
                execute: _ => HandleCreating(),
                canExecute: _ => true);
            CancelCommand = new RelayCommand(
                execute: _ => Navigation.NavigateTo<PaymentsViewModel>(),
                canExecute: _ => true);
        }

        private void InitializeData()
        {
            Clients = _clientsRepository.GetAll();
            Employees = _employeesRepository.GetAll();
            Routes = _routesRepository.GetAll();
            Payment.PaymentDate = DateTime.Now;
        }

        private void HandleCreating()
        {
            if (Payment.Route is not null)
            {
                Payment.PaymentMethod = "Ожидает оплаты";
                Payment.Status = "Ожидает оплаты";
                _paymentsRepository.Insert(Payment);
                _paymentsRepository.SaveChanges();
                Navigation.NavigateTo<PaymentsViewModel>();
            }

            else MessageBox.Show("Некорректные данные.");
        }
    }
}
