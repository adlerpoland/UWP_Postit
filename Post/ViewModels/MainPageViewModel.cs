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
using Windows.ApplicationModel.DataTransfer;

namespace Post.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public ObservableCollection<CollectionRepresentation> NotesCollection = new ObservableCollection<CollectionRepresentation>();
        Note movedObject = null;
        bool objectWasMoved = false;
        ObservableCollection<Note> source;

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

        public void ListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            var item = e.Items[0];
            Note it = item as Note;

            movedObject = it;
            objectWasMoved = false;

            var destinationListView = sender as ListView;
            var listViewItemsSource = destinationListView?.ItemsSource as ObservableCollection<Note>;
            source = listViewItemsSource;


            //Debug.WriteLine(it.header);
            e.Data.RequestedOperation = DataPackageOperation.Move;
        }

        public void ListView_DragOver(object sender, DragEventArgs e)
        {
            //if (e.DataView.Contains(StandardDataFormats.StorageItems))
            //{
            e.AcceptedOperation = DataPackageOperation.Move;
            //}
        }

        public void ListView_Drop(object sender, DragEventArgs e)
        {
            //if (e.DataView.Contains(StandardDataFormats.Text))
            //{
            //var item = await e.DataView.GetTextAsync();
            //var item = await e.DataView.GetStorageItemsAsync();

            //Note it = item as Note;

            var destinationListView = sender as ListView;
            var listViewItemsSource = destinationListView?.ItemsSource as ObservableCollection<Note>;

            if (listViewItemsSource != null)
            {
                objectWasMoved = true;

                //IF IS PARENT
                if (source[0] == movedObject)
                {
                    //MOVE OBJECT TO LIST
                    //listViewItemsSource.Add(movedObject);

                    //GET OBJECT AND CHILDREN AND ADD
                    foreach (Note n in source)
                    {
                        listViewItemsSource.Add(n);
                    }

                    //CHECK IF OBJECT IS STILL PARENT
                    Note firstChild = listViewItemsSource[0];

                    Debug.WriteLine("PARENT HEADER: " + firstChild.header);
                    if(firstChild != movedObject)
                    {
                        Debug.WriteLine("FIRST CHILD ID: " + firstChild.id + " MOVED OBJECT ID: " + movedObject.id);
                        movedObject.parentid = firstChild.id;
                    }
                    else
                    {
                        Debug.WriteLine("FIRST CHILD ID: " + firstChild.id + " MOVED OBJECT ID: " + movedObject.id);
                        movedObject.parentid = 0;
                    }
                }
                //IF IS CHILD
                else
                {
                    //GET PARENT ID
                    //movedObject.parentid = listViewItemsSource[0].id;
                }
            }
            else
            {
                objectWasMoved = false;
            }
        }

        public void ListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            //Debug.WriteLine("COMPLETE");
            if(objectWasMoved == true)
            {
                var destinationListView = sender as ListView;
                var listViewItemsSource = destinationListView?.ItemsSource as ObservableCollection<Note>;

                if (source[0] == movedObject)
                {
                    //GET OBJECT CHILDREN CHANGE PARENT ID AND REMOVE
                    bool parent = false;
                    ObservableCollection<Note> toRemove = new ObservableCollection<Note>();
                    foreach (Note n in listViewItemsSource)
                    {           
                        if (n.id == movedObject.id)
                            parent = true;
                        else if(parent==true)
                        {
                            toRemove.Add(n);
                        }
                    }

                    foreach (Note n in toRemove)
                    {
                        listViewItemsSource.Remove(n);
                        n.parentid = movedObject.parentid;
                    }

                    //REMOVE OBJECT FROM LIST
                    listViewItemsSource.Remove(movedObject);
                }
                else
                {

                }
            }
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

