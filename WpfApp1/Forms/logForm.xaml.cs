using System.Windows;
using System.Windows.Controls.Ribbon;
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
        private string _nickTextBox { get => NickTextBox.Text; }
        private string _passwTextBox { get => PasswTextBox.Text; }

        private void Reset() 
        {
            NickTextBox.Clear();
            PasswTextBox.Clear();
        }

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
            string descrHK = "Hollow Knight is a classically styled 2D action adventure across a vast interconnected world. Explore twisting caverns, ancient cities and deadly wastes; battle tainted creatures and befriend bizarre bugs; and solve ancient mysteries at the kingdom's heart.";


            DBreader.AddNewGame("Elden Ring", DateTime.Parse("2022/2/25"), descrElden);
            DBreader.AddNewGame("Titanfall 2", DateTime.Parse("2016/10/28"), descrTF2);
            DBreader.AddNewGame("METAL GEAR RISING: REVENGEANCE", DateTime.Parse("2013/2/19"), descrMGR);
            DBreader.AddNewGame("Tales of Berseria", DateTime.Parse("2016/8/18"), descrTOB);
            DBreader.AddNewGame("Hollow Knight", DateTime.Parse("2017/2/25"), descrHK);

            DBreader.AddNewPublisher("Bandai Namco Entertainment");
            DBreader.AddNewPublisher("Electronic Arts");
            DBreader.AddNewPublisher("Konami");
            DBreader.AddNewPublisher("Team Cherry");

            DBreader.BindGamePublisher(1, 1);
            DBreader.BindGamePublisher(2, 2);
            DBreader.BindGamePublisher(3, 3);
            DBreader.BindGamePublisher(4, 1);
            DBreader.BindGamePublisher(5, 4);

            DBreader.MyNewGame(1, profile_id: 1, 4500, @"E:\games\ELDEN RING\Game\eldenring.exe");
            DBreader.MyNewGame(2, profile_id: 1, 30000);
            DBreader.MyNewGame(3, profile_id: 1, 660);
            DBreader.MyNewGame(5, profile_id: 1, 0, @"E:\games\Hollow Knight\Hollow Knight.exe");

            DBreader.MyNewGame(1, profile_id: 2, 5);
            DBreader.MyNewGame(4, profile_id: 2, 3660);

            DBreader.UpdateMyGame(game_id: 5, profile_id: 1, minuts_in_game: 4500);
        }

        public MainWindow()
        {
            InitializeComponent();
            recreate();
            DBreader.CheckGameByName("asd");
            DBreader.CheckGameByName("Elden Ring");
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            string nick = _nickTextBox;
            string passw = _passwTextBox;

            bool result = DBreader.LogIn(nick, passw);

            if (result)
            {
                var p = DBreader.Get_profile(nick);
                Forms.MainForm new_form = new(p, this);
                this.Hide();
                new_form.ShowDialog();
                Reset();
            }
            else
                MessageBox.Show("login fail");

        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            string nick = _nickTextBox;
            string passw = _passwTextBox;

            bool result = DBreader.SignIn(nick, passw);

            if (result)
            {
                var p = DBreader.Get_profile(nick);
                Forms.MainForm new_form = new(p, this);
                Hide();
                new_form.ShowDialog();
                Reset();
            }
            else
                MessageBox.Show("SignIn fail");
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                this.DragMove();
        }
    }
}
