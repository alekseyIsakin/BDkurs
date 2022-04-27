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
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class publisherNew : UserControl
    {
        public event EventHandler NewPublisher;
        public publisherNew()
        {
            InitializeComponent();
        }

        public void NewPublisherClick(object sender, EventArgs args) 
        {
            var PublName = publisherTextBox.Text;
            if (PublName == "") return;

            Publisher newPubl = new() { ID = -1, Name = PublName };
            NewPublisher?.Invoke(this, new PublisherEventArgs(newPubl));
        }
    }
}
