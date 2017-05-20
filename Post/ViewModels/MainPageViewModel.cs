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
using System.Diagnostics;
using Windows.UI.Xaml.Data;
using System.Globalization;

namespace Post.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public ObservableCollection<CollectionRepresentation> NotesCollection = new ObservableCollection<CollectionRepresentation>();

        public MainPageViewModel()
        {
            BuildCollection();
            
        }

        public void BuildCollection()
        {
            var list = NoteManager.GetNoteList();
            NotesCollection.Clear();

            foreach (Note n in list)
            {
                //If it's first note then create collection
                if(NoteManager.isFirstNote(n.id))
                {
                    var collection = (new CollectionRepresentation { NoteCollection = getSingleCollection(n.id) });
                    NotesCollection.Add(collection);
                }
            }
            
        }

        public class CollectionRepresentation
        {
            public ObservableCollection<Note> NoteCollection { get; set; }
        }
        public ObservableCollection<Note> getSingleCollection(int id)
        {
            return NoteManager.GetSingleCollection(id);
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

        public void DebugPage()
        {
            
        }
    }
}

