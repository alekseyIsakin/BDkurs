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
        public void recreate() 
        {
            if (DBreader.IsCreate)
                System.IO.File.Delete(DBreader.db_name);
            DBreader.Create();

            DBreader.SignIn("w0m0", "123");
            DBreader.SignIn("kar", "456");

            string descrElden = @"THE NEW FANTASY ACTION RPG. Rise, Tarnished, and be guided by grace to brandish the power of the Elden Ring and become an Elden Lord in the Lands Between.";
            string descrTF2 = "Respawn Entertainment gives you the most advanced titan technology in its new, single player campaign & multiplayer experience. Combine & conquer with new titans & pilots, deadlier weapons, & customization and progression systems that help you and your titan flow as one unstoppable killing force.";
            string descrMGR = "Developed by Kojima Productions and PlatinumGames, METAL GEAR RISING: REVENGEANCE takes the renowned METAL GEAR franchise into exciting new territory with an all-new action experience. The game seamlessly melds pure action and epic story-telling that surrounds Raiden – a child soldier transformed into a half-human, half-cyborg ninja who uses his High Frequency katana blade to cut through any thing that stands in his vengeful path!";
            string descrTOB = "Players embark on a journey of self-discovery as they assume the role of Velvet, a young woman whose once kind demeanor has been replaced and overcome with a festering anger and hatred after a traumatic experience three years prior to the events within Tales of Berseria.";

            DBreader.Add_new_game("Elden Ring", "2022/2/25", descrElden);
            DBreader.Add_new_game("Titanfall 2", "2016/10/28", descrTF2);
            DBreader.Add_new_game("METAL GEAR RISING: REVENGEANCE", "2014/1/10", descrMGR);
            DBreader.Add_new_game("Tales of Berseria", "2017/1/27", descrTOB);

            DBreader.Add_new_publisher("Bandai Namco Entertainment");
            DBreader.Add_new_publisher("Electronic Arts");
            DBreader.Add_new_publisher("Konami");

            DBreader.Bound_game_publisher(1, 1);
            DBreader.Bound_game_publisher(2, 2);
            DBreader.Bound_game_publisher(3, 3);
            DBreader.Bound_game_publisher(4, 1);

            DBreader.My_new_game(1, 1);
            DBreader.My_new_game(2, 1);
            DBreader.My_new_game(3, 1);
            DBreader.My_new_game(1, 2);
            DBreader.My_new_game(4, 2);

        }

        public MainWindow()
        {
            InitializeComponent();
            recreate();
            //Forms.playground pg = new();
            //pg.Show();
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            string nick = NickTextBox.Text;
            string passw = PasswTextBox.Text;

            bool result = DBreader.LogIn(nick, passw);

            if (result)
            {
                var p = DBreader.Get_profile(nick);
                Forms.MainForm new_form = new(p, this); 
                //this.Hide();
                new_form.ShowDialog();
                Close();
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
                var p = DBreader.Get_profile(nick);
                Forms.MainForm new_form = new(p, this);
                new_form.Show();
                //this.Hide();
            }
            else
                MessageBox.Show("SignIn fail");
        }
    }
}
