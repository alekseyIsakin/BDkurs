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
using WpfApp1.DBcore;

namespace WpfApp1.Forms.MyControls
{
    /// <summary>
    /// Логика взаимодействия для publisherSetter.xaml
    /// </summary>
    public partial class publisherSetter : UserControl
    {
        public Publisher publisher { get; set; } = new();
        public event EventHandler DelClick;
        public publisherSetter()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public void RaiseDelClick() 
        {
            DelClick?.Invoke(this, new());
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DelClick?.Invoke(this, new());
        }
    }
}
