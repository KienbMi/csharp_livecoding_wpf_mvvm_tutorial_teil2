using ActReport.Core.Contracts;
using ActReport.Core.Entities;
using ActReport.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ActReport.ViewModel
{
    public class ActivityViewModel : BaseViewModel
    {
        private Employee _employee;
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
                using(IUnitOfWork uow = new UnitOfWork())
                {
                    foreach (var item in e.OldItems)
                    {
                        uow.ActivityRepository.Delete((item as Activity).Id);
                    }
                    uow.Save();
                }
            }
        }
    }
}
