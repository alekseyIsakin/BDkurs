using System.Windows;
using System.Windows.Media.Imaging;
using System;
using WpfApp1.DBcore;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            if (DBreader.IsCreate == false)
                DBreader.Create();
            Forms.playground pg = new();
            pg.Show();
            DBreader.Add_new_game("Elden Ring", "2022/2/25");
            DBreader.Add_new_game("Titanfall 2", "2016/10/28");
            DBreader.Add_new_game("METAL GEAR RISING: REVENGEANCE", "2014/1/10");
            DBreader.Add_new_game("Tales of Berseria", "2017/1/27");

            DBreader.Add_new_publisher("Bandai Namco Entertainment");
            DBreader.Add_new_publisher("Electronic Arts");
            DBreader.Add_new_publisher("Konami");

            DBreader.Bound_game_publisher(1, 1);
            DBreader.Bound_game_publisher(2, 2);
            DBreader.Bound_game_publisher(3, 3);
            DBreader.Bound_game_publisher(4, 1);
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            string nick = NickTextBox.Text;
            string passw = PasswTextBox.Text;

            bool result = DBreader.LogIn(nick, passw);

            if (result)
            {
                Forms.MainForm new_form = new(nick, this);
                new_form.Show();
            }
            else
                MessageBox.Show("login fail");

        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            string nick = NickTextBox.Text;
            string passw = PasswTextBox.Text;

            bool result = DBreader.SignIn(nick, passw);

            if (result)
            {
                Forms.MainForm new_form = new(nick, this);
                new_form.Show();
            }
            else
                MessageBox.Show("SignIn fail");
        }
    }
}
