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
        Edit_game_form edit_game_form = new();
        Edit_publisher_form edit_publicher_form = new();

        public MainForm(Profile profile, Window parent)
        {
            InitializeComponent();
            _profile = profile;

            var my_games = DBreader.Get_my_games_info(_profile.ID);
            List<ListBoxItem> listBoxItems = new();
            foreach (var g in my_games)
            {
                ListBoxItem li = new();
                li.Content = g.game_title;
                listBoxItems.Add(li);
            };
            MyGamesList.ItemsSource = listBoxItems;
        }

        private void edit_game_click(object sender, RoutedEventArgs e)
        {

        }

        private void edit_publisher_click(object sender, RoutedEventArgs e)
        {

        }
    }
}
