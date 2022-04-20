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
    /// Логика взаимодействия для new_game.xaml
    /// </summary>
    public partial class EditGameForm : Window
    {
        Game selectedGame = Game.EMPTY;
        GameInfo selectedGameInfo = new();
        public EditGameForm()
        {
            InitializeComponent();
            FillGameList();

            selectedGame = DBreader.Get_Game(2);

            for (int i = 0; i < GameComboBox.Items.Count; i++)
            {
                var li = (ListBoxItem)GameComboBox.Items[i];
                int check_id = Convert.ToInt32(li.DataContext);

                if (check_id == selectedGame.ID)
                {
                    GameComboBox.SelectedItem = li;
                    break;
                }
            }
        }
        public EditGameForm(int _game_id, int _profile_id)
        {
            InitializeComponent();
            FillGameList();
            selectedGame = DBreader.Get_Game(_game_id);

            for (int i = 0; i < GameComboBox.Items.Count; i++) 
            {
                var li = (ListBoxItem)GameComboBox.Items[i];
                int check_id = Convert.ToInt32(li.DataContext);

                if (check_id == selectedGame.ID) 
                {
                    GameComboBox.SelectedItem = li;
                    break;
                }
            }
        }

        private void FillGameList() 
        {
            List<Game> games = DBreader.Get_games();
            games.Sort((v1, v2) => v1.Title.CompareTo(v2.Title));
            games.Add(Game.EMPTY);

            List<ComboBoxItem> listBoxItems = new();

            foreach (var g in games)
            {
                ComboBoxItem li = new();
                li.Content = g.Title;
                li.DataContext = g.ID;
                li.Selected += GameLiSelected;
                listBoxItems.Add(li);
            };
            GameComboBox.ItemsSource = listBoxItems;
        }

        private void GameLiSelected(object sender, RoutedEventArgs e)
        {
            var li = (ComboBoxItem)sender;
            int game_id = Convert.ToInt32(li.DataContext);
            selectedGame = DBreader.Get_Game(game_id);

            GameTitleTextBox.Text = selectedGame.Title;
            ReleaseDatePicker.SelectedDate = selectedGame.Relese_date;

            PublishersPanel.Children.Clear();
            foreach (Publisher publ in selectedGame.Publishers) 
            {
                AddPublisher(publ);
            }

            DescriptionTexBlock.Text = selectedGame.Descriptions;
        }

        private void AddPublisherClick(object sender, EventArgs e)
        {
            Publisher publ = ((Controls.PublisherEventArgs)e).slectedPublisher;

            if (selectedGame.Publishers.Contains(publ))
                return;
            selectedGame.Publishers.Add(publ);
            AddPublisher(publ);
        }

        private void RemovePublControl(object sender, EventArgs args) 
        {
            var control = sender as Controls.publisherSetter;

            if (selectedGame.Publishers.Contains(control.publisher))
                selectedGame.Publishers.Remove(control.publisher);

            PublishersPanel.Children.Remove(control);
        }
        private void AddPublisher(Publisher publ)
        {
            Controls.publisherSetter publControl = new() { publisher = publ };

            publControl.DelClick += RemovePublControl;
            PublishersPanel.Children.Add(publControl);
        }

        private void GameTitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            selectedGame.Title = GameTitleTextBox.Text;
        }
        private void ReleaseDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime date = new();
            if (ReleaseDatePicker.SelectedDate.HasValue)
                date = ReleaseDatePicker.SelectedDate.Value;

            selectedGame.Relese_date = date;
        }

        private void DescriptionTexBlock_TextChanged(object sender, TextChangedEventArgs e)
        {
            selectedGame.Descriptions = DescriptionTexBlock.Text;
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGame.ID == -1)
                DBreader.AddNewGame(selectedGame);
            else
                DBreader.UpdateGame(selectedGame);
        }
    }
}
