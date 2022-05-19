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
        private List<GameInfo> myGames => DBreader.Get_my_game_infos(DBreader.ActiveProfile.ID);

        private Game selectedGame;
        private GameInfo selectedGameInfo;
        private DateTime lastRunningTime;

        private bool onlyMyGamesIsShow = true;
        public MainForm(Window parent)
        {
            InitializeComponent();
            _login_form = parent;

            advanceFilter.ReloadPublishers();
            FillGamePanel(advanceFilter.GetFilteredGames());
        }

        private void LiSelected(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as ListBoxItem).DataContext);
            selectedGame = DBreader.Get_Game(id);
            UpdateGameRepr();
        }

        private void UpdateGameRepr()
        {
            labelGameTitle.Content = selectedGame.Title;

            textDescription.Text = "\nPublishers:\n";
            foreach (Publisher publ in selectedGame.Publishers ?? new() { })
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
                PlayButton.ToolTip = selectedGameInfo.executable_file;

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

            //if (onlyMyGamesIsShow)
            //    ShowMyGames();
            //else
            //    ShowAllGames();
            //ReloadMyGames();
            advanceFilter.ReloadPublishers();
            FillGamePanel(GetFilteredGames());
            UpdateGameRepr();
        }
        private void ToggleAdvanceFilter_Click(object sender, RoutedEventArgs e)
        {
            SetFilterVisibility(!filterPopup.IsOpen);
        }

        private void SetFilterVisibility(bool state)
        {
            filterPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;

            if (state)
            {
                toggleFilterBtn.Content = "Hide";
                filterPopup.IsOpen = true;
            }
            else
            {
                toggleFilterBtn.Content = "Show Filter";
                filterPopup.IsOpen = false;
            }
        }
        private List<Game> GetFilteredGames()
        {
            return advanceFilter.GetFilteredGames();
        }
        private void advanceFilter_Apply(object sender, EventArgs e)
        {
            List<Game> games = ((MyControls.GameFilterEventArg)e).newGames;
            FillGamePanel(games);
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameInfo.executable_file == "") return;

            BackUpSaveFile(".backup1");
            SetFilterVisibility(false);
            lastRunningTime = DateTime.Now;
            WindowState = WindowState.Minimized;

            System.Threading.Thread thread = new(() =>
            {
                System.Diagnostics.Process proc = new();
                proc.StartInfo.FileName = selectedGameInfo.executable_file;

                try
                {
                    proc.Start();
                    proc.WaitForExit();
                }
                catch (Exception excep)
                {
                    MessageBox.Show($"Error [{excep.Message}]", "Warning");
                }
                proc.Dispose();
                this.ThreadEnd();
            });
            thread.Start();
        }
        private void ThreadEnd()
        {
            this.Dispatcher.Invoke(()=>
            {
                TimeSpan gameTime = DateTime.Now - lastRunningTime;
                selectedGameInfo.time_in_game += (ulong)gameTime.TotalMinutes;
                DBreader.UpdateMyGame(selectedGameInfo);

                //ReloadMyGames();
                if (onlyMyGamesIsShow)
                    ShowMyGames();
                else
                    ShowAllGames();
                UpdateGameRepr();
                WindowState = WindowState.Normal;

                BackUpSaveFile(".backup2");
            });
        }

        private void BackUpSaveFile(string postfix = "")
        {
            if (selectedGameInfo.save_file != "")
            {
                try
                {
                    System.IO.File.Copy(selectedGameInfo.save_file, selectedGameInfo.save_file + postfix, overwrite: true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Backup saveFile error: [{ ex.Message}]");
                }
            }
        }
        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (!filterPopup.IsOpen)
                return;

            if (filterPopup.VerticalOffset == 0.1)
                filterPopup.VerticalOffset = 0;
            else
                filterPopup.VerticalOffset = 0.1;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetFilterVisibility(false);
        }
    }
}
