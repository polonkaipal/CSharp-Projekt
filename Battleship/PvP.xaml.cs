using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Battleship
{
    /// <summary>
    /// Interaction logic for PvP.xaml
    /// </summary>
    public partial class PvP : Window
    {
        private int calculatedCell = -1;
        private int rows = 10;
        private int columns = 10;
        private bool shadowExists = false;
        private char[,] myPlayfield = new char[10, 10];
        private char[,] enemyPlayfield = new char[10, 10];
        private bool player1Coming;
        private bool windowPlayer1;

        private int changePlayerCounter = 0;

        private string player1Name;
        private string player2Name;

        PvP player2Window;
        Random rnd = new Random();

        public delegate string Hit(int cell);
        public event Hit OnHit;

        public delegate void CloseWindow();
        public event CloseWindow onCloseWindow;

        public PvP(string player1Name, Grid player1PlayfieldGrid, char[,] player1Playfield, string player2Name, Grid player2PlayfieldGrid, char[,] player2Playfield)
        {
            InitializeComponent();
            this.Title = player1Name;
            this.myPlayfield = player1Playfield;

            this.player1Name = player1Name;
            this.player2Name = player2Name;
            windowPlayer1 = true;
            string playerStart = whichPlayerStart(player1Name, player2Name);

            player2Window = new PvP(player1Name, player2Name, player2PlayfieldGrid, player2Playfield, player1Coming);
            player2Window.Title = player2Name;
            player2Window.Show();

            initializeLabels(player1Name, player2Name, playerStart);

            shipStatHpInit();
            playerShipsLoad(player1PlayfieldGrid);

            player2Window.OnHit += new Hit(this.onShoot);
            this.OnHit += new Hit(player2Window.onShoot);

            player2Window.onCloseWindow += new CloseWindow(this.onClose);
            this.onCloseWindow += new CloseWindow(player2Window.onClose);

        }

        public PvP(string player1Name, string player2Name, Grid player2PlayfieldGrid, char[,] player2Playfield, bool player1Coming)
        {
            InitializeComponent();

            windowPlayer1 = false;
            this.myPlayfield = player2Playfield;
            this.player1Coming = player1Coming;
            this.player1Name = player1Name;
            this.player2Name = player2Name;

            shipStatHpInit();
            playerShipsLoad(player2PlayfieldGrid);
        }

        private string whichPlayerStart(string player1Name, string player2Name)
        {
            if (rnd.Next(0, 2) == 0)
            {
                player1Coming = true;
                return player1Name;
            }
            else
            {
                player1Coming = false;
                return player2Name;
            }
        }

        private void initializeLabels(string player1Name, string player2Name, string playerStart)
        {
            player1infoLabel.Content = player1Name + " Hits:";
            player2infoLabel.Content = player2Name + " Hits:";

            player2Window.player1infoLabel.Content = player1Name + " Hits:";
            player2Window.player2infoLabel.Content = player2Name + " Hits:";

            playerComingLabel.Content = playerStart + " is coming";
            player2Window.playerComingLabel.Content = playerStart + " is coming";
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

        public string onShoot(int cell)
        {
            bool isHit = isHitShipUnit(cell);

            setShipUnit(cell, isHit, true);

            if (isHit)
            {
                hitsLabelChange();
                return myPlayfield[cell / rows, cell % columns].ToString();
            }

            player1Coming = !player1Coming;
            roundsLabelChange();
            whichPlayerComingLabelChange();

            return "false";
        }

        private bool isHitShipUnit(int cell)
        {
            if (char.IsDigit(myPlayfield[cell / rows, cell % columns]))
            {
                return true;
            }

            return false;
        }

        private void setShipUnit(int cell, bool isHit, bool setLeftTable)
        {
            Rectangle ship = shipUnitSettings(isHit);

            Grid.SetRow(ship, cell / rows);
            Grid.SetColumn(ship, cell % columns);

            if (setLeftTable)
            {
                leftTable.Children.Add(ship);
            }
            else
            {
                rightTable.Children.Add(ship);
            }
        }

        private Rectangle shipUnitSettings(bool isHit)
        {
            Rectangle unit = new Rectangle();

            if (isHit)
            {
                unit.Fill = Brushes.DarkRed;
            }
            else
            {
                unit.Fill = Brushes.LightGray;
            }

            var Y = unit.Width / rows;
            var X = unit.Height / columns;
            unit.Width = Y;
            unit.Height = X;

            return unit;
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

        private void onGridMouseClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                if (player1Coming && windowPlayer1 || !player1Coming && !windowPlayer1)
                {
                    deleteShadow();
                    shadowExists = false;

                    int cell = calculateCell();

                    bool shooted = isCellShooted(cell);

                    if (!shooted)
                    {
                        string shipUnitName = this.OnHit(cell);

                        if (shipUnitName != "false")
                        {
                            setShipUnit(cell, true, false);
                            shipHpDecrement(shipUnitName);
                            enemyPlayfield[cell / rows, cell % columns] = 'T';

                            hitsLabelChange();
                            everyShipDestroyed();
                        }
                        else
                        {
                            setShipUnit(cell, false, false);
                            enemyPlayfield[cell / rows, cell % columns] = 'V';

                            player1Coming = !player1Coming;
                            roundsLabelChange();
                            whichPlayerComingLabelChange();
                        }
                    }
                }
            }
        }

        private void everyShipDestroyed()
        {
            if (player1HitsLabel.Content.ToString() == "15")
            {
                MessageBox.Show(player1Name + " won the game!", "The game is over", MessageBoxButton.OK);
                gameEnd(player1Name);
            }
            else if (player2HitsLabel.Content.ToString() == "15")
            {
                MessageBox.Show(player2Name + " won the game!", "The game is over", MessageBoxButton.OK);
                gameEnd(player2Name);
            }
        }

        private void roundsLabelChange()
        {
            changePlayerCounter++;

            if (changePlayerCounter % 2 == 0)
            {
                roundsLabel.Content = Convert.ToInt32(roundsLabel.Content) + 1;
            }
        }

        private void hitsLabelChange()
        {
            if (windowPlayer1 && player1Coming)
            {
                player1HitsLabel.Content = Convert.ToInt32(player1HitsLabel.Content) + 1;
            }
            else if (!windowPlayer1 && !player1Coming)
            {
                player2HitsLabel.Content = Convert.ToInt32(player2HitsLabel.Content) + 1;
            }

            if (windowPlayer1 && !player1Coming)
            {
                player2HitsLabel.Content = Convert.ToInt32(player2HitsLabel.Content) + 1;
            }
            else if (!windowPlayer1 && player1Coming)
            {
                player1HitsLabel.Content = Convert.ToInt32(player1HitsLabel.Content) + 1;
            }
        }

        private void whichPlayerComingLabelChange()
        {
            if (player1Coming)
            {
                playerComingLabel.Content = player1Name + " is coming";
            }
            else
            {
                playerComingLabel.Content = player2Name + " is coming";
            }
        }

        private bool isCellShooted(int cell)
        {
            if (enemyPlayfield[cell / rows, cell % columns] == 'T' || enemyPlayfield[cell / rows, cell % columns] == 'V')
            {
                return true;
            }

            return false;
        }

        private void shipHpDecrement(string shipUnitName)
        {
            switch (shipUnitName)
            {
                case "5":
                    carrierHpGrid.Children.RemoveAt(carrierHpGrid.Children.Count - 1);
                    break;
                case "4":
                    battleshipHpGrid.Children.RemoveAt(battleshipHpGrid.Children.Count - 1);
                    break;
                case "3":
                    cruiserHpGrid.Children.RemoveAt(cruiserHpGrid.Children.Count - 1);
                    break;
                case "2":
                    submarineHpGrid.Children.RemoveAt(submarineHpGrid.Children.Count - 1);
                    break;
                case "1":
                    destroyerHpGrid.Children.RemoveAt(destroyerHpGrid.Children.Count - 1);
                    break;
            }
        }

        private void onGridMouseOver(object sender, MouseEventArgs e) //ship shadow
        {
            int cell = calculateCell();

            if (calculatedCell != cell)
            {
                calculatedCell = cell;

                deleteShadow();

                Rectangle shadow = shadowUnitSettings();

                Grid.SetRow(shadow, cell / rows);
                Grid.SetColumn(shadow, cell % columns);

                rightTable.Children.Add(shadow);
                shadowExists = true;
            }
        }

        private Rectangle shadowUnitSettings()
        {
            Rectangle shadow = new()
            {
                Fill = Brushes.LightGray
            };
            var Y = rightTable.Width / rows;
            var X = rightTable.Height / columns;
            shadow.Width = Y;
            shadow.Height = X;

            return shadow;
        }

        private void deleteShadow()
        {
            if (shadowExists)
            {
                int lastItem = rightTable.Children.Count - 1;
                rightTable.Children.RemoveAt(lastItem);
            }
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

        public void onClose()
        {
            this.Close();
        }

        private void gameEnd(string winner)
        {
            //score mentése
            List<Score> scores = ScoreResult.ReadResult("score.json");
            Score newScore = new()
            {
                Enemy = player2Name,
                EnemyHits = Convert.ToInt32(player2HitsLabel.Content),
                Player = player1Name,
                PlayerHits = Convert.ToInt32(player1HitsLabel.Content),
                Rounds = Convert.ToInt32(roundsLabel.Content),
                Winner = winner
            };

            scores.Add(newScore);
            ScoreResult.WriteResult(scores, "score.json");


            this.onCloseWindow();

            StartWindow startWindow = new StartWindow();
            this.Close();
            startWindow.Show();
        }

        private void surrendBtn_Click(object sender, RoutedEventArgs e)
        {
            if (windowPlayer1)
            {
                gameEnd(player2Name);
            }
            else
            {
                gameEnd(player1Name);
            }
        }

        private void stats_Click(object sender, RoutedEventArgs e)
        {
            Stats stats = new Stats();
            stats.Show();
        }
    }
}
