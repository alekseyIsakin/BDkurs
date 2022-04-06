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

namespace WpfApp1.Forms
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class MainForm : Window
    {
        private string Nick = "";
        Edit_game_form edit_game_form = new();
        Edit_publisher_form edit_publicher_form = new();

        public MainForm(string nick, Window parent)
        {
            InitializeComponent();
        }

        private void edit_game_click(object sender, RoutedEventArgs e)
        {

        }

        private void edit_publisher_click(object sender, RoutedEventArgs e)
        {

        }
    }
}
