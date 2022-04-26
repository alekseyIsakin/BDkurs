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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1.Forms.Controls
{
    /// <summary>
    /// Логика взаимодействия для TempPopup.xaml
    /// </summary>
    public partial class TempPopup : System.Windows.Controls.Primitives.Popup
    {
        private System.Windows.Threading.DispatcherTimer timer = new();
        public string Text { get; set; } = "empty";
        byte alpha = 255;
        byte step = 5;
        public TempPopup()
        {
            InitializeComponent();
            DataContext = this;
        }
        public void Show() 
        {
            timer.Interval = TimeSpan.FromMilliseconds(25);
            timer.Start();
            timer.Tick += timerTick;
        }
        private void timerTick(object? sender, EventArgs e)
        {
            Panel.Background = new SolidColorBrush(Color.FromArgb(alpha, 0, 255,0));
            alpha -= step;

            if (alpha < step) 
            {
                timer.Stop();
                IsOpen = false;
            }
        }
    }
}
