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
        bool shipHorizontal = false;

        char[,] battleshipPlayfield = new char[10, 10];

        public ShipPlacement()
        {
            InitializeComponent();
        }

        private void onGridMouseClick(object sender, MouseButtonEventArgs e) //ship placement in the playfield
        {
            if (e.ClickCount == 1)
            {
                int shipLength = shipLengthCalculate();
                deleteShipShadow(shipLength);
                shipShadow = false;

                for (int i = 0; i < shipLength; i++)
                {
                    int cell = calculateCell();

                    Rectangle ship = shipSettings();

                    bool shipPlacementEnoughSpace = true;

                    //enough space for the selected ship or not
                    shipPlacementEnoughSpace = !shipExtendsBeyond(cell, shipLength, shipHorizontal);

                    //collision with another ship
                    if (shipPlacementEnoughSpace)
                    {
                        shipPlacementEnoughSpace = !shipCollision(i, cell, shipLength, shipHorizontal);
                    }

                    if (shipPlacementEnoughSpace)
                    {
                        shipPlaceToPlayfield(ship, i, cell, shipHorizontal);
                        setSelectedShipButtonDisabled();
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void setSelectedShipButtonDisabled()
        {
            switch (selectedShip) //placed ship button set disabled
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

        private void shipPlaceToPlayfield(Rectangle ship, int i, int cell, bool shipHorizontal)
        {
            if (shipHorizontal)
            {
                Grid.SetRow(ship, cell / 10);
                Grid.SetColumn(ship, cell % 10 + i);

                //save the ship position
                battleshipPlayfield[cell / 10, cell % 10 + i] = 'H';
            }
            else if (!shipHorizontal)
            {
                Grid.SetRow(ship, cell / 10 + i);
                Grid.SetColumn(ship, cell % 10);

                //save the ship position
                battleshipPlayfield[cell / 10 + i, cell % 10] = 'H';
            }

            playfield.Children.Add(ship);
        }

        private bool shipExtendsBeyond(int cell, int shipLength, bool shipHorizontal)
        {
            if (shipHorizontal)
            {
                if (cell / 10 < 10 && cell % 10 + shipLength - 1 < 10)
                {
                    return false;
                }
            }
            else if (!shipHorizontal)
            {
                if (cell / 10 + shipLength - 1 < 10 && cell % 10 < 10)
                {
                    return false;
                }
            }

            return true;
        }

        private bool shipCollision(int i, int cell, int shipLength, bool shipHorizontal)
        {
            for (int unit = 0 + i; unit < shipLength; unit++)
            {
                if (shipHorizontal)
                {
                    if (battleshipPlayfield[cell / 10, cell % 10 + unit] == 'H')
                    {
                        return true;
                    }
                }
                else if (!shipHorizontal)
                {
                    if (battleshipPlayfield[cell / 10 + unit, cell % 10] == 'H')
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private Rectangle shipSettings()
        {
            Rectangle ship = new Rectangle();

            ship.Fill = Brushes.DodgerBlue;
            var Y = playfield.Width / rows;
            var X = playfield.Height / columns;
            ship.Width = Y;
            ship.Height = X;

            ship.Name = selectedShip;

            return ship;
        }

        private void shipBtn(object sender, RoutedEventArgs e) //select ship type
        {
            var ShipButton = (Button)sender;
            selectedShip = ShipButton.Content.ToString();
        }

        private void onGridMouseOver(object sender, MouseEventArgs e) //ship shadow
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

                        // horizontal/vertical ship alignment
                        if (!shipHorizontal)
                        {
                            Grid.SetRow(ship, cell / 10 + i);
                            Grid.SetColumn(ship, cell % 10);
                        }
                        else if (shipHorizontal)
                        {
                            Grid.SetRow(ship, cell / 10);
                            Grid.SetColumn(ship, cell % 10 + i);
                        }

                        shipShadow = true;
                        playfield.Children.Add(ship);
                    }
                }

            }
        }

        private int calculateCell() //which cell the cursor is on
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

        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            selectedShip = null;
            calculatedCell = -1;
            shipShadow = false;

            carrierBtn.IsEnabled = true;
            battleshipBtn.IsEnabled = true;
            cruiserBtn.IsEnabled = true;
            submarineBtn.IsEnabled = true;
            destroyerBtn.IsEnabled = true;

            playfield.Children.Clear();

            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    battleshipPlayfield[row, col] = '\0';
                }
            }
        }

        private void rotateBtn_Click(object sender, RoutedEventArgs e)
        {
            shipHorizontal = !shipHorizontal;
        }

        private void submitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (everyShipPlaced())
            {
                MainWindow battleshipPlayfieldWindow = new MainWindow();
                App.Current.MainWindow = battleshipPlayfieldWindow;
                this.Close();
                battleshipPlayfieldWindow.Show();
            }
            else
            {
                MessageBox.Show("All ships must be placed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private bool everyShipPlaced()
        {
            if (!carrierBtn.IsEnabled && !battleshipBtn.IsEnabled && !cruiserBtn.IsEnabled && !submarineBtn.IsEnabled && !destroyerBtn.IsEnabled)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}