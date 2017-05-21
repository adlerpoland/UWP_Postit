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
using NotificationsExtensions.ToastContent;
using Windows.UI.Notifications;

namespace Post.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        //COLLECTION OF LISTVIEWS
        public ObservableCollection<CollectionRepresentation> NotesCollection = new ObservableCollection<CollectionRepresentation>();

        //MOVING TASKS BETWEEN LISTVIEWS
        Note movedObject = null;
        bool objectWasMoved = false;
        ObservableCollection<Note> source;

        //NOTIFICATION ON START
        private bool notificate = true;

        public bool Notificate
        {
            get { return notificate; }
            set { notificate = value; }
        }

        //CREATING NEW TASK
        private String _header = "";
        private String _content = "";
        private String _date = "";

        public String taskHeader
        {
            get { return _header; }
            set { _header = value; RaisePropertyChanged("taskHeader"); }
        }
        public String taskContent
        {
            get { return _content; }
            set { _content = value; RaisePropertyChanged("taskContent"); }
        }
        public String taskDate
        {
            get { return _date; }
            set { _date = value; RaisePropertyChanged("taskDate"); }
        }

        //VISIBILITY
        private Visibility _isRemoveVisible = Visibility.Collapsed;
        private Visibility _isNewTaskVisible = Visibility.Collapsed;

        public Visibility isRemoveVisible
        {
            get { return _isRemoveVisible; }
            set { _isRemoveVisible = value; RaisePropertyChanged("isRemoveVisible"); }
        }

        public Visibility isNewTaskVisible
        {
            get { return _isNewTaskVisible; }
            set { _isNewTaskVisible = value; RaisePropertyChanged("isNewTaskVisible"); }
        }

        //VIEWMODEL ON START
        public MainPageViewModel()
        {
            BuildCollection();
            NotificateAboutUpcomingTask();
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

            //CREATE EMPTY LISTVIEW IF SOURCE.COUNT > 1
            if(listViewItemsSource.Count>1)
            {
                var empty = new CollectionRepresentation { NoteCollection = new ObservableCollection<Note>() };
                NotesCollection.Add(empty);
            } 
        }

        public void ListView_DragOver(object sender, DragEventArgs e)
        {
            if(movedObject != null)
            {
                var destinationListView = sender as ListView;
                var listViewItemsSource = destinationListView?.ItemsSource as ObservableCollection<Note>;
                if (listViewItemsSource != source)
                {
                    //IF NOT EMPTY THEN CHECK LAST DATA
                    if (listViewItemsSource.Count > 0)
                    {
                        Note lastNote = listViewItemsSource.Last();
                        if (lastNote.date.CompareTo(movedObject.date) <= 0)
                            e.AcceptedOperation = DataPackageOperation.Move;
                    }
                    else
                        e.AcceptedOperation = DataPackageOperation.Move;
                }
            }
        }

        public void ListView_Drop(object sender, DragEventArgs e)
        {
            if (movedObject != null)
            {
                var destinationListView = sender as ListView;
                var listViewItemsSource = destinationListView?.ItemsSource as ObservableCollection<Note>;

                if (listViewItemsSource != null && listViewItemsSource != source)
                {
                    objectWasMoved = true;

                    //IF IS PARENT 0
                    if (source[0] == movedObject)
                    {
                        //GET OBJECT AND CHILDREN AND ADD
                        foreach (Note n in source)
                        {
                            n.header = RemoveHeaderCount(n.header);
                            listViewItemsSource.Add(n);
                        }

                        //CHECK IF OBJECT IS STILL PARENT
                        Note firstChild = listViewItemsSource[0];

                        //IS NOT
                        if (firstChild != movedObject)
                        {
                            int i = 1;
                            foreach (Note child in listViewItemsSource)
                            {
                                if(child.id != firstChild.id)
                                {
                                    child.parentid = firstChild.id;
                                }
                                child.header = RemoveHeaderCount(child.header);
                                child.header = child.header + " #" + i++;
                            }
                        }
                        //IS STILL PARENT
                        else
                        {
                            int i = 1;
                            foreach (Note child in listViewItemsSource)
                            {
                                if (child.id != movedObject.id)
                                {
                                    child.parentid = firstChild.id;
                                }
                                child.header = RemoveHeaderCount(child.header);
                                child.header = child.header + " #" + i++;
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
                            if (below)
                            {
                                n.header = RemoveHeaderCount(n.header);
                                //n.header = n.header + " #" + source.Count + 1;
                                listViewItemsSource.Add(n);
                            }
                        }

                        //CHECK IF OBJECT IS PARENT
                        Note firstChild = listViewItemsSource[0];

                        if (firstChild != movedObject)
                        {
                            int i = 1;
                            foreach (Note child in listViewItemsSource)
                            {
                                if (child.id != firstChild.id)
                                {
                                    child.parentid = firstChild.id;
                                }
                                child.header = RemoveHeaderCount(child.header);
                                child.header = child.header + " #" + i++;
                            }
                        }
                        else
                        {
                            int i = 1;
                            foreach (Note child in listViewItemsSource)
                            {
                                if (child.id != movedObject.id)
                                {
                                    child.parentid = firstChild.id;
                                }
                                child.header = RemoveHeaderCount(child.header);
                                child.header = child.header + " #" + i++;
                            }
                            movedObject.parentid = 0;
                        }
                    }

                    //SCROLL TO BOTTOM
                    destinationListView.SelectedIndex = destinationListView.Items.Count - 1;
                    destinationListView.ScrollIntoView(destinationListView.SelectedItem);
                    destinationListView.SelectedIndex = -1;
                }
                else
                {
                    objectWasMoved = false;
                }
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
            //REMOVE EMPTY LISTVIEWS FROM GRIDVIEW
            removeEmptyListViews();

            //REMOVE MOVEDOBJECT
            movedObject = null;
        }

        public void removeEmptyListViews()
        {
            ObservableCollection<CollectionRepresentation> list = new ObservableCollection<CollectionRepresentation>();
            foreach(CollectionRepresentation n in NotesCollection)
            {
                if (n.NoteCollection.Count <= 0)
                    list.Add(n);
            }
            foreach(CollectionRepresentation n in list)
            {
                NotesCollection.Remove(n);
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

            //REMOVE EMPTY
            removeEmptyListViews();
        }

        public void GotoDetailsPage() =>
            NavigationService.Navigate(typeof(Views.DetailPage), 0);

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);

        public String RemoveHeaderCount(String h)
        {
            return h.Split('#')[0].Trim();
        }

        
        public void SubmitNewTask()
        {
            String head = taskHeader;
            String cont = taskContent;
            String dat = taskDate;

            String valid = validate(head, cont, dat);
            if (valid.Equals("true"))
            {
                //UKRYJ TWORZENIE KARTECZKi
                isNewTaskVisible = Visibility.Collapsed;
                taskHeader = "";
                taskContent = "";
                taskDate = "";

                //STWORZ DATE ZE STRINGA
                DateTime output;
                DateTime.TryParseExact(dat, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out output);

                //UTWORZ KARTECZKE
                int lastId = GetLastID();
                Note custom = new Note { id = lastId, parentid = 0, header = head, content = cont, date = output };

                //ADD #1
                custom.header = custom.header + " #1";

                //UTWÓRZ NOW¥ LISTE
                var empty = new CollectionRepresentation { NoteCollection = new ObservableCollection<Note>() };
                //DODAJ KARTECZKE DO LISTY
                empty.NoteCollection.Add(custom);
                //DODAJ LISTE DO GRIDVIEW
                NotesCollection.Add(empty);

                //WYSWIETL KOMUNIKAT SUKCESU
                IToastText02 notification = ToastContentFactory.CreateToastText02();
                notification.TextHeading.Text = "Sukces";
                notification.TextBodyWrap.Text = "Utworzono now¹ karteczkê!";
                notification.Duration = ToastDuration.Short;

                ToastNotification not = new ToastNotification(notification.GetXml());
                ToastNotificationManager.CreateToastNotifier().Show(not);
            }
            else
            {
                //WYSWIETL KOMUNIKAT BLEDU
                IToastText02 notification = ToastContentFactory.CreateToastText02();
                notification.TextHeading.Text = "B³¹d";
                notification.TextBodyWrap.Text = valid;
                notification.Duration = ToastDuration.Short;

                ToastNotification not = new ToastNotification(notification.GetXml());
                ToastNotificationManager.CreateToastNotifier().Show(not);
            }
        }

        public String validate(String h, String c, String d)
        {
            if(h.Length>12 || h.Length < 2)
            {
                return "Tytu³ jest z³ej d³ugoœci";
            }
            else if(h.Contains('#'))
            {
                return "U¿yto niedozwolonego znaku!";
            }
            else if(c.Length < 5 || c.Length > 120)
            {
                return "Treœæ jest z³ej d³ugoœci";
            }
            else
            {
                DateTime output;
                if (DateTime.TryParseExact(d, "dd.MM.yyyy",System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out output))
                {
                    if(output.CompareTo(DateTime.Now.Date) < 0)
                    {
                        return "Data jest za stara!";
                    }
                    else
                    {
                        return "true";
                    }           
                }
                else
                {
                    return "Data jest z³ego formatu!\n(dd.MM.yyyy)";
                }
            }
        }

        public void CreateNewTask()
        {
            isNewTaskVisible = Visibility.Visible;
        }
        public void CancelNewTask()
        {
            isNewTaskVisible = Visibility.Collapsed;
            taskHeader = "";
            taskContent = "";
            taskDate = "";
        }

        public int GetLastID()
        {
            int id = 1;
            foreach(CollectionRepresentation collection in NotesCollection)
            {
                foreach(Note n in collection.NoteCollection)
                {
                    if (n.id >= id)
                        id = n.id + 1;
                }
            }
            return id;
        }

        public void NotificateAboutUpcomingTask()
        {
            if(Notificate == true)
            {
                DateTime early = new DateTime(9999, 12, 30);
                String head = "";
                DateTime today = DateTime.Now.Date;
                foreach (CollectionRepresentation collection in NotesCollection)
                {
                    foreach (Note n in collection.NoteCollection)
                    {
                        if (n.date.CompareTo(early) < 0)
                        {
                            early = n.date;
                            head = n.header;
                        }
                    }
                }

                //DISPLAY MSG TODAY
                if (early.Date.CompareTo(today) <= 0)
                {
                    IToastText02 notification = ToastContentFactory.CreateToastText02();
                    notification.TextHeading.Text = "Dzisiaj mija termin zadania!";
                    notification.TextBodyWrap.Text = head;
                    notification.Duration = ToastDuration.Short;

                    ToastNotification not = new ToastNotification(notification.GetXml());
                    not.ExpirationTime = DateTimeOffset.Now.AddSeconds(5);
                    ToastNotificationManager.CreateToastNotifier().Show(not);
                }
                else
                {
                    IToastText02 notification = ToastContentFactory.CreateToastText02();
                    notification.TextHeading.Text = "Zbli¿a siê termin zadania!";
                    notification.TextBodyWrap.Text = head + " (" + early.Date.ToString("dd.MM.yyyy") + ")";
                    notification.Duration = ToastDuration.Short;

                    ToastNotification not = new ToastNotification(notification.GetXml());
                    not.ExpirationTime = DateTimeOffset.Now.AddSeconds(5);
                    ToastNotificationManager.CreateToastNotifier().Show(not);            
                }
            }      
        }

        public void DebugPage()
        {
            
        }
    }
}

