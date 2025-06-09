using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.Catalogs
{
    public class CatalogsUpdateViewModel : Core.ViewModel
    {
        private readonly IRepository<ICatalogItem, long> _repository;
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

        private ICatalogItem _catalogItem = null!;
        public ICatalogItem CatalogItem
        {
            get => _catalogItem;
            set
            {
                _catalogItem = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand UpdateCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        public CatalogsUpdateViewModel(IRepository<ICatalogItem, long> repository, INavigationService navigationService)
        {
            _repository = repository;
            _navigation = navigationService;

            UpdateCommand = new RelayCommand(
                execute: _ => { /* Логика обновления */ },
                canExecute: _ => true);
            CancelCommand = new RelayCommand(
                execute: _ => Navigation.NavigateTo<CatalogsViewModel>(),
                canExecute: _ => true);
        }
    }
}