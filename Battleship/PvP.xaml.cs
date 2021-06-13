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
    /// Interaction logic for PvP.xaml
    /// </summary>
    public partial class PvP : Window
    {
        public PvP()
        {
            InitializeComponent();
        }

        public PvP(Grid player1PlayfieldGrid, char[,] player1BattleshipPlayfield, Grid playfield, char[,] battleshipPlayfield)
        {

        }
    }
}
