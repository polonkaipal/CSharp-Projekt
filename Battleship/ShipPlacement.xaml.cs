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

                bool emptyCells = true;

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

                    bool shipPlacementEnoughSpace = true;

                    // horizontal/vertical ship alignment
                    if (!shipHorizontal)
                    {
                        //enough space for the selected ship or not
                        if (cell / 10 + shipLength - 1 < 10 && cell % 10 < 10)
                        {
                            //collision with another ship
                            for (int unit = 0 + i; unit < shipLength; unit++)
                            {
                                if (battleshipPlayfield[cell / 10 + unit, cell % 10] == 'H')
                                {
                                    emptyCells = false;
                                    break;
                                }
                            }

                            if (!emptyCells)
                            {
                                shipPlacementEnoughSpace = false;
                                break;
                            }

                            Grid.SetRow(ship, cell / 10 + i);
                            Grid.SetColumn(ship, cell % 10);

                            //save the ship position
                            battleshipPlayfield[cell / 10 + i, cell % 10] = 'H';
                        }
                        else
                        {
                            shipPlacementEnoughSpace = false;
                        }
                    }
                    else if (shipHorizontal)
                    {
                        //enough space for the selected ship or not
                        if (cell / 10 < 10 && cell % 10 + shipLength - 1 < 10)
                        {
                            //collision with another ship
                            for (int unit = 0 + i; unit < shipLength; unit++)
                            {
                                if (battleshipPlayfield[cell / 10, cell % 10 + unit] == 'H')
                                {
                                    emptyCells = false;
                                    break;
                                }
                            }

                            if (!emptyCells)
                            {
                                shipPlacementEnoughSpace = false;
                                break;
                            }

                            Grid.SetRow(ship, cell / 10);
                            Grid.SetColumn(ship, cell % 10 + i);

                            //save the ship position
                            battleshipPlayfield[cell / 10, cell % 10 + i] = 'H';
                        }
                        else
                        {
                            shipPlacementEnoughSpace = false;
                        }
                    }

                    if (shipPlacementEnoughSpace)
                    {
                        playfield.Children.Add(ship);

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
                }
            }
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

        private void randomBtn_Click(object sender, RoutedEventArgs e)
        {
            playfield.Children.Clear();

            Array.Clear(battleshipPlayfield, 0, battleshipPlayfield.Length);

            carrierBtn.IsEnabled = false;
            battleshipBtn.IsEnabled = false;
            cruiserBtn.IsEnabled = false;
            submarineBtn.IsEnabled = false;
            destroyerBtn.IsEnabled = false;

            Random rnd = new Random();

            int randomOrient;
            int randomPosX = (int)rnd.Next(0, 10);
            int randomPosY = (int)rnd.Next(0, 10);

            bool empty;

            for (int i = 5; i > 0; i--)
            {
                empty = false;

                randomOrient = (int)rnd.Next(0, 2);

                if (randomOrient == 0) // vízsintes
                {
                    randomPosX = (int)rnd.Next(0, 10 - i + 1);
                    randomPosY = (int)rnd.Next(0, 10);

                    while (empty == false)
                    {
                        if ((randomPosX != 0 && battleshipPlayfield[randomPosY, randomPosX-1] == 'H') || ((randomPosX + i - 1)!= 9 && battleshipPlayfield[randomPosY, randomPosX + i] == 'H'))
                        {
                            randomPosX = (int)rnd.Next(0, 10 - i + 1);
                            randomPosY = (int)rnd.Next(0, 10);
                        }
                        else
                        {
                            for (int k = 0; k < i; k++)
                            {
                                if (battleshipPlayfield[randomPosY, randomPosX + k] == 'H' || (randomPosY != 0 && battleshipPlayfield[randomPosY - 1, randomPosX + k] == 'H') || (randomPosY != 9 && battleshipPlayfield[randomPosY + 1, randomPosX + k] == 'H'))
                                {
                                    randomPosX = (int)rnd.Next(0, 10 - i + 1);
                                    randomPosY = (int)rnd.Next(0, 10);
                                    k = 0;
                                    break;
                                }
                                else if (k == (i - 1))
                                {
                                    empty = true;
                                }
                            }
                        }
                    }

                    for (int col = 0; col < i; col++)
                    {
                        var ship = new Rectangle();
                        ship.Fill = Brushes.DodgerBlue;
                        var Y = playfield.Width / rows;
                        var X = playfield.Height / columns;
                        ship.Width = Y;
                        ship.Height = X;

                        Grid.SetRow(ship, randomPosY);
                        Grid.SetColumn(ship, col + randomPosX);

                        battleshipPlayfield[randomPosY, randomPosX + col] = 'H';
                        playfield.Children.Add(ship);
                    }
                }
                else if (randomOrient == 1) //függőleges
                {
                    randomPosY = (int)rnd.Next(0, 10 - i + 1);
                    randomPosX = (int)rnd.Next(0, 10);

                    while (empty == false)
                    {
                        if ((randomPosY != 0 && battleshipPlayfield[randomPosY - 1, randomPosX] == 'H') || ((randomPosY + i - 1) != 9 && battleshipPlayfield[randomPosY + i, randomPosX] == 'H') )
                        {
                            randomPosY = (int)rnd.Next(0, 10 - i + 1);
                            randomPosX = (int)rnd.Next(0, 10);
                        }
                        else
                        {
                            for (int k = 0; k < i; k++)
                            {
                                if (battleshipPlayfield[randomPosY + k, randomPosX] == 'H' || (randomPosX != 0 && battleshipPlayfield[randomPosY + k, randomPosX - 1] == 'H') || (randomPosX != 9 && battleshipPlayfield[randomPosY + k, randomPosX + 1] == 'H'))
                                {
                                    randomPosY = (int)rnd.Next(0, 10 - i + 1);
                                    randomPosX = (int)rnd.Next(0, 10);
                                    k = 0;
                                    break;
                                }
                                else if(k == (i-1))
                                {
                                    empty = true;
                                }
                            }                            
                        }
                    }

                    for (int row = 0; row < i; row++)
                    {
                        var ship = new Rectangle();
                        ship.Fill = Brushes.DodgerBlue;
                        var Y = playfield.Width / rows;
                        var X = playfield.Height / columns;
                        ship.Width = Y;
                        ship.Height = X;

                        Grid.SetRow(ship, row + randomPosY);
                        Grid.SetColumn(ship, randomPosX);

                        battleshipPlayfield[randomPosY + row, randomPosX] = 'H';
                        playfield.Children.Add(ship);
                    }
                }
            }
        }
    }
}