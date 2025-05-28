using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Data.Dto;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Services.Navigation;

namespace Travel_Company.WPF.MVVM.ViewModel.Groups;

public class GroupsViewModel : Core.ViewModel
{
    private readonly IRepository<TouristGroup, long> _groupsRepository;

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

    private List<TouristGroup> _fetchedGroups;
    private List<TouristGroup> _groups = null!;
    public List<TouristGroup> Groups
    {
        get => _groups;
        set
        {
            _groups = value;
            OnPropertyChanged();
        }
    }

    private TouristGroup _selectedItem = null!;
    public TouristGroup SelectedItem
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

    private void FilterItems()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Groups = _fetchedGroups.ToList();
            return;
        }

        Groups = _fetchedGroups
            .Where(c => c.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public RelayCommand NavigateToUpdatingCommand { get; set; } = null!;
    public RelayCommand NavigateToInsertingCommand { get; set; } = null!;
    public RelayCommand DeleteSelectedItemCommand { get; set; } = null!;

    public GroupsViewModel(IRepository<TouristGroup, long> repository, INavigationService navigation)
    {
        _groupsRepository = repository;
        _navigation = navigation;

        _fetchedGroups = FetchDataGridData();
        Groups = _fetchedGroups;

        InitializeCommands();
    }

    private List<TouristGroup> FetchDataGridData() => _groupsRepository
        .GetQuaryable()
        .Include(g => g.TourGuide)
        .Include(g => g.Route)
        .Include(g => g.Clients)
        .ToList();

    private void InitializeCommands()
    {
        NavigateToUpdatingCommand = new RelayCommand(
            execute: _ => HandleUpdating(),
            canExecute: _ => true);
        NavigateToInsertingCommand = new RelayCommand(
           execute: _ => Navigation.NavigateTo<GroupsCreateViewModel>(),
           canExecute: _ => true);
        DeleteSelectedItemCommand = new RelayCommand(
            execute: _ => HandleDeleting(),
            canExecute: _ => true);
    }

    private void HandleUpdating()
    {
        if (SelectedItem is not null)
        {
            var message = new TouristGroupMessage { Group = SelectedItem };
            App.EventAggregator.Publish(message);
            Navigation.NavigateTo<GroupsUpdateViewModel>();
        }
    }

    private void HandleDeleting()
    {
        if (SelectedItem is not null)
        {
            _groupsRepository.Delete(SelectedItem);
            _groupsRepository.SaveChanges();

            _fetchedGroups = FetchDataGridData();
            Groups = _fetchedGroups;
        }
    }
}