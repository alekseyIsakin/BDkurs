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

namespace WpfApp1.Forms.Controls
{
    /// <summary>
    /// Логика взаимодействия для publisherCmb.xaml
    /// </summary>

    public partial class PublisherCmb : UserControl
    {
        public List<Publisher> publishers;
        public event EventHandler PublisherCmbSelected;
        public PublisherCmb()
        {
            try 
            {
                publishers = DBreader.Get_publishers();
            }
            catch
            {
                publishers = new();
            }
            publishers.Sort((x1, x2) => x1.Name.CompareTo(x2.Name));

            InitializeComponent();

            publisherCmb.ItemsSource = publishers;
            publisherCmb.SelectedIndex = 0;
        }

        private void PublisherCmb_Selected(object sender, RoutedEventArgs e)
        {
            Publisher publ = (Publisher)publisherCmb.SelectedItem;
            PublisherEventArgs arg = new(publ);

            PublisherCmbSelected?.Invoke(this, arg);
        }
    }
}
