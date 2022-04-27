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

namespace WpfApp1.Forms.MyControls
{
    /// <summary>
    /// Логика взаимодействия для TempPopup.xaml
    /// </summary>
    public partial class TempPopup : System.Windows.Controls.Primitives.Popup
    {
        private System.Windows.Threading.DispatcherTimer timer = new();
        const int startAlpha = 400;
        int alpha = 255;
        byte step = 5;
        public TempPopup()
        {
            InitializeComponent();
            DataContext = this;
            timer.Tick += timerTick;
            timer.Interval = TimeSpan.FromMilliseconds(25);
        }
        public void Show(string msg) 
        {
            IsOpen = false;
            alpha = startAlpha;
            textLabel.Content = msg;
            IsOpen = true;
            timer.Start();

        }
        private void timerTick(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() => 
            {
                Panel.Background = new SolidColorBrush(Color.FromArgb(
                    (byte)Math.Min(255, alpha), 
                    0, 255,0));
                alpha -= step;

                if (alpha < step) 
                {
                    IsOpen = false;
                    timer.Stop();
                }
            }));
        }

        private void Popup_MouseEnter(object sender, MouseEventArgs e)
        {
            timer.Stop();
        }

        private void Popup_MouseLeave(object sender, MouseEventArgs e)
        {
            timer.Start();
        }
    }
}
