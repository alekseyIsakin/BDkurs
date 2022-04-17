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
        Edit_game_form edit_game_form = new();
        Edit_publisher_form edit_publicher_form = new();
        List<GameInfo> my_games;
        public MainForm(Profile profile, Window parent)
        {
            InitializeComponent();
            _profile = profile;
            _login_form = parent;

            my_games = DBreader.Get_my_games_info(_profile.ID);
            List<ListBoxItem> listBoxItems = new();

            foreach (var g in my_games)
            {
                ListBoxItem li = new();
                li.Content = g.game_title;
                li.DataContext = g.game_id;
                li.Selected += Li_Selected;
                listBoxItems.Add(li);
            };
            MyGamesList.ItemsSource = listBoxItems;
        }

        private void Li_Selected(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as ListBoxItem).DataContext);

            Game gm = DBreader.Get_Game(id);

            labelGameTitle.Content = gm.Title;

            textDescription.Text = "\nPublishers:\n";
            foreach (Publisher publ in gm.Publishers)
                textDescription.Text += publ.Name;

            textDescription.Text += "\n\nDescriptions:\n";
            textDescription.Text += gm.Descriptions;
        }

        private void edit_game_click(object sender, RoutedEventArgs e)
        {

        }

        private void edit_publisher_click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }

        private void ToggleMyGamesList_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
