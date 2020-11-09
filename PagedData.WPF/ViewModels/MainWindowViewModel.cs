using DynamicData;
using DynamicData.Binding;
using DynamicData.Operators;
using PagedData.WPF.Commands;
using PagedData.WPF.Models;
using PagedData.WPF.Services;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace PagedData.WPF.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private const int PAGE_SIZE = 25;
        private const int FIRST_PAGE = 1;

        private readonly IEmployeesService _employeesService;
        private readonly ISubject<PageRequest> _pager;
        
        private readonly ReadOnlyObservableCollection<Employee> _employees;
        public ReadOnlyObservableCollection<Employee> Employees => _employees;

        public MainWindowViewModel(IEmployeesService employeesService)
        {
            _employeesService = employeesService;

            _pager = new BehaviorSubject<PageRequest>(new PageRequest(FIRST_PAGE, PAGE_SIZE));

            _employeesService.EmployeesConnection()
                .Sort(SortExpressionComparer<Employee>.Ascending(e => e.ID))
                .Page(_pager)
                .Do(change => PagingUpdate(change.Response))
                .ObserveOnDispatcher()
                .Bind(out _employees)
                .Subscribe();
        }

        private int _totalItems;
        public int TotalItems
        {
            get => _totalItems;
            set
            {
                _totalItems = value;
                OnPropertyChanged();
            }
        }

        private int _currentPage;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        private int _totalPages;
        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                OnPropertyChanged();
            }
        }

        private void PagingUpdate(IPageResponse response)
        {
            TotalItems = response.TotalSize;
            CurrentPage = response.Page;
            TotalPages = response.Pages;
        }

        #region Load employee data command
        private RelayCommand _loadDataCommand;
        public RelayCommand LoadDataCommand =>
            _loadDataCommand ??= new RelayCommand(_ => LoadEmployeeData());

        private void LoadEmployeeData() => _employeesService.LoadData();
        #endregion

        #region Previous page command
        private RelayCommand _previousPageCommand;
        public RelayCommand PreviousPageCommand => _previousPageCommand ??=
            new RelayCommand(_ => MoveToPreviousPage(), _ => CanMoveToPreviousPage());

        private void MoveToPreviousPage() => 
            _pager.OnNext(new PageRequest(_currentPage - 1, PAGE_SIZE));

        private bool CanMoveToPreviousPage() => CurrentPage > FIRST_PAGE;
        #endregion

        #region Next page command
        private RelayCommand _nextPageCommand;
        public RelayCommand NextPageCommand => _nextPageCommand ??=
            new RelayCommand(_ => MoveToNextPage(), _ => CanMoveToNextPage());

        private void MoveToNextPage() =>
            _pager.OnNext(new PageRequest(_currentPage + 1, PAGE_SIZE));

        private bool CanMoveToNextPage() => CurrentPage < TotalPages;
        #endregion

        #region First page command
        private RelayCommand _firstPageCommand;
        public RelayCommand FirstPageCommand => _firstPageCommand ??=
            new RelayCommand(_ => MoveToFirstPage(), _ => CanMoveToFirstPage());

        private void MoveToFirstPage() => 
            _pager.OnNext(new PageRequest(FIRST_PAGE, PAGE_SIZE));

        private bool CanMoveToFirstPage() => CurrentPage > FIRST_PAGE;
        #endregion

        #region Last page command
        private RelayCommand _lastPageCommand;
        public RelayCommand LastPageCommand => _lastPageCommand ??=
            new RelayCommand(_ => MoveToLastPage(), _ => CanMoveToLastPage());

        private void MoveToLastPage() => 
            _pager.OnNext(new PageRequest(_totalPages, PAGE_SIZE));

        private bool CanMoveToLastPage() => CurrentPage < TotalPages;
        #endregion
    }
}
