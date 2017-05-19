using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using Post.Models;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using System.Collections.ObjectModel;
using Windows.Foundation;

namespace Post.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public ObservableCollection<Note> Notes;
        public MainPageViewModel()
        {
            Notes = NoteManager.GetNotes();
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        public void GotoDetailsPage() =>
            NavigationService.Navigate(typeof(Views.DetailPage), 0);

        public void GotoTaskPage() =>
            NavigationService.Navigate(typeof(Views.TaskPage), 0);

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);

    }
}

