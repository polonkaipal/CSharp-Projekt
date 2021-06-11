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

namespace Battleship
{
    /// <summary>
    /// Interaction logic for Opponent.xaml
    /// </summary>
    public partial class Opponent : Window
    {
        String mode;

        public Opponent()
        {
            InitializeComponent();
        }

        private void openWindow()
        {
            ShipPlacement shipPlacementtWindow = new ShipPlacement(mode);
            App.Current.MainWindow = shipPlacementtWindow;
            this.Close();
            shipPlacementtWindow.Show();
        }

        private void pcBtn_Click(object sender, RoutedEventArgs e)
        {
            mode = "pc";
            openWindow();
        }

        private void ppBtn_Click (object sender, RoutedEventArgs e)
        {
            mode = "pp";
            openWindow();
        }
    }
}
