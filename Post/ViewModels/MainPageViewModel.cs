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
        private Visibility _isRemoveVisible = Visibility.Collapsed;


        public Visibility isRemoveVisible
        {
            get { return _isRemoveVisible; }
            set { _isRemoveVisible = value; RaisePropertyChanged("isRemoveVisible"); }
        }

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

            //CHECK IF IT'S LAST NOTE
            //SO YOU CAN DISPLAY X FOR REMOVING
            if(listViewItemsSource.Last() == movedObject)
            {
                isRemoveVisible = Visibility.Visible;
            }
        }

        public void ListView_DragOver(object sender, DragEventArgs e)
        {
            var destinationListView = sender as ListView;
            var listViewItemsSource = destinationListView?.ItemsSource as ObservableCollection<Note>;
            if(listViewItemsSource!=source)
            {
                //IF NOT EMPTY THEN CHECK LAST DATA
                if(listViewItemsSource.Count > 0)
                {
                    Note lastNote = listViewItemsSource.Last();
                    if (lastNote.date.CompareTo(movedObject.date) <= 0)
                        e.AcceptedOperation = DataPackageOperation.Move;
                }
                else
                    e.AcceptedOperation = DataPackageOperation.Move;
            }
        }

        public void ListView_Drop(object sender, DragEventArgs e)
        {
            var destinationListView = sender as ListView;
            var listViewItemsSource = destinationListView?.ItemsSource as ObservableCollection<Note>;

            if (listViewItemsSource != null && listViewItemsSource!=source)
            {
                objectWasMoved = true;

                //IF IS PARENT
                if (source[0] == movedObject)
                {
                    //GET OBJECT AND CHILDREN AND ADD
                    foreach (Note n in source)
                    {
                        listViewItemsSource.Add(n);
                    }

                    //CHECK IF OBJECT IS STILL PARENT
                    Note firstChild = listViewItemsSource[0];

                    if(firstChild != movedObject)
                    {
                        foreach(Note child in listViewItemsSource)
                        {
                            child.parentid = firstChild.id;
                        }
                    }
                    else
                    {
                        foreach (Note child in listViewItemsSource)
                        {
                            child.parentid = movedObject.id;
                        }
                        movedObject.parentid = 0;
                    }
                }
                //IF IS CHILD
                else
                {
                    //GET OBJECT AND CHILDREN BELOW IT AND ADD
                    bool below = false;
                    foreach (Note n in source)
                    {
                        if (n.id == movedObject.id)
                            below = true;
                        if(below)
                            listViewItemsSource.Add(n);
                    }

                    //CHECK IF OBJECT IS PARENT
                    Note firstChild = listViewItemsSource[0];

                    if (firstChild != movedObject)
                    {
                        foreach (Note child in listViewItemsSource)
                        {
                            child.parentid = firstChild.id;
                        }
                    }
                    else
                    {
                        foreach (Note child in listViewItemsSource)
                        {
                            child.parentid = movedObject.id;
                        }
                        movedObject.parentid = 0;
                    }
                }
            }
            else
            {
                objectWasMoved = false;
            }
        }

        public void ListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            //HIDE REMOVE
            isRemoveVisible = Visibility.Collapsed;

            //Debug.WriteLine("COMPLETE");
            if (objectWasMoved == true)
            {
                var destinationListView = sender as ListView;
                var listViewItemsSource = destinationListView?.ItemsSource as ObservableCollection<Note>;

                //IF WAS PARENT
                if (source[0] == movedObject)
                {
                    //GET OBJECT CHILDREN AND REMOVE
                    ObservableCollection<Note> toRemove = new ObservableCollection<Note>();
                    foreach (Note n in listViewItemsSource)
                    {
                        toRemove.Add(n);
                    }
                    foreach (Note n in toRemove)
                    {
                        listViewItemsSource.Remove(n);
                    }
                }
                //IF WAS CHILD
                else
                {
                    bool parent = false;
                    ObservableCollection<Note> toRemove = new ObservableCollection<Note>();
                    foreach (Note n in listViewItemsSource)
                    {
                        if (n.id == movedObject.id)
                        {
                            parent = true;
                            toRemove.Add(n);
                        }
                        else if (parent == true)
                        {
                            toRemove.Add(n);
                        }
                    }
                    foreach (Note n in toRemove)
                    {
                        listViewItemsSource.Remove(n);
                    }
                }
            }
        }

        public void Remove_DragOver(object sender, DragEventArgs e)
        {
            if(source.Last() == movedObject)
            {
                e.AcceptedOperation = DataPackageOperation.Move;
                e.DragUIOverride.Caption = "Usuñ";
            }
        }

        public void Remove_Drop(object sender, DragEventArgs e)
        {
            //REMOVE OBJECT
            source.Remove(movedObject);
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

