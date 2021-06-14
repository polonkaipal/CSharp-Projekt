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
    /// Interaction logic for Stats.xaml
    /// </summary>
    public partial class Stats : Window
    {
        public List<Score> result;
        public Stats()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            result = ScoreResult.ReadResult("score.json");
            table.ItemsSource = result;
        }
    }
}
