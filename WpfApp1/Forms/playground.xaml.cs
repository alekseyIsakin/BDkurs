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
using System.Windows.Shapes;

namespace WpfApp1.Forms
{
    /// <summary>
    /// Логика взаимодействия для playground.xaml
    /// </summary>
    public partial class playground : Window
    {
        public List<DBcore.Game> l_games;
        public playground()
        {
            InitializeComponent();
            l_games = DBcore.DBreader.Get_games();
            
            GameGrid.ItemsSource = l_games;
        }
    }
}
