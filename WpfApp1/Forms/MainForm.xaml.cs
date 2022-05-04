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
using System.Windows.Shapes;
using WpfApp1.DBcore;

namespace WpfApp1.Forms
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class MainForm : Window
    {
        private Window _login_form;
        private List<GameInfo> myGames = new();

        private Game selectedGame;
        private GameInfo selectedGameInfo;

        private bool onlyMyGamesIsShow = true;
        public MainForm(Window parent)
        {
            InitializeComponent();
            _login_form = parent;

            ShowMyGames();
        }

        private void LiSelected(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as ListBoxItem).DataContext);

            selectedGame = DBreader.Get_Game(id);

            labelGameTitle.Content = selectedGame.Title;

            textDescription.Text = "\nPublishers:\n";
            foreach (Publisher publ in selectedGame.Publishers ?? new(){ }) 
            {
                textDescription.Text += $"{publ.Name}\n";
            }

            textDescription.Text += "\nRelease: ";
            textDescription.Text += selectedGame.Relese_date + "\n";
            textDescription.Text += "\nDescriptions:\n";
            textDescription.Text += selectedGame.Descriptions;


            var selected_game = myGames.Where(gmInfo => gmInfo.game_id == selectedGame.ID);

            if (selected_game.Count() == 1)
            {
                selectedGameInfo = selected_game.First();
                TimeSpan timeInGame = TimeSpan.FromMinutes(selectedGameInfo.time_in_game);
                bool has_exe = selectedGameInfo.executable_file != "";

                PlayButton.IsEnabled = has_exe;
                timeInGameLabel.Content = $"{ Math.Floor(timeInGame.TotalHours)}:{timeInGame:mm}";
            }
            else 
            {
                timeInGameLabel.Content = $"00:00";
                PlayButton.IsEnabled = false;
                selectedGameInfo = new();
            }
        }

        private void FillGamePanel(List<Game> games)
        {
            if (myGamesList is null) return;

            List<ListBoxItem> listBoxItems = new();

            foreach (var g in games)
            {
                ListBoxItem li = new();
                li.Content = g.Title;
                li.DataContext = g.ID;
                li.Selected += LiSelected;
                listBoxItems.Add(li);
            };

            onlyMyGamesIsShow = false;
            myGamesList.ItemsSource = listBoxItems;
        }
        private void ShowMyGames(object sender, RoutedEventArgs e)
        {
            ShowMyGames();
        }

        private void ShowAllGames(object sender, RoutedEventArgs e)
        {
            ShowAllGames();
        }

        private void ShowFilteredGames()
        {
            List<Game> games = advanceFilter.GetFilteredGames();
            FillGamePanel(games);
        }
        private void ShowAllGames()
        {
            List<Game> games = DBreader.GetGamesByFilter(title:"", publID:null, isInstalled:null, myGames: null);
            FillGamePanel(games);
        }
        private void ShowMyGames()
        {
            List<Game> games = DBreader.GetGamesByFilter(title:"", publID:null, isInstalled:null, myGames: true);
            FillGamePanel(games);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _login_form.Show();
        }

        private void EditGameClick(object sender, RoutedEventArgs e)
        {
            var editor = new EditGameForm(selectedGame.ID);
            editor.ShowDialog();

            if (onlyMyGamesIsShow)
                ShowMyGames();
            else
                ShowAllGames();

        }
        private void ToggleAdvanceFilter_Click(object sender, RoutedEventArgs e) 
        {
            if (advanceFilter.Visibility == Visibility.Visible)
            {
                toggleFilterBtn.Content = "Show";
                advanceFilter.Visibility = Visibility.Collapsed;
            }
            else
            {
                toggleFilterBtn.Content = "Hide";
                advanceFilter.Visibility = Visibility.Visible;
            }
        }

        private void advanceFilter_Apply(object sender, EventArgs e)
        {
            List<Game> games = ((MyControls.GameFilterEventArg)e).newGames;
            FillGamePanel(games);
        }
    }
}
