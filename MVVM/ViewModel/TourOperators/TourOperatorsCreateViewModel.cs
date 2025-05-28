using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.MVVM.ViewModel.Penalties;
using Travel_Company.WPF.Resources.Localizations;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.TourOperators
{
    public class TourOperatorsCreateViewModel: Core.ViewModel
    {
        private readonly IRepository<TourOperator, long> _tourOperatorRepository;
        private INavigationService _navigation = null!;

        public RelayCommand CreateCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        public INavigationService Navigation
        {
            get => _navigation;
            set
            {
                _navigation = value;
                OnPropertyChanged();
            }
        }

        private TourOperator _TourOperator = new();
        public TourOperator TourOperator
        {
            get => _TourOperator;
            set
            {
                _TourOperator = value;
                OnPropertyChanged();
            }
        }

        public TourOperatorsCreateViewModel(IRepository<TourOperator, long> tourOperatorRepository,
             INavigationService navigationService)
        {
            CreateCommand = new RelayCommand(
            execute: _ => HandleCreating(),
            canExecute: _ => true);
            CancelCommand = new RelayCommand(
                execute: _ => Navigation.NavigateTo<TourOperatorsViewModel>(),
                canExecute: _ => true);
            _tourOperatorRepository = tourOperatorRepository;
            _navigation = navigationService;
        }
        private void HandleCreating()
        {
            if (!Validator.ValidateTourOperator(TourOperator))
            {
                MessageBox.Show(
                    LocalizedStrings.Instance["InputErrorMessageBoxText"],
                    LocalizedStrings.Instance["InputErrorMessageBoxTitle"],
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _tourOperatorRepository.Insert(TourOperator);
            _tourOperatorRepository.SaveChanges();

            Navigation.NavigateTo<TourOperatorsViewModel>();
        }
    }
}
