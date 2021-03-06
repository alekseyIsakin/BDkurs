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
        private Game selectedGame = Game.EMPTY;
        private GameInfo selectedGameInfo = GameInfo.EMPTY;
        private Profile profile => DBcore.DBreader.ActiveProfile;
        MyControls.TempPopup popup = new();
        public EditGameForm()
        {
            InitializeComponent();
            FillGameListUI();

            selectedGame = DBreader.Get_Game(1);

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

            if (GameComboBox.SelectedIndex == -1)
                GameComboBox.SelectedIndex = 0;
        }
        public EditGameForm(int _game_id)
        {
            selectedGame = DBreader.Get_Game(_game_id);

            InitializeComponent();
            FillGameListUI();
            FillGameInfoUI(selectedGame);

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

            if (GameComboBox.SelectedIndex == -1)
                GameComboBox.SelectedIndex = 0;
        }


        private void AddPublisherClick(object sender, EventArgs e)
        {
            Publisher publ = ((MyControls.PublisherEventArgs)e).slectedPublisher;
            if (selectedGame.Publishers.Contains(publ))
                return;

            if (publ.ID == -1 && DBreader.CheckPublisherByName(publ.Name)) 
            {
                var res = MessageBox.Show($"Append new publisher [{publ.Name}] to DataBase", "Warning", MessageBoxButton.YesNo);

                if (res == MessageBoxResult.Yes) 
                {
                    publ.ID = DBreader.AddNewPublisher(publ);
                    PublishersCMB.ReloadPublishers();
                }
            }

            if (publ.ID == -1) return;

            selectedGame.Publishers.Add(publ);
            AddPublisher(publ);
        }
        private void AddPublisher(Publisher publ)
        {
            MyControls.publisherSetter publControl = new() { publisher = publ };

            publControl.DelClick += RemovePublControl;
            PublishersPanel.Children.Add(publControl);
        }
        private void TotalDeletePublisherClick(object sender, EventArgs e)
        {
            Publisher publ = ((MyControls.PublisherEventArgs)e).slectedPublisher;
            Action DeletePublisherControl = () => {; };


            if (selectedGame.Publishers.Contains(publ)) 
                foreach (MyControls.publisherSetter pub in PublishersPanel.Children)
                {
                    if (pub.publisher.Name == publ.Name)
                    {
                        DeletePublisherControl = pub.RaiseDelClick;
                        break;
                    }
                }

            var res = MessageBox.Show($"Delete [{publ.Name}] from DataBase\nAre you sure?\nthis action cannot be undone", "Warning", MessageBoxButton.YesNo);

            if (publ.ID != -1 && res == MessageBoxResult.Yes) 
            {
                DeletePublisherControl();
                DBreader.DeletePublisher(publ);
                PublishersCMB.ReloadPublishers();
            }
        }
        private void RemovePublControl(object sender, EventArgs args) 
        {
            var control = sender as MyControls.publisherSetter;

            if (selectedGame.Publishers.Contains(control.publisher))
                selectedGame.Publishers.Remove(control.publisher);

            PublishersPanel.Children.Remove(control);
        }

        private void FillGameListUI()
        {
            List<Game> games = advanceFilter.GetFilteredGames();
            FillGameListUI(games);
        }
        private void FillGameListUI(List<Game> games)
        {
            //games.Sort((v1, v2) => string.Compare(v1.Title, v2.Title, StringComparison.Ordinal));
            games.Add(Game.EMPTY);

            List<ComboBoxItem> listBoxItems = new();

            foreach (var g in games)
            {
                ComboBoxItem li = new();
                //li.MouseLeftButtonDown += GameLiSelected;
                li.Content = g.Title;
                li.DataContext = g.ID;
                li.Selected += GameLiSelected;
                listBoxItems.Add(li);
            };
            GameComboBox.ItemsSource = listBoxItems;
        }

        private void FillGameInfoUI(Game selectedGame)
        {
            ResetGameInfoColors();
            var gameInfo = DBreader.Get_my_game_infos(profile.ID, selectedGame.ID);

            if (gameInfo.Count == 0 || selectedGame.ID == -1)
            {
                DisableGameInfoUI();
                selectedGameInfo = GameInfo.EMPTY;
            }
            else 
            {
                EnableGameInfoUI();
                selectedGameInfo = gameInfo.First();
            }


            FillGameInfoUI(selectedGameInfo);
        }
        private void FillGameInfoUI(GameInfo gi)
        {
            gmInfoExecPath.Text = gi.executable_file;
            gmInfoSavePath.Text = gi.save_file;
            textBoxHours.Text = (gi.time_in_game / 60).ToString();
            textBoxMinuts.Text = (gi.time_in_game % 60).ToString();
        }
        private void ResetGameInfoColors() 
        {
            gmInfoExecPath.Background = Brushes.White;
            gmInfoSavePath.Background = Brushes.White;
            textBoxHours.Background   = Brushes.White;
            textBoxMinuts.Background  = Brushes.White;
        }


        private void GameLiSelected(object sender, RoutedEventArgs e)
        {
            var li = (ComboBoxItem)sender;
            int game_id = Convert.ToInt32(li.DataContext);

            FillGameUIByID(game_id);
            FillGameInfoUI(selectedGame);
        }
        private void FillGameUIByID(int game_id)
        {
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
        private GameInfo GetGameInfoByUI()
        {
            GameInfo gi = GameInfo.EMPTY;
            gi.game_id = selectedGame.ID;
            gi.profile_id = profile.ID;


            string path_to_exe  = gmInfoExecPath.Text;
            string path_to_save = gmInfoSavePath.Text;
            string minuts_str   = textBoxMinuts.Text;
            string hours_str    = textBoxHours.Text;

            path_to_exe = path_to_exe.Replace("\"", "");
            path_to_save = path_to_save.Replace("\"", "");

            ulong.TryParse(minuts_str, out ulong minuts);
            ulong.TryParse(hours_str, out ulong hours);
            gi.time_in_game = (hours * 60) + minuts;

            textBoxHours.Text = (gi.time_in_game / 60).ToString();
            textBoxMinuts.Text = (gi.time_in_game % 60).ToString();

            if (System.IO.File.Exists(path_to_exe)) 
            {
                gmInfoExecPath.Background = Brushes.White;
                gi.executable_file = path_to_exe;
            }
            else 
            {
                gmInfoExecPath.Background = Brushes.Red;
                gi.executable_file = "";
            }

            if (System.IO.File.Exists(path_to_save) || System.IO.Directory.Exists(path_to_save)) 
            {
                gmInfoExecPath.Background = Brushes.White;
                gi.save_file = path_to_save;
            }
            else
            {
                gmInfoSavePath.Background = Brushes.Red;
                gi.save_file = "";
            }

            return gi;
        }
        private void textBoxTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            var math = System.Text.RegularExpressions.Regex.Replace(textBox.Text, @"([^0-9])*", string.Empty);

            if (textBox.Text != math)
            {
                int crt = textBox.CaretIndex;
                textBox.Text = math;
                textBox.CaretIndex = crt - 1;
            }
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            List<GameInfo> myGame;
            DateTime date = new();
            int selected_ind = GameComboBox.SelectedIndex;
            bool has_new_gameinfo = MyGame_checkBox.IsChecked ?? false;

            selectedGame.Title = GameTitleTextBox.Text;
            if (ReleaseDatePicker.SelectedDate.HasValue)
                date = ReleaseDatePicker.SelectedDate.Value;
            selectedGame.Relese_date = date;
            selectedGame.Descriptions = DescriptionTexBlock.Text;

            if (selectedGame.ID == -1)
                selectedGame.ID = DBreader.AddNewGame(selectedGame);
            else
                DBreader.UpdateGame(selectedGame);
            FillGameListUI();

            selectedGameInfo = GetGameInfoByUI();

            FillGameUIByID(selectedGame.ID);
            FillGameInfoUI(selectedGameInfo);

            myGame = DBreader.Get_my_game_infos(
                selectedGameInfo.profile_id,
                selectedGameInfo.game_id);

            string msg = "Changes have been applied";

            if (has_new_gameinfo == false)
            {
                if (myGame.Count() > 0)
                {
                    DBreader.DeleteMyGame(
                        selectedGameInfo.profile_id,
                        selectedGameInfo.game_id);
                    msg += "\nGame has been removed from MyGames";
                    myGame.Clear();
                }
            }
            else 
            {
                if (myGame.Count == 0)
                {
                    DBreader.SetupNewGame(selectedGameInfo);
                    msg += "\nGame added to MyGames";
                }
                else 
                {
                    DBreader.UpdateMyGame(selectedGameInfo);
                    msg += "\nMyGame has been updated";
                }
            }

            GameComboBox.SelectedIndex = selected_ind;

            popup.PlacementTarget = (UIElement)sender;
            popup.Show(msg);
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e) 
        {
            var res = MessageBox.Show($"Delete [{selectedGame.Title}] from library? This action cannot be undone", "Warning!", MessageBoxButton.YesNo);

            if (res == MessageBoxResult.No)
                return;

            string msg = "Game has been removed from library";
            int selected_ind = GameComboBox.SelectedIndex;
            if (selectedGame.ID != -1) 
            {
                DBreader.DeleteGame(selectedGame);
                FillGameListUI();
                selected_ind -= 1;
            }

            GameComboBox.SelectedIndex = Math.Max(0, selected_ind);

            popup.PlacementTarget = (UIElement)sender;
            popup.IsOpen = true;
            popup.Show(msg);
        }
        private void DeleteGameInfoButton_Click(object sender, RoutedEventArgs e) 
        {
            DBreader.DeleteMyGame(profile.ID, selectedGameInfo.game_id);
            FillGameInfoUI(selectedGame);
        }

        private void InMyGames_Checked(object sender, RoutedEventArgs e)
        {
            EnableGameInfoUI();
        }
        private void InMyGames_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableGameInfoUI();
        }
        private void EnableGameInfoUI()
        {
            gmInfoExecPath.IsEnabled = true;
            gmInfoSavePath.IsEnabled = true;
            textBoxMinuts.IsEnabled = true;
            textBoxHours.IsEnabled = true;
            MyGame_checkBox.IsChecked = true;
        }
        private void DisableGameInfoUI()
        {
            gmInfoExecPath.IsEnabled = false;
            gmInfoSavePath.IsEnabled = false;
            textBoxMinuts.IsEnabled = false;
            textBoxHours.IsEnabled = false;
            MyGame_checkBox.IsChecked = false;
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
            FillGameListUI(games);
            GameComboBox.SelectedIndex = 0;
        }
    }
}
