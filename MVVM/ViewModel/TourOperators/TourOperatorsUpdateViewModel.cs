using System;
using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Resources.Localizations;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.TourOperators
{
    public class TourOperatorsUpdateViewModel : Core.ViewModel
    {
        private readonly IRepository<TourOperator, long> _tourOperatorRepository;
        private INavigationService _navigation = null!;

        public RelayCommand UpdateCommand { get; set; }
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

        private TourOperator _tourOperator = new();
        public TourOperator TourOperator
        {
            get => _tourOperator;
            set
            {
                _tourOperator = value;
                OnPropertyChanged();
            }
        }

        public TourOperatorsUpdateViewModel(IRepository<TourOperator, long> tourOperatorRepository,
            INavigationService navigationService)
        {
            _tourOperatorRepository = tourOperatorRepository;
            _navigation = navigationService;

            // Subscribe to message for loading existing tour operator
            App.EventAggregator.Subscribe<TourOperatorMessage>(HandleStartupMessage);

            UpdateCommand = new RelayCommand(
                execute: _ => HandleUpdating(),
                canExecute: _ => true);
            CancelCommand = new RelayCommand(
                execute: _ => Navigation.NavigateTo<TourOperatorsViewModel>(),
                canExecute: _ => true);
        }

        private void HandleStartupMessage(TourOperatorMessage message)
        {
            TourOperator = message.TourOperator ?? new TourOperator();
        }

        private void HandleUpdating()
        {
            try
            {
                if (!Validator.ValidateTourOperator(TourOperator))
                {
                    MessageBox.Show(
                        LocalizedStrings.Instance["InputErrorMessageBoxText"],
                        LocalizedStrings.Instance["InputErrorMessageBoxTitle"],
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _tourOperatorRepository.Update(TourOperator);
                _tourOperatorRepository.SaveChanges();

                Navigation.NavigateTo<TourOperatorsViewModel>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating tour operator: {ex.Message}", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    // Define TourOperatorMessage for navigation
    public class TourOperatorMessage
    {
        public TourOperator TourOperator { get; set; }
    }
}