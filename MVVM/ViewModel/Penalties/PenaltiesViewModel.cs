using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

    private Penalty _selectedItem = null!;
    public Penalty SelectedItem
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
            FilterItems();
        }
    }

    private void FilterItems()
    {
        if (string.IsNullOrWhiteSpace(SearchText) && _clientToFilterBy is null)
        {
            Penalties = _fetchedPenalties.ToList();
        }

        if (string.IsNullOrWhiteSpace(SearchText) && _clientToFilterBy is not null)
        {
            Penalties = _fetchedPenalties.Where(p => p.ClientId == _clientToFilterBy.Id).ToList();
        }

        if (_clientToFilterBy is null)
        {
            Penalties = _fetchedPenalties
                .Where(c => c.Client.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (_clientToFilterBy is not null)
        {
            Penalties = _fetchedPenalties
                .Where(c => c.Client.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) &&
                    c.ClientId == _clientToFilterBy.Id)
                .ToList();
        }
    }

    public RelayCommand NavigateToUpdatingCommand { get; set; } = null!;
    public RelayCommand NavigateToInsertingCommand { get; set; } = null!;
    public RelayCommand DeleteSelectedItemCommand { get; set; } = null!;

    public PenaltiesViewModel(IRepository<Penalty, long> repository, INavigationService navigation)
    {
        _penaltiesRepository = repository;
        _navigation = navigation;

        App.EventAggregator.Subscribe<ClientMessage>(HandleStartupMessage);

        _fetchedPenalties = FetchDataGridData();
        Penalties = _fetchedPenalties;

        InitializeTitle();
        InitializeCommands();
    }

    private List<Penalty> FetchDataGridData()
    {
        return (_clientToFilterBy is null)
            ? _penaltiesRepository
                .GetQuaryable()
                .Include(p => p.Client)
                .ThenInclude(c => c.TouristGroups)
                .Include(c => c.TourGuide)
                .ToList()
            : _penaltiesRepository
                .GetQuaryable()
                .Include(p => p.Client)
                .ThenInclude(c => c.TouristGroups)
                .Where(p => p.ClientId == _clientToFilterBy.Id)
                .Include(c => c.TourGuide)
                .ToList();
    }

    private void InitializeTitle()
    {
        if (_clientToFilterBy is not null)
        {
            PageTitle = $"{LocalizedStrings.Instance["Penalties"]}: {_clientToFilterBy.FullName}";
        }
    }

    private void InitializeCommands()
    {
        NavigateToUpdatingCommand = new RelayCommand(
            execute: _ => HandleUpdating(),
            canExecute: _ => true);
        NavigateToInsertingCommand = new RelayCommand(
           execute: _ => Navigation.NavigateTo<PenaltiesCreateViewModel>(),
           canExecute: _ => true);
        DeleteSelectedItemCommand = new RelayCommand(
            execute: _ => HandleDeleting(),
            canExecute: _ => true);
    }
    private void HandleUpdating()
    {
        if (SelectedItem is not null)
        {
            var message = new PenaltyMessage { Penalty = SelectedItem };
            App.EventAggregator.Publish(message);
            Navigation.NavigateTo<PenaltiesUpdateViewModel>();
        }
    }

    private void HandleDeleting()
    {
        if (SelectedItem is not null)
        {
            _penaltiesRepository.Delete(SelectedItem);
            _penaltiesRepository.SaveChanges();

            _fetchedPenalties = FetchDataGridData();
            Penalties = _fetchedPenalties;
        }
    }

    private void HandleStartupMessage(ClientMessage message)
    {
        _clientToFilterBy = message.Client;
        App.EventAggregator.RemoveMessage<ClientMessage>();
    }
}