using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Battleship
{
    /// <summary>
    /// Interaction logic for ShipPlacement.xaml
    /// </summary>
    public partial class ShipPlacement : Window
    {
        private string selectedShip = null;
        private char selectedShipUnit;
        private int rows = 10;
        private int columns = 10;
        private int calculatedCell = -1;
        private bool shipShadow = false;
        private bool shipHorizontal = false;

        private char[,] battleshipPlayfield = new char[10, 10];

        private bool vsComputer;
        private bool player2PlaceShips = false;
        private string player1Name;
        private string player2Name;
        private char[,] player1BattleshipPlayfield = new char[10, 10];
        private Grid player1PlayfieldGrid;

        public ShipPlacement(string player1Name)
        {
            InitializeComponent();

            vsComputer = true;
            this.player1Name = player1Name;

            this.Title = player1Name + "'s ship placement";
            welcomeLabel.Content = player1Name + "'s ship placement";
        }

        public ShipPlacement(string player1Name, string player2Name)
        {
            InitializeComponent();

            vsComputer = false;
            this.player1Name = player1Name;
            this.player2Name = player2Name;

            this.Title = player1Name + "'s ship placement";
            welcomeLabel.Content = player1Name + "'s ship placement";
        }

        public ShipPlacement(string player1Name, string player2Name, Grid playfield, char[,] battleshipPlayfield)
        {
            InitializeComponent();

            player2PlaceShips = true;
            this.player1Name = player1Name;
            this.player2Name = player2Name;
            this.player1BattleshipPlayfield = battleshipPlayfield;
            this.player1PlayfieldGrid = playfield;

            this.Title = player2Name + "'s ship placement";
            welcomeLabel.Content = player2Name + "'s ship placement";
        }

        private void onGridMouseClick(object sender, MouseButtonEventArgs e) //ship placement in the playfield
        {
            if (e.ClickCount == 1)
            {
                int shipLength = shipLengthCalculate();
                deleteShipShadow(shipLength);
                shipShadow = false;

                bool shipPlacementEnoughSpace = true;

                for (int i = 0; i < shipLength; i++)
                {
                    int cell = calculateCell();

                    Rectangle ship = shipSettings();

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
                    }
                    else
                    {
                        break;
                    }
                }

                if (shipPlacementEnoughSpace)
                {
                    setSelectedShipButtonDisabled();
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
                Grid.SetRow(ship, cell / rows);
                Grid.SetColumn(ship, cell % columns + i);

                //save the ship position
                battleshipPlayfield[cell / rows, cell % columns + i] = selectedShipUnit;
            }
            else if (!shipHorizontal)
            {
                Grid.SetRow(ship, cell / rows + i);
                Grid.SetColumn(ship, cell % columns);

                //save the ship position
                battleshipPlayfield[cell / rows + i, cell % columns] = selectedShipUnit;
            }

            playfield.Children.Add(ship);
        }

        private bool shipExtendsBeyond(int cell, int shipLength, bool shipHorizontal)
        {
            if (shipHorizontal)
            {
                if (cell / rows < rows && cell % columns + shipLength - 1 < columns)
                {
                    return false;
                }
            }
            else if (!shipHorizontal)
            {
                if (cell / rows + shipLength - 1 < rows && cell % columns < columns)
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
                    if (char.IsDigit(battleshipPlayfield[cell / rows, cell % columns + unit]))
                    {
                        return true;
                    }
                }
                else if (!shipHorizontal)
                {
                    if (char.IsDigit(battleshipPlayfield[cell / rows + unit, cell % columns]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private Rectangle shipSettings()
        {
            Rectangle ship = new()
            {
                Fill = Brushes.DodgerBlue
            };
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

            switch (selectedShip)
            {
                case "Carrier":
                    selectedShipUnit = '5';
                    break;
                case "Battleship":
                    selectedShipUnit = '4';
                    break;
                case "Cruiser":
                    selectedShipUnit = '3';
                    break;
                case "Submarine":
                    selectedShipUnit = '2';
                    break;
                case "Destroyer":
                    selectedShipUnit = '1';
                    break;
            }
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
                        Rectangle shadow = shadowUnitSettings();

                        // horizontal/vertical ship alignment
                        if (!shipHorizontal)
                        {
                            Grid.SetRow(shadow, cell / rows + i);
                            Grid.SetColumn(shadow, cell % columns);
                        }
                        else if (shipHorizontal)
                        {
                            Grid.SetRow(shadow, cell / rows);
                            Grid.SetColumn(shadow, cell % columns + i);
                        }

                        shipShadow = true;
                        playfield.Children.Add(shadow);
                    }
                }
            }
        }

        private Rectangle shadowUnitSettings()
        {
            Rectangle shadow = new()
            {
                Fill = Brushes.LightGray
            };
            var Y = playfield.Width / rows;
            var X = playfield.Height / columns;
            shadow.Width = Y;
            shadow.Height = X;

            return shadow;
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
            int length = selectedShip switch
            {
                "Carrier" => 5,
                "Battleship" => 4,
                "Cruiser" => 3,
                "Submarine" => 2,
                "Destroyer" => 1,
                _ => 0,
            };
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

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
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
                if (player2PlaceShips)
                {
                    PvP player1BattleshipPlayfieldWindow = new(player1Name, player1PlayfieldGrid, player1BattleshipPlayfield, player2Name, playfield, battleshipPlayfield);
                    App.Current.MainWindow = player1BattleshipPlayfieldWindow;
                    this.Close();
                    player1BattleshipPlayfieldWindow.Show();
                }
                else if (vsComputer)
                {
                    MainWindow battleshipPlayfieldWindow = new(playfield, battleshipPlayfield);
                    App.Current.MainWindow = battleshipPlayfieldWindow;
                    this.Close();
                    battleshipPlayfieldWindow.Show();
                }
                else if (!vsComputer)
                {
                    ShipPlacement player2ShipPlacementWindow = new(player1Name, player2Name, playfield, battleshipPlayfield);
                    App.Current.MainWindow = player2ShipPlacementWindow;
                    this.Close();
                    player2ShipPlacementWindow.Show();
                }
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

        private void randomBtn_Click(object sender, RoutedEventArgs e)
        {
            playfield.Children.Clear();

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    battleshipPlayfield[row, col] = '\0';
                }
            }

            carrierBtn.IsEnabled = false;
            battleshipBtn.IsEnabled = false;
            cruiserBtn.IsEnabled = false;
            submarineBtn.IsEnabled = false;
            destroyerBtn.IsEnabled = false;

            Random rnd = new();

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
                        if ((randomPosX != 0 && char.IsDigit(battleshipPlayfield[randomPosY, randomPosX-1])) || ((randomPosX + i - 1)!= 9 && char.IsDigit(battleshipPlayfield[randomPosY, randomPosX + i])))
                        {
                            randomPosX = (int)rnd.Next(0, 10 - i + 1);
                            randomPosY = (int)rnd.Next(0, 10);
                        }
                        else
                        {
                            for (int k = 0; k < i; k++)
                            {
                                if (char.IsDigit(battleshipPlayfield[randomPosY, randomPosX + k]) || (randomPosY != 0 && char.IsDigit(battleshipPlayfield[randomPosY - 1, randomPosX + k])) || (randomPosY != 9 && char.IsDigit(battleshipPlayfield[randomPosY + 1, randomPosX + k])))
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
                        Rectangle ship = shipSettings(i);

                        Grid.SetRow(ship, randomPosY);
                        Grid.SetColumn(ship, col + randomPosX);

                        battleshipPlayfield[randomPosY, randomPosX + col] = Convert.ToChar(i.ToString());
                        playfield.Children.Add(ship);
                    }
                }
                else if (randomOrient == 1) //függőleges
                {
                    randomPosY = (int)rnd.Next(0, 10 - i + 1);
                    randomPosX = (int)rnd.Next(0, 10);

                    while (empty == false)
                    {
                        if ((randomPosY != 0 && char.IsDigit(battleshipPlayfield[randomPosY - 1, randomPosX])) || ((randomPosY + i - 1) != 9 && char.IsDigit(battleshipPlayfield[randomPosY + i, randomPosX])))
                        {
                            randomPosY = (int)rnd.Next(0, 10 - i + 1);
                            randomPosX = (int)rnd.Next(0, 10);
                        }
                        else
                        {
                            for (int k = 0; k < i; k++)
                            {
                                if (char.IsDigit(battleshipPlayfield[randomPosY + k, randomPosX]) || (randomPosX != 0 && char.IsDigit(battleshipPlayfield[randomPosY + k, randomPosX - 1])) || (randomPosX != 9 && char.IsDigit(battleshipPlayfield[randomPosY + k, randomPosX + 1])))
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
                        Rectangle ship = shipSettings(i);

                        Grid.SetRow(ship, row + randomPosY);
                        Grid.SetColumn(ship, randomPosX);

                        battleshipPlayfield[randomPosY + row, randomPosX] = Convert.ToChar(i.ToString());
                        playfield.Children.Add(ship);
                    }
                }
            }
        }

        private Rectangle shipSettings(int shipLength)
        {
            Rectangle ship = new()
            {
                Fill = Brushes.DodgerBlue
            };
            var Y = playfield.Width / rows;
            var X = playfield.Height / columns;
            ship.Width = Y;
            ship.Height = X;

            shipSetName(ship, shipLength);

            return ship;
        }

        private void shipSetName(Rectangle ship, int shipLength)
        {
            switch (shipLength)
            {
                case 5:
                    ship.Name = "Carrier";
                    break;
                case 4:
                    ship.Name = "Battleship";
                    break;
                case 3:
                    ship.Name = "Cruiser";
                    break;
                case 2:
                    ship.Name = "Submarine";
                    break;
                case 1:
                    ship.Name = "Destroyer";
                    break;
            }
        }
    }
}