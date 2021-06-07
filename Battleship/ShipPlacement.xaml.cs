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
        string selectedShip = null;
        int rows = 10;
        int columns = 10;
        int calculatedCell = -1;
        bool shipShadow = false;

        public ShipPlacement()
        {
            InitializeComponent();

        }

        private void onGridMouseClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                int shipLength = shipLengthCalculate();
                deleteShipShadow(shipLength);
                shipShadow = false;

                bool emptyCells = true;

                for (int i = 0; i < shipLength; i++)
                {
                    //foglalt e már a helye
                }
                    
                for (int i = 0; i < shipLength; i++)
                {
                    int cell = calculateCell();

                    //playfield.Children[2].GetValue(Name)

                    var ship = new Rectangle();
                    ship.Fill = Brushes.DodgerBlue;
                    var Y = playfield.Width / rows;
                    var X = playfield.Height / columns;
                    ship.Width = Y;
                    ship.Height = X;

                    ship.Name = selectedShip;

                    Grid.SetRow(ship, cell / 10 + i);
                    Grid.SetColumn(ship, cell % 10);

                    playfield.Children.Add(ship);
                }

                switch (selectedShip)
                {
                    case "Carrier":
                        carrierBtn.IsEnabled = false;
                        break;
                    case "Battleship":
                        battleshipBtn.IsEnabled = false;
                        break;
                    case "Cruiser":
                        cruiserBtn.IsEnabled = false;
                        break;
                    case "Submarine":
                        submarineBtn.IsEnabled = false;
                        break;
                    case "Destroyer":
                        destroyerBtn.IsEnabled = false;
                        break;
                }

                selectedShip = null;
            }
        }

        private void shipBtn(object sender, RoutedEventArgs e)
        {
            var ShipButton = (Button)sender;
            selectedShip = ShipButton.Content.ToString();
        }

        private void onGridMouseOver(object sender, MouseEventArgs e)
        {
            int shipLength = shipLengthCalculate();

            if (shipLength != 0)
            {
                int cell = calculateCell();

                if (calculatedCell != cell)
                {
                    calculatedCell = cell;

                    deleteShipShadow(shipLength);

                    for (int i = 0; i < shipLength; i++)
                    {
                        var ship = new Rectangle();
                        ship.Fill = Brushes.LightGray;
                        var Y = playfield.Width / rows;
                        var X = playfield.Height / columns;
                        ship.Width = Y;
                        ship.Height = X;
                
                        Grid.SetRow(ship, cell / 10 + i);
                        Grid.SetColumn(ship, cell % 10);

                        shipShadow = true;
                        playfield.Children.Add(ship);
                    }
                }

            }
        }

        private int calculateCell()
        {
            var point = Mouse.GetPosition(playfield);

            int row = 0;
            int col = 0;
            double accumulatedHeight = 0.0;
            double accumulatedWidth = 0.0;

            foreach (var rowDefinition in playfield.RowDefinitions)
            {
                accumulatedHeight += rowDefinition.ActualHeight;
                if (accumulatedHeight >= point.Y)
                    break;
                row++;
            }

            foreach (var columnDefinition in playfield.ColumnDefinitions)
            {
                accumulatedWidth += columnDefinition.ActualWidth;
                if (accumulatedWidth >= point.X)
                    break;
                col++;
            }

            return (row * 10) + col;
        }

        private int shipLengthCalculate()
        {
            int length = 0;

            switch (selectedShip)
            {
                case "Carrier":
                    length = 5;
                    break;
                case "Battleship":
                    length = 4;
                    break;
                case "Cruiser":
                    length = 3;
                    break;
                case "Submarine":
                    length = 2;
                    break;
                case "Destroyer":
                    length = 1;
                    break;
                default:
                    length = 0;
                    break;
            }

            return length;
        }

        private void deleteShipShadow(int shipLength)
        {
            if (shipShadow == true)
            {
                for (int i = 0; i < shipLength; i++)
                {
                    int lastItem = playfield.Children.Count - 1;
                    playfield.Children.RemoveAt(lastItem);
                }
            }
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            selectedShip = null;
            calculatedCell = -1;
            shipShadow = false;

            playfield.Children.Clear();
        }
    }
}
