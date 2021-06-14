using System;
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

        int number = 0;

        PvP player2Window;
        Random rnd = new Random();

        public PvP(string player1Name, Grid player1PlayfieldGrid, char[,] player1BattleshipPlayfield, string player2Name, Grid player2PlayfieldGrid, char[,] player2BattleshipPlayfield)
        {
            InitializeComponent();
            this.Title = player1Name;

            player2Window = new PvP(player2Name, player2PlayfieldGrid, player2BattleshipPlayfield);
            player2Window.Title = player2Name;
            player2Window.Show();

            string playerStart = whichPlayerStart(player1Name, player2Name);
            initializeLabels(player1Name, player2Name, playerStart);

            shipStatHpInit();
            playerShipsLoad(player1PlayfieldGrid);
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

        public PvP(string player2Name, Grid player2PlayfieldGrid, char[,] player2BattleshipPlayfield)
        {
            InitializeComponent();

            shipStatHpInit();
            playerShipsLoad(player2PlayfieldGrid);
        }

        private string whichPlayerStart(string player1Name, string player2Name)
        {
            if (rnd.Next(0, 2) == 0)
            {
                return player1Name;
            }
            else
            {
                return player2Name;
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
            Rectangle hpUnit = new Rectangle();
            hpUnit.Fill = Brushes.Green;
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
                deleteShadow();
                shadowExists = false;


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
            var shadow = new Rectangle();
            shadow.Fill = Brushes.LightGray;
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
    }
}
