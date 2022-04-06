using System.Windows;
using System.Windows.Media.Imaging;
using System;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DBcore.DBreader dbreader = new();
        public MainWindow()
        {
            InitializeComponent();
            
            if (dbreader.IsCreate == false)
                dbreader.Create();
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            string nick = NickTextBox.Text;
            string passw = PasswTextBox.Text;

            bool result = dbreader.LogIn(nick, passw);

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

            bool result = dbreader.SignIn(nick, passw);

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
