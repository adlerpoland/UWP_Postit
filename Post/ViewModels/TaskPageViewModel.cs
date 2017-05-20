using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using NotificationsExtensions.ToastContent;
using Windows.UI.Notifications;

namespace Post.ViewModels
{
    public class TaskPageViewModel : ViewModelBase
    {
        public TaskPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Value = "Designtime value";
            }
        }

        private string _Value = "Default";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            Value = (suspensionState.ContainsKey(nameof(Value))) ? suspensionState[nameof(Value)]?.ToString() : parameter?.ToString();
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(Value)] = Value;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        public void GotoMainPage() =>
            NavigationService.Navigate(typeof(Views.MainPage));

        public void CreateNewPost()
        {
            //VALIDATE
            bool validate = false;
            if (validate)
            {
                NavigationService.Navigate(typeof(Views.MainPage));
            }
            else
            {
                IToastText04 notification = ToastContentFactory.CreateToastText04();
                notification.TextHeading.Text = "Validation error";
                notification.TextBody1.Text = "Sorry but you've input wrong data!\n\n";
                notification.TextBody2.Text = DateTime.Now.ToString("hh:mm:ss");
                notification.Duration = ToastDuration.Short;


                /*ScheduledToastNotification time;
                time = new ScheduledToastNotification(notification.GetXml(), DateTime.Now.AddMilliseconds(100));
                time.Id = "notificationID";
                ToastNotificationManager.CreateToastNotifier().AddToSchedule(time);*/

                ToastNotification not = new ToastNotification(notification.GetXml());
                ToastNotificationManager.CreateToastNotifier().Show(not);
            }
        }
    }
}

