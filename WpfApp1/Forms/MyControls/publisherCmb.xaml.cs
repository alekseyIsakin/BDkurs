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
    /// Логика взаимодействия для publisherCmb.xaml
    /// </summary>

    public partial class PublisherCmb : UserControl
    {
        public List<Publisher> publishers = new();
        public event EventHandler PublisherCmbSelected;
        public event EventHandler PublisherCmbDeleteClick;
        public PublisherCmb()
        {
            InitializeComponent();
            ReloadPublishers();
        }

        internal void DisableDelBtn()
        {
            DelBtn.IsEnabled = false;
        }

        public void ReloadPublishers()
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


            publisherCmb.ItemsSource = publishers;
            publisherCmb.SelectedIndex = 0;
        }
        private Publisher GetPublisher()
        {
            Publisher publ;
            var selected = publisherCmb.SelectedItem;

            if (selected != null)
                publ = (Publisher)selected;

            else
                publ = new()
                {
                    ID = -1,
                    Name = publisherCmb.Text
                };
            return publ;
        }

        private void PublisherCmb_Delete(object sender, RoutedEventArgs e)
        {
            Publisher publ = GetPublisher();
            PublisherCmbDeleteClick?.Invoke(this, new PublisherEventArgs(publ));
        }

        private void PublisherCmb_Selected(object sender, RoutedEventArgs e)
        {
            Publisher publ = GetPublisher();
            PublisherCmbSelected?.Invoke(this, new PublisherEventArgs(publ));
        }
    }
}
