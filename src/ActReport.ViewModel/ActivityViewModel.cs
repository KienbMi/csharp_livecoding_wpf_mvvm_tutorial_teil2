using ActReport.Core.Contracts;
using ActReport.Core.Entities;
using ActReport.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ActReport.ViewModel
{
    public class ActivityViewModel : BaseViewModel
    {
        private Employee _employee;
        private Activity _selectedActivity;
        private ObservableCollection<Activity> _activities;

        public ObservableCollection<Activity> Activities
        {
            get => _activities;
            set
            {
                _activities = value;
                OnPropertyChanged(nameof(Activities));
            }
        }

        public Activity SelectedActivity
        {
            get
            {
                return _selectedActivity;
            }
            set
            {
                _selectedActivity = value;
                OnPropertyChanged(nameof(SelectedActivity));
            }
        }

        public string FullName => $"{_employee.FirstName} {_employee.LastName}";

        public ActivityViewModel(IController controller, Employee employee) : base(controller)
        {
            _employee = employee;
            LoadActivities();
        }

        private void LoadActivities()
        {
            using (IUnitOfWork uow = new UnitOfWork())
            {
                Activities = new ObservableCollection<Activity>(uow.ActivityRepository.Get(
                    filter: x => x.Employee_Id == _employee.Id,
                    orderBy: coll => coll.OrderBy(activity => activity.Date).ThenBy(activity => activity.StartTime)
                    ));
            }
            Activities.CollectionChanged += Activities_CollectionChanged;
        }

        private void Activities_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                using (IUnitOfWork uow = new UnitOfWork())
                {
                    foreach (var item in e.OldItems)
                    {
                        uow.ActivityRepository.Delete((item as Activity).Id);
                    }
                    uow.Save();
                }
            }
        }

        // Commands
        private ICommand _cmdEditActivity;

        public ICommand CmdEditActivity
        {
            get
            {
                if (_cmdEditActivity == null)
                {
                    _cmdEditActivity = new RelayCommand(
                        execute: _ => _controller.ShowWindow(new ActivityDetailViewModel(_controller, _employee, SelectedActivity)),
                        canExecute: _ => SelectedActivity != null);
                }
                return _cmdEditActivity;
            }
        }

        private ICommand _cmdNewActivity;

        public ICommand CmdNewActivity
        {
            get
            {
                if (_cmdNewActivity == null)
                {
                    _cmdNewActivity = new RelayCommand(
                        execute: _ => _controller.ShowWindow(new ActivityDetailViewModel(_controller, _employee, null)),
                        canExecute: _ => true
                        );
                }
                return _cmdNewActivity;
            }
        }

        private ICommand _cmdDeleteActivity;
        public ICommand CmdDeleteActivity 
        { 
            get
            {
                if (_cmdDeleteActivity == null)
                {
                    _cmdDeleteActivity = new RelayCommand(
                        execute: _ =>
                        {
                            using (IUnitOfWork uow = new UnitOfWork())
                            {
                                uow.ActivityRepository.Delete(SelectedActivity.Id);
                                uow.Save();
                            }
                            LoadActivities();
                        },
                        canExecute: _ => SelectedActivity != null);
                }
                return _cmdDeleteActivity;
            }
        }
    }
}
