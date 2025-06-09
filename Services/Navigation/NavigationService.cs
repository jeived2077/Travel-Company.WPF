using System;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.MVVM.ViewModel;

namespace Travel_Company.WPF.Services.Navigation;

public class NavigationService : ObservableObject, INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Func<Type, ViewModel> _viewModelFactory;

    private ViewModel _currentView;
    public ViewModel CurrentView
    {
        get => _currentView;
        private set
        {
            _currentView = value;
            OnPropertyChanged();
        }
    }

    public NavigationService(IServiceProvider serviceProvider, Func<Type, ViewModel> viewModelFactory)
    {
        _serviceProvider = serviceProvider;
        _viewModelFactory = viewModelFactory;
    }

    public void Initialize()
    {
        NavigateTo<LoginViewModel>(); // Устанавливаем начальный вид
    }

    public void NavigateTo<TViewModel>() where TViewModel : ViewModel
    {
        CurrentView = _viewModelFactory.Invoke(typeof(TViewModel));
    }
}