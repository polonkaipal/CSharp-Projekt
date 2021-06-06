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
    /// Interaction logic for ShipPlacement.xaml
    /// </summary>
    public partial class ShipPlacement : Window
    {
        public ShipPlacement()
        {
            InitializeComponent();
        }

        private void onGridMouseClick(object sender, MouseButtonEventArgs e)
        {
            //Grid square = (Grid)sender;
            //melyikcellaLabel.Content = square.Name;

            if (e.ClickCount == 1)
            {
                var point = Mouse.GetPosition(leftTable);

                int row = 0;
                int col = 0;
                double accumulatedHeight = 0.0;
                double accumulatedWidth = 0.0;

                foreach (var rowDefinition in leftTable.RowDefinitions)
                {
                    accumulatedHeight += rowDefinition.ActualHeight;
                    if (accumulatedHeight >= point.Y)
                        break;
                    row++;
                }

                // calc col mouse was over
                foreach (var columnDefinition in leftTable.ColumnDefinitions)
                {
                    accumulatedWidth += columnDefinition.ActualWidth;
                    if (accumulatedWidth >= point.X)
                        break;
                    col++;
                }

            }
        }
    }
}
