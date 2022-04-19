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
        private Profile _profile;
        private Window _login_form;

        private List<GameInfo> myGames = new();
        private Game selectedGame;
        private GameInfo selectedGameInfo;

        public MainForm(Profile profile, Window parent)
        {
            InitializeComponent();
            _profile = profile;
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
                timeInGameLabel.Content = $"0:00";
                PlayButton.IsEnabled = false;
                selectedGameInfo = new();
            }
        }

        private void ShowMyGames(object sender, RoutedEventArgs e)
        {
            if (myGamesList == null) return;
            
            ShowMyGames();
        }

        private void ShowAllGames(object sender, RoutedEventArgs e)
        {
            if (myGamesList == null) return;

            ShowAllGames();
        }

        private void ShowAllGames()
        {
            List<Game> games = DBreader.Get_games();
            List<ListBoxItem> listBoxItems = new();

            foreach (var g in games)
            {
                ListBoxItem li = new();
                li.Content = g.Title;
                li.DataContext = g.ID;
                li.Selected += LiSelected;
                listBoxItems.Add(li);
            };
            myGamesList.ItemsSource = listBoxItems;
        }

        private void ShowMyGames()
        {
            myGames = DBreader.Get_my_games_info(_profile.ID);
            List<ListBoxItem> listBoxItems = new();

            foreach (var g in myGames)
            {
                ListBoxItem li = new();
                li.Content = g.game_title;
                li.DataContext = g.game_id;
                li.Selected += LiSelected;
                listBoxItems.Add(li);
            };
            myGamesList.ItemsSource = listBoxItems;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _login_form.Show();
        }

        private void EditGameClick(object sender, RoutedEventArgs e)
        {
            var editor = new EditGameForm(selectedGame.ID, _profile.ID);
            editor.ShowDialog();
        }
    }
}
