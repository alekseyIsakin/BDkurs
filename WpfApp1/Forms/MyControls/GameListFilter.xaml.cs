using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.DBcore;
namespace WpfApp1.Forms.MyControls
{
    /// <summary>
    /// Логика взаимодействия для GameListFilter.xaml
    /// </summary>
    public partial class GameListFilter : UserControl
    {
        public event EventHandler ApplyClickEvent;
        private List<Publisher> publishersFilter = new();
        public GameListFilter()
        {
            InitializeComponent();
            publisherFilter.DisableDelBtn();
        }

        private void publisherFilter_PublisherCmbSelected(object sender, EventArgs e)
        {
            Publisher publ = ((PublisherEventArgs)e).slectedPublisher;
            if (publishersFilter.Contains(publ))
                return;

            if (publ.ID == -1 && DBreader.CheckPublisherByName(publ.Name))
            {
                var res = MessageBox.Show($"Append new publisher [{publ.Name}] to DataBase", "Warning", MessageBoxButton.YesNo);

                if (res == MessageBoxResult.Yes)
                {
                    publ.ID = DBreader.AddNewPublisher(publ);
                    publisherFilter.ReloadPublishers();
                }
            }

            if (publ.ID == -1) return;

            publishersFilter.Add(publ);
            AddPublisher(publ);
        }
        private void AddPublisher(Publisher publ)
        {
            publisherSetter publControl = new() { publisher = publ };

            publControl.DelClick += RemovePublControl;
            PublishersPanel.Children.Add(publControl);
        }
        private void RemovePublControl(object sender, EventArgs args)
        {
            var control = sender as publisherSetter;

            if (publishersFilter.Contains(control.publisher))
                publishersFilter.Remove(control.publisher);

            PublishersPanel.Children.Remove(control);
        }
        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            List<Game> games = GetFilteredGames();
            ApplyClickEvent?.Invoke(this, new GameFilterEventArg(games));
        }

        public List<Game> GetFilteredGames()
        {
            string title = titleFilter.Text;
            List<int> publID = new();
            bool? isInstalled = null;
            bool? inMyLibrary = null;

            if (inMyLibraryCheck.IsEnabled)
                inMyLibrary = inMyLibraryCheck.IsChecked;
            if (installedCheck.IsEnabled)
                isInstalled = installedCheck.IsChecked;

            foreach (var p in publishersFilter)
                publID.Add(p.ID);

            return DBreader.GetGamesByFilter(title, publID, isInstalled, DBreader.ActiveProfile.ID);
        }

        private void titleEnable_Click(object sender, RoutedEventArgs e)
        {
            titleFilter.Clear();
            titleFilter.IsEnabled = !titleFilter.IsEnabled;
        }

        private void InstalledEnable_Click(object sender, RoutedEventArgs e)
        {
            installedCheck.IsEnabled = !installedCheck.IsEnabled;
        }
        private void inMyLibraryEnable_Click(object sender, RoutedEventArgs e)
        {
            inMyLibraryCheck.IsEnabled = !inMyLibraryCheck.IsEnabled;
        }
    }
}
