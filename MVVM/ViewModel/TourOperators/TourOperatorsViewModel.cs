using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.TourOperators
{
    public class TourOperatorsViewModel : Core.ViewModel
    {
        private readonly IRepository<Models.TourOperator, long> _operatorsRepository;

        public RelayCommand NavigateToInsertingCommand { get; set; } = null!;
        public RelayCommand NavigateToUpdatingCommand { get; set; } = null!;
        public RelayCommand DeleteSelectedItemCommand { get; set; } = null!;
        private List<TourOperator> _fetchedTourOperators;

        private INavigationService _navigation;

        public INavigationService Navigation
        {
            get => _navigation;
            set
            {
                _navigation = value;
                OnPropertyChanged();
            }
        }

        public List<Models.TourOperator> TourOperators
        {
            get => _fetchedTourOperators;
            set
            {
                _fetchedTourOperators = value;
                OnPropertyChanged();
            }
        }

        public Models.TourOperator SelectedItem { get; set; }

        public TourOperatorsViewModel(IRepository<TourOperator, long> operatorsRepository, INavigationService navigationService)
        {
            _operatorsRepository = operatorsRepository;
            TourOperators = FetchDataGridData();
            _navigation = navigationService;

            NavigateToInsertingCommand = new RelayCommand(
                execute: _ => Navigation.NavigateTo<TourOperatorsCreateViewModel>(),
                canExecute: _ => true);

            NavigateToUpdatingCommand = new RelayCommand(
                execute: _ => HandleUpdating(),
                canExecute: _ => SelectedItem != null);

            DeleteSelectedItemCommand = new RelayCommand(
                execute: _ => HandleDeleting(),
                canExecute: _ => true);
        }

        private List<Models.TourOperator> FetchDataGridData()
        {
            return _operatorsRepository
                .GetQuaryable()
                .Include(x => x.Routes)
                .ToList();
        }

        private void HandleUpdating()
        {
            if (SelectedItem != null)
            {
                var message = new TourOperatorMessage { TourOperator = SelectedItem };
                App.EventAggregator.Publish(message);
                Navigation.NavigateTo<TourOperatorsUpdateViewModel>();
            }
        }

        private void HandleDeleting()
        {
            if (SelectedItem != null)
            {
                _operatorsRepository.Delete(SelectedItem);
                _operatorsRepository.SaveChanges();

                _fetchedTourOperators = FetchDataGridData();
                TourOperators = _fetchedTourOperators;
            }
        }
    }
}