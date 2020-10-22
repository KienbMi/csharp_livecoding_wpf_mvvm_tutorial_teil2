using ActReport.Core.Contracts;
using ActReport.Core.Entities;
using ActReport.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ActReport.ViewModel
{
    public class ActivityDetailViewModel : BaseViewModel
    {
        private Employee _employee;
        private Activity _activity;
        private DateTime _date;
        private DateTime _startTime;
        private DateTime _endTime;
        private string _activityText;

        public string FullName => $"{_employee.FirstName} {_employee.LastName}";

        public DateTime Date 
        { 
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
        public DateTime StartTime 
        { 
            get
            {
                return _startTime;
            }
            set
            {
                _startTime = value;
                OnPropertyChanged(nameof(StartTime));
            }
        }
        public DateTime EndTime 
        { 
            get
            {
                return _endTime;
            }
            set
            {
                _endTime = value;
                OnPropertyChanged(nameof(EndTime));
            }
        }

        public string ActivityText 
        { 
            get
            {
                return _activityText;
            }
            set
            {
                _activityText = value;
                OnPropertyChanged(nameof(ActivityText));
            }
        }

        public int ActivityID 
        { 
            get
            {
                return _activity.Id;
            }
        }

        public ActivityDetailViewModel(IController controller, Employee employee, Activity activity) : base(controller)
        {
            _employee = employee;

            if (activity == null)
            {
                _activity = new Activity
                {
                    Employee = employee,
                    Employee_Id = employee.Id
                };
            }
            else
            {
                _activity = activity;
            }

            _date = _activity.Date;
            _startTime = _activity.StartTime;
            _endTime = _activity.EndTime;
            _activityText = _activity.ActivityText;
        }

        // Commands
        ICommand _cmdSaveActivity;
        public ICommand CmdSaveActivity 
        { 
            get
            {
                if (_cmdSaveActivity == null)
                {
                    _cmdSaveActivity = new RelayCommand(
                        execute: _ => 
                        { 
                            using(IUnitOfWork uow = new UnitOfWork())
                            {
                                _activity.Date = Date;
                                _activity.StartTime = StartTime;
                                _activity.EndTime = EndTime;
                                _activity.ActivityText = ActivityText;

                                if (_activity.Id == 0)
                                {
                                    uow.ActivityRepository.Insert(_activity);
                                }
                                else
                                {
                                    uow.ActivityRepository.Update(_activity);
                                }
                                uow.Save();
                            }                        
                        },
                        canExecute: _ => true
                        );
                }
                return _cmdSaveActivity;
            }
        }

        ICommand _cmdCancelActivity;
        public ICommand CmdCancelActivity
        {
            get
            {
                if (_cmdCancelActivity == null)
                {
                    _cmdCancelActivity = new RelayCommand(
                        execute: _ => { },
                        canExecute: _ => true
                        );
                }
                return _cmdCancelActivity;
            }
        }
    }
}
