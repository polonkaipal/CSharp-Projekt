using System.Collections.Generic;
using System.Windows;

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
