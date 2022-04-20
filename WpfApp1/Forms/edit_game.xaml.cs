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
        Game selectedGame;
        GameInfo selectedGameInfo;
        public EditGameForm(int _game, int _profile)
        {
            InitializeComponent();
            FillGameList();
        }

        private void SetupControlGameData(Game gm) 
        {
        
        }

        private void FillGameList() 
        {
            List<Game> games = DBreader.Get_games();
            List<ListBoxItem> listBoxItems = new();

            foreach (var g in games)
            {
                ListBoxItem li = new();
                li.Content = g.Title;
                li.DataContext = g.ID;
                //li.Selected += LiSelected;
                listBoxItems.Add(li);
            };
            GameComboBox.ItemsSource = listBoxItems;
        }
    }
}
