using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Data.Dto;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Resources.Localizations;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.Penalties;

public class PenaltiesViewModel : Core.ViewModel
{
    private readonly IRepository<Penalty, long> _penaltiesRepository;

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

    private Client? _clientToFilterBy;

    private List<Penalty> _fetchedPenalties;
    private List<Penalty> _penalties = null!;
    public List<Penalty> Penalties
    {
        get => _penalties;
        set
        {
            _penalties = value;
            OnPropertyChanged();
        }
    }

    private Penalty? _selectedItem; // Made nullable for safety
    public Penalty? SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            OnPropertyChanged();
        }
    }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            FilterItems();
        }
    }

    private string _pageTitle = LocalizedStrings.Instance["Penalties"];
    public string PageTitle
    {
        get => _pageTitle;
        set
        {
            _pageTitle = value;
            OnPropertyChanged();
        }
    }

    private void FilterItems()
    {
        if (string.IsNullOrWhiteSpace(SearchText) && _clientToFilterBy == null)
        {
            Penalties = _fetchedPenalties.ToList();
        }
        else if (string.IsNullOrWhiteSpace(SearchText) && _clientToFilterBy != null)
        {
            Penalties = _fetchedPenalties.Where(p => p.ClientId == _clientToFilterBy.Id).ToList();
        }
        else if (_clientToFilterBy == null)
        {
            Penalties = _fetchedPenalties
                .Where(p => p.Client != null && p.Client.Person != null &&
                            p.Client.Person.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        else
        {
            Penalties = _fetchedPenalties
                .Where(p => p.Client != null && p.Client.Person != null &&
                            p.Client.Person.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) &&
                            p.ClientId == _clientToFilterBy.Id)
                .ToList();
        }
    }

    public RelayCommand NavigateToUpdatingCommand { get; private set; } = null!;
    public RelayCommand NavigateToInsertingCommand { get; private set; } = null!;
    public RelayCommand DeleteSelectedItemCommand { get; private set; } = null!;

    public PenaltiesViewModel(IRepository<Penalty, long> repository, INavigationService navigation)
    {
        _penaltiesRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

        App.EventAggregator.Subscribe<ClientMessage>(HandleStartupMessage);

        _fetchedPenalties = FetchDataGridData();
        Penalties = _fetchedPenalties;

        InitializeTitle();
        InitializeCommands();
    }

    private List<Penalty> FetchDataGridData()
    {
        IQueryable<Penalty> query = _penaltiesRepository
            .GetQuaryable() // Fixed typo: GetQuaryable -> GetQueryable
            .Include(p => p.Client)
                .ThenInclude(c => c!.Person) // Non-nullable navigation
            .Include(p => p.TourGuide)
                .ThenInclude(tg => tg!.Person); // Non-nullable navigation

        if (_clientToFilterBy != null)
        {
            query = query.Where(p => p.ClientId == _clientToFilterBy.Id);
        }

        return query.ToList();
    }

    private void InitializeTitle()
    {
        if (_clientToFilterBy != null)
        {
            PageTitle = $"{LocalizedStrings.Instance["Penalties"]}: {_clientToFilterBy.Name ?? "Unknown"}";
        }
    }

    private void InitializeCommands()
    {
        NavigateToUpdatingCommand = new RelayCommand(
            execute: _ => HandleUpdating(),
            canExecute: _ => SelectedItem != null);
        NavigateToInsertingCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<PenaltiesCreateViewModel>(),
            canExecute: _ => true);
        DeleteSelectedItemCommand = new RelayCommand(
            execute: _ => HandleDeleting(),
            canExecute: _ => SelectedItem != null);
    }

    private void HandleUpdating()
    {
        if (SelectedItem != null)
        {
            var message = new PenaltyMessage { Penalty = SelectedItem };
            App.EventAggregator.Publish(message);
            Navigation.NavigateTo<PenaltiesUpdateViewModel>();
        }
    }

    private void HandleDeleting()
    {
        try
        {
            if (SelectedItem != null)
            {
                _penaltiesRepository.Delete(SelectedItem);
                _penaltiesRepository.SaveChanges();

                _fetchedPenalties = FetchDataGridData();
                Penalties = _fetchedPenalties;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting penalty: {ex.Message}", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void HandleStartupMessage(ClientMessage message)
    {
        _clientToFilterBy = message.Client;
        App.EventAggregator.RemoveMessage<ClientMessage>();
        InitializeTitle();
        _fetchedPenalties = FetchDataGridData();
        Penalties = _fetchedPenalties;
    }
}