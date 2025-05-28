using Travel_Company.WPF.Core;

namespace Travel_Company.WPF.Services.Navigation;

public interface INavigationService
{
    ViewModel CurrentView { get; }
    void NavigateTo<T>() where T : ViewModel;
}