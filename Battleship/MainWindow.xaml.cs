using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Timers;
using System.Diagnostics;
using System.Collections.Generic;

namespace Battleship
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool shipShadow = false;
        private int calculatedCell = -1;
        private string player1Name;

        private static readonly int rows = 10;
        private static readonly int columns = 10;

        Random rnd = new();

        char[,] playerPlayfield = new char[10, 10];
        char[,] aiPlayfield = new char[10, 10];

        bool aiShipsShow = false;

        bool left, right, down, up, con = false;

        int firstHitX, firstHitY, randomX, randomY;

        private int changePlayerCounter = 0;
        private int playerHits;

        private const double RefreshTimeSec = 10;
        private readonly DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Send);

        public MainWindow(Grid playfield, char[,] playerPlayfield, string player1Name)
        {
            InitializeComponent();

            this.playerPlayfield = playerPlayfield;
            this.player1Name = player1Name;
            playerShipsLoad(playfield);

            shipAI(rnd);

            //game(rnd);

            shipStatHpInit();
        }

        private int calculateCell() //which cell the cursor is on
        {
            var point = Mouse.GetPosition(rightTable);

            int row = 0;
            int col = 0;
            double accumulatedHeight = 0.0;
            double accumulatedWidth = 0.0;

            foreach (var rowDefinition in rightTable.RowDefinitions)
            {
                accumulatedHeight += rowDefinition.ActualHeight;
                if (accumulatedHeight >= point.Y)
                    break;
                row++;
            }

            foreach (var columnDefinition in rightTable.ColumnDefinitions)
            {
                accumulatedWidth += columnDefinition.ActualWidth;
                if (accumulatedWidth >= point.X)
                    break;
                col++;
            }

            return (row * 10) + col;
        }

        private void deleteShipShadow(int shipLength)
        {
            if (shipShadow == true)
            {
                for (int i = 0; i < shipLength; i++)
                {
                    int lastItem = rightTable.Children.Count - 1;
                    rightTable.Children.RemoveAt(lastItem);
                }
            }
        }

        private void onGridMouseOver(object sender, MouseEventArgs e) //ship shadow
        {
            int shipLength = 1;

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
                        var Y = rightTable.Width / rows;
                        var X = rightTable.Height / columns;
                        ship.Width = Y;
                        ship.Height = X;

                        Grid.SetRow(ship, cell / rows + i);
                        Grid.SetColumn(ship, cell % columns);

                        shipShadow = true;
                        rightTable.Children.Add(ship);
                    }
                }
            }
        }

        private void onGridMouseClick(object sender, MouseButtonEventArgs e) //ship placement in the playfield
        {
            if(e.ClickCount == 1)
            {
                int shipLength = 1;
                deleteShipShadow(shipLength);
                shipShadow = false;

                int cell = calculateCell();

                Debug.WriteLine($"{cell % columns}, {cell / rows}");

                for (int i = 0; i < shipLength; i++)
                {
                    if (char.IsDigit(aiPlayfield[cell % columns, cell / rows]))
                    {
                        var ship = shipSettings(shipLength);
                        ship.Fill = Brushes.DarkRed;
                        Grid.SetRow(ship, cell / rows);
                        Grid.SetColumn(ship, cell % columns);

                        Debug.WriteLine(aiPlayfield[cell % columns, cell / rows]);

                        char c = aiPlayfield[cell % columns, cell / rows];

                        aiPlayfield[cell % columns, cell / rows] = 'T';                        
                        
                        Debug.WriteLine(int.Parse(c.ToString()));

                        shipHp(int.Parse(c.ToString()));

                        ship.Visibility = Visibility.Visible;
                        rightTable.Children.Add(ship);

                        playerHits++;
                        playerHitsLabel.Content = playerHits;

                        if (isEndGame(0)) // player
                        {
                            onScore(player1Name);
                            MessageBox.Show("The Player won!", "Winner", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                            StartWindow startWindow = new StartWindow();
                            this.Close();
                            startWindow.Show();
                        }
                    }
                    else if(!(aiPlayfield[cell % columns, cell / rows] == 'T' || aiPlayfield[cell % columns, cell / rows] == 'V' ))
                    {
                        var ship = shipSettings(shipLength);
                        ship.Fill = Brushes.Gray;
                        Grid.SetRow(ship, cell / rows);
                        Grid.SetColumn(ship, cell % columns);

                        aiPlayfield[cell % columns, cell / rows] = 'V';

                        ship.Visibility = Visibility.Visible;
                        rightTable.Children.Add(ship);

                        roundsLabelIncrement();

                        Random rnd = new Random();
                        game(rnd);
                        if(isEndGame(1))
                        {
                            onScore("AI");
                            MessageBox.Show("The AI won!", "Loser", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                            StartWindow startWindow = new StartWindow();
                            this.Close();
                            startWindow.Show();
                        }
                    }
                }
            }     
        }

        private void onScore(string winner)
        {
            //score
            List<Score> scores = ScoreResult.ReadResult("score.json");
            if(scores == null)
            {
                scores = new List<Score>();
            }
            Score newScore = new()
            {
                Enemy = "AI",
                EnemyHits = Convert.ToInt32(computerHitsLabel.Content),
                Player = player1Name,
                PlayerHits = Convert.ToInt32(playerHitsLabel.Content),
                Rounds = Convert.ToInt32(roundsLabel.Content),
                Winner = winner
            };

            scores.Add(newScore);
            ScoreResult.WriteResult(scores, "score.json");
        }

        private void shipHp(int s)
        {
            Debug.WriteLine(s);

            if (s == 1)
            {
                destroyerHpGrid.Children.RemoveAt(destroyerHpGrid.Children.Count - 1);
            }
            else if (s == 2)
            {
                submarineHpGrid.Children.RemoveAt(submarineHpGrid.Children.Count - 1);
            }
            else if (s == 3)
            {
                cruiserHpGrid.Children.RemoveAt(cruiserHpGrid.Children.Count - 1);
            }
            else if (s == 4)
            {
                battleshipHpGrid.Children.RemoveAt(battleshipHpGrid.Children.Count - 1);
            }
            else if (s == 5)
            {
                carrierHpGrid.Children.RemoveAt(carrierHpGrid.Children.Count - 1);
            }
        }

        public bool isEndGame(int player)
        {
            if(player == 0)
            {
                for (int row = 0; row < 10; row++)
                {
                    for(int col =  0; col < 10; col++)
                    {
                        if(char.IsDigit(aiPlayfield[row, col]))
                        {
                            return false;
                        }
                    }
                }
            }
            else if(player == 1)
            {
                for (int row = 0; row < 10; row++)
                {
                    for (int col = 0; col < 10; col++)
                    {
                        if (char.IsDigit(playerPlayfield[row, col]))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void shipStatHpInit()
        {
           
            for (int ship = 5; ship > 0; ship--)
            {
                for (int unit = 0; unit < ship; unit++)
                {
                    Rectangle hpUnit = shipHpSettings(ship);

                    Grid.SetColumn(hpUnit, unit);

                    switch (ship)
                    {
                        case 5:
                            carrierHpGrid.Children.Add(hpUnit);
                            break;
                        case 4:
                            battleshipHpGrid.Children.Add(hpUnit);
                            break;
                        case 3:
                            cruiserHpGrid.Children.Add(hpUnit);
                            break;
                        case 2:
                            submarineHpGrid.Children.Add(hpUnit);
                            break;
                        case 1:
                            destroyerHpGrid.Children.Add(hpUnit);
                            break;
                    }
                }
            }
        }

        private Rectangle shipHpSettings(int shipLength)
        {
            Rectangle hpUnit = new()
            {
                Fill = Brushes.Green
            };
            var Y = carrierHpGrid.Width;
            var X = carrierHpGrid.Height / shipLength;
            hpUnit.Width = Y;
            hpUnit.Height = X;

            return hpUnit;
        }

        public MainWindow(Grid playfield, char[,] playerPlayfield, char[,] playerPlayfield2)
        {
            InitializeComponent();

            this.playerPlayfield = playerPlayfield;

            playerShipsLoad(playfield);


            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    Rectangle ship = new()
                    {
                        Fill = Brushes.Red
                    };
                    var Y = rightTable.Width / rows;
                    var X = rightTable.Height / columns;
                    ship.Width = Y;
                    ship.Height = X;

                    Grid.SetRow(ship, row);
                    Grid.SetColumn(ship, col);

                    if (char.IsDigit(playerPlayfield[row, col]))
                    {
                        rightTable.Children.Add(ship);
                    }
                }

            }

        }

        private void playerShipsLoad(Grid playfield)
        {
            for (int unit = playfield.Children.Count - 1; unit >= 0; unit--)
            {
                var child = playfield.Children[unit];
                playfield.Children.RemoveAt(unit);
                leftTable.Children.Add(child);
            }
        }

        private void shipAI(Random rnd)
        {
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
                        if ((randomPosX != 0 && char.IsDigit(aiPlayfield[randomPosY, randomPosX - 1])) || ((randomPosX + i - 1) != 9 && char.IsDigit(aiPlayfield[randomPosY, randomPosX + i])))
                        {
                            randomPosX = (int)rnd.Next(0, 10 - i + 1);
                            randomPosY = (int)rnd.Next(0, 10);
                        }
                        else
                        {
                            for (int k = 0; k < i; k++)
                            {
                                if (char.IsDigit(aiPlayfield[randomPosY, randomPosX + k]) || (randomPosY != 0 && char.IsDigit(aiPlayfield[randomPosY - 1, randomPosX + k])) || (randomPosY != 9 && char.IsDigit(aiPlayfield[randomPosY + 1, randomPosX + k])))
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

                        Grid.SetRow(ship, col + randomPosX);
                        Grid.SetColumn(ship, randomPosY);

                        aiPlayfield[randomPosY, col + randomPosX] = Convert.ToChar(i.ToString());
                        rightTable.Children.Add(ship);
                    }
                }
                else if (randomOrient == 1) //függőleges
                {
                    randomPosY = (int)rnd.Next(0, 10 - i + 1);
                    randomPosX = (int)rnd.Next(0, 10);

                    while (empty == false)
                    {
                        if ((randomPosY != 0 && char.IsDigit(aiPlayfield[randomPosY - 1, randomPosX])) || ((randomPosY + i - 1) != 9 && char.IsDigit(aiPlayfield[randomPosY + i, randomPosX])))
                        {
                            randomPosY = (int)rnd.Next(0, 10 - i + 1);
                            randomPosX = (int)rnd.Next(0, 10);
                        }
                        else
                        {
                            for (int k = 0; k < i; k++)
                            {
                                if (char.IsDigit(aiPlayfield[randomPosY + k, randomPosX]) || (randomPosX != 0 && char.IsDigit(aiPlayfield[randomPosY + k, randomPosX - 1])) || (randomPosX != 9 && char.IsDigit(aiPlayfield[randomPosY + k, randomPosX + 1])))
                                {
                                    randomPosY = (int)rnd.Next(0, 10 - i + 1);
                                    randomPosX = (int)rnd.Next(0, 10);
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

                    for (int row = 0; row < i; row++)
                    {
                        Rectangle ship = shipSettings(i);

                        Grid.SetRow(ship, randomPosX);
                        Grid.SetColumn(ship, row + randomPosY);

                        aiPlayfield[randomPosY + row, randomPosX] = Convert.ToChar(i.ToString());
                        rightTable.Children.Add(ship);
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
            var Y = rightTable.Width / rows;
            var X = rightTable.Height / columns;
            ship.Width = Y;
            ship.Height = X;

            shipSetName(ship, shipLength);

            ship.Visibility = Visibility.Hidden;

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

        private void aiVisibility_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.S))
            {
                aiShipsShow = !aiShipsShow;

                for (int unit = 0; unit < rightTable.Children.Count; unit++)
                {
                    if (aiShipsShow)
                    {
                        rightTable.Children[unit].Visibility = Visibility.Visible;
                    }
                    else
                    {
                        rightTable.Children[unit].Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        private void surrendBtn_Click(object sender, RoutedEventArgs e)
        {
            onScore("AI");
            StartWindow startWindow = new StartWindow();
            this.Close();
            startWindow.Show();
        }

        private void stats_Click(object sender, RoutedEventArgs e)
        {
            Stats stats = new();
            stats.Show();
        }

        private void game(Random rnd)
        {
            bool player = false; // false - AI | true - Player 
            bool isHit = false;

            while (!player)
            {
                if (!con)
                {
                    int cell = AiMethods.generateAiShoot(rnd, playerPlayfield);
                    randomY = cell / rows;
                    randomX = cell % columns;

                    isHit = shoot(randomY, randomX, "center");

                    if (isHit)
                    {
                        firstHitX = randomX;
                        firstHitY = randomY;
                    }
                }
                else
                {
                    isHit = true;
                }

                while (isHit) //ha eltalálta vagy olyan helyre lőtt ahová nem lehet vagy már lőtt oda
                {
                    con = true;

                    int direction = rnd.Next(0, 4);

                    switch (direction)
                    {
                        case 0:
                            while (!up)
                            {
                                if (shoot(randomY, randomX, "Up"))
                                {
                                    randomY++;
                                    computerHitsLabelIncerement();
                                }
                                else
                                {
                                    randomY = firstHitY;
                                    player = true;
                                    isHit = false;
                                    up = true;
                                    roundsLabelIncrement();
                                }
                            }
                            break;
                        case 1:
                            while (!down)
                            {
                                if (shoot(randomY, randomX, "Down"))
                                {
                                    randomY--;
                                    computerHitsLabelIncerement();
                                }
                                else
                                {
                                    randomY = firstHitY;
                                    player = true;
                                    isHit = false;
                                    down = true;
                                    roundsLabelIncrement();
                                }
                            }
                            break;
                        case 2:
                            while (!left)
                            {
                                if (shoot(randomY, randomX, "Left"))
                                {
                                    randomX--;
                                    computerHitsLabelIncerement();
                                }
                                else
                                {
                                    randomX = firstHitX;
                                    player = true;
                                    isHit = false;
                                    left = true;
                                    roundsLabelIncrement();
                                }
                            }
                            break;
                        case 3:
                            while (!right)
                            {
                                if (shoot(randomY, randomX, "Right"))
                                {
                                    randomX++;
                                    computerHitsLabelIncerement();
                                }
                                else
                                {
                                    randomX = firstHitX;
                                    player = true;
                                    isHit = false;
                                    right = true;
                                    roundsLabelIncrement();
                                }
                            }
                            break;
                    }

                    if (shipDestroyed(up, down, left, right))
                    {
                        break;
                    }
                    
                }

                player = true;
            }
        }

        private void roundsLabelIncrement()
        {
            changePlayerCounter++;
            if (changePlayerCounter % 2 == 0)
            {
                roundsLabel.Content = Convert.ToInt32(roundsLabel.Content) + 1;
            }
        }

        private void computerHitsLabelIncerement()
        {
            computerHitsLabel.Content = Convert.ToInt32(computerHitsLabel.Content) + 1;
        }

        private bool shipDestroyed(bool up, bool down, bool left, bool right)
        {
            if (up && down && left && right)
            {
                initializeDirection();
                con = false;

                return true;
            }

            return false;
        }

        private void initializeDirection()
        {
            up = false;
            down = false;
            left = false;
            right = false;
        }

        private bool shoot(int randomY, int randomX, string direction)
        {
            switch (direction)
            {
                case "Up":
                    randomY++;
                    break;
                case "Down":
                    randomY--;
                    break;
                case "Left":
                    randomX--;
                    break;
                case "Right":
                    randomX++;
                    break;
            }

            if (!AiMethods.isCellWall(randomX, randomY))
            {
                if (!AiMethods.isCellShootedAI(randomX, randomY, playerPlayfield))
                {
                    if (AiMethods.isHitPlayerShipUnit(randomX, randomY, playerPlayfield))
                    {
                        shootedCellChange(randomX, randomY, true);
                        paintHitCell(randomX, randomY);

                        return true;
                    }
                    else
                    {
                        shootedCellChange(randomX, randomY, false);
                        paintMissCell(randomX, randomY);

                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void shootedCellChange(int randomX, int randomY, bool isHit)
        {
            if (isHit)
            {
                playerPlayfield[randomY, randomX] = 'T';
            }
            else
            {
                playerPlayfield[randomY, randomX] = 'V';
            }
        }

        private void paintMissCell(int randomX, int randomY)
        {
            Rectangle ship = shipHpSettings(1);
            ship.Fill = Brushes.Gray;
            Grid.SetRow(ship, randomY);
            Grid.SetColumn(ship, randomX);

            leftTable.Children.Add(ship);
        }

        private void paintHitCell(int randomX, int randomY)
        {
            Rectangle ship = shipHpSettings(1);
            ship.Fill = Brushes.DarkRed;

            Grid.SetRow(ship, randomY);
            Grid.SetColumn(ship, randomX);

            leftTable.Children.Add(ship);
        }
    }
}