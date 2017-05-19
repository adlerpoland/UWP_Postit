using System;
using Post.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Post.Models;

namespace Post.Views
{
    public sealed partial class MainPage : Page
    {
        private List<Note> Notes;
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            Notes = NoteManager.GetNotes();
        }
    }
}
