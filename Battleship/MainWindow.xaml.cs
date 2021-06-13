using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Battleship
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly char[] _characters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j' };
        private static readonly short[] _nums = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private static readonly int rows = _characters.Length;
        private static readonly int columns = _nums.Length;

        Random rnd = new Random();

        char[,] playerPlayfield = new char[10, 10];
        char[,] playerPlayfield2 = new char[10, 10];
        char[,] aiPlayfield = new char[10, 10];

        bool aiShipsShow = false;

        int left = 0, right = 0, down = 0, up = 0;
        int ures = 0;

        int hitX = -1, hitY = -1;

        public MainWindow(Grid playfield, char[,] playerPlayfield)
        {
            InitializeComponent();

            this.playerPlayfield = playerPlayfield;
            playerShipsLoad(playfield);

            shipAI(rnd);

            //game(rnd);

            shipStatHpInit();
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

        public MainWindow(Grid playfield, char[,] playerPlayfield, char[,] playerPlayfield2)
        {
            InitializeComponent();

            this.playerPlayfield = playerPlayfield; 
            this.playerPlayfield2 = playerPlayfield2;
            playerShipsLoad(playfield);


            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    Rectangle ship = new Rectangle();
                    ship.Fill = Brushes.Red;
                    var Y = rightTable.Width / rows;
                    var X = rightTable.Height / columns;
                    ship.Width = Y;
                    ship.Height = X;

                    Grid.SetRow(ship, row);
                    Grid.SetColumn(ship, col);

                    if (playerPlayfield[row, col] == 'H')
                    {
                        rightTable.Children.Add(ship);
                    }
                }

            }

        }

        private void new_gameBtn_Click(object sender, RoutedEventArgs e)
        {
            Opponent opponentWindow = new Opponent();
            App.Current.MainWindow = opponentWindow;
            this.Close();
            opponentWindow.Show();
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
                        if ((randomPosX != 0 && aiPlayfield[randomPosY, randomPosX - 1] == 'H') || ((randomPosX + i - 1) != 9 && aiPlayfield[randomPosY, randomPosX + i] == 'H'))
                        {
                            randomPosX = (int)rnd.Next(0, 10 - i + 1);
                            randomPosY = (int)rnd.Next(0, 10);
                        }
                        else
                        {
                            for (int k = 0; k < i; k++)
                            {
                                if (aiPlayfield[randomPosY, randomPosX + k] == 'H' || (randomPosY != 0 && aiPlayfield[randomPosY - 1, randomPosX + k] == 'H') || (randomPosY != 9 && aiPlayfield[randomPosY + 1, randomPosX + k] == 'H'))
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

                        aiPlayfield[randomPosY, randomPosX + col] = 'H';
                        rightTable.Children.Add(ship);
                    }
                }
                else if (randomOrient == 1) //függőleges
                {
                    randomPosY = (int)rnd.Next(0, 10 - i + 1);
                    randomPosX = (int)rnd.Next(0, 10);

                    while (empty == false)
                    {
                        if ((randomPosY != 0 && aiPlayfield[randomPosY - 1, randomPosX] == 'H') || ((randomPosY + i - 1) != 9 && aiPlayfield[randomPosY + i, randomPosX] == 'H'))
                        {
                            randomPosY = (int)rnd.Next(0, 10 - i + 1);
                            randomPosX = (int)rnd.Next(0, 10);
                        }
                        else
                        {
                            for (int k = 0; k < i; k++)
                            {
                                if (aiPlayfield[randomPosY + k, randomPosX] == 'H' || (randomPosX != 0 && aiPlayfield[randomPosY + k, randomPosX - 1] == 'H') || (randomPosX != 9 && aiPlayfield[randomPosY + k, randomPosX + 1] == 'H'))
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

                        Grid.SetRow(ship, row + randomPosY);
                        Grid.SetColumn(ship, randomPosX);

                        aiPlayfield[randomPosY + row, randomPosX] = 'H';
                        rightTable.Children.Add(ship);
                    }
                }
            }
        }

        private Rectangle shipSettings(int shipLength)
        {
            Rectangle ship = new Rectangle();
            ship.Fill = Brushes.DodgerBlue;
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

        private void surrendClick(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            game(rnd);
        }

        

        private void game(Random rnd)
        {
            bool player = false; // false - AI | true - Player 
            bool hit;  

            int randomX;
            int randomY;

            while (!player)
            {
                /*
                if (ures == 100)
                {
                    surrrendBtn.IsEnabled = false;
                    break;
                }
                */

                randomY = (int)rnd.Next(0, 10);
                randomX = (int)rnd.Next(0, 10);

                if(playerPlayfield[randomY, randomX] == 'T' || playerPlayfield[randomY, randomX] == 'V')
                {
                    continue;
                }

                if (playerPlayfield[randomY, randomX] == 'H')
                {
                    //playerPlayfield[randomY, randomX] = 'T';

                    hitX = randomX;
                    hitY = randomY;

                    hit = true;

                    while (hit)
                    {
                        emptyCells(hitY, hitX);

                        if (playerPlayfield[hitY, hitX] == 'H')
                        {
                            //hitX = randomX;
                            playerPlayfield[hitY, hitX] = 'T';
                            paintHitCell(hitX, hitY);
                            
                            if(hitY != 0)
                            {
                                hitY--;
                            }
                            else
                            {
                                hit = false;
                            }
                        }
                        else if (!(playerPlayfield[hitY, hitX] == 'T' || playerPlayfield[hitY, hitX] == 'V'))
                        {
                            playerPlayfield[hitY, hitX] = 'V';
                            paintMissCell(hitX, hitY);
                            hit = false;
                            //player = true;
                        }
                        else if(playerPlayfield[hitY, hitX] == 'T' || playerPlayfield[hitY, hitX] == 'V')
                        {
                            randomY = (int)rnd.Next(0, 10);
                            randomX = (int)rnd.Next(0, 10);
                            hitX = randomX;
                            hitY = randomY;
                        }
                    }

                    //Debug.WriteLine($"up: {up}\tright: {right}\tdown: {down}\tleft: {left}");

                    player = true;
                }
                else if(!(playerPlayfield[randomY, randomX] == 'T' || playerPlayfield[randomY, randomX] == 'V'))
                {
                    playerPlayfield[randomY, randomX] = 'V';

                    paintMissCell(randomX, randomY);
           
                    player = true;
                }   
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
            ship.Fill = Brushes.Black;

            Grid.SetRow(ship, randomY);
            Grid.SetColumn(ship, randomX);

            leftTable.Children.Add(ship);
        }

        private void emptyCells(int randomY, int randomX)
        {
            if (randomY != 0 && (playerPlayfield[randomY - 1, randomX] != 'T' || playerPlayfield[randomY - 1, randomX] != 'V'))
            {
                up = randomY - 1;
            }
            else
            {
                up = -1;
            }

            if (randomY != 9 && (playerPlayfield[randomY + 1, randomX] != 'T' || playerPlayfield[randomY + 1, randomX] != 'V'))
            {
                down = randomY + 1;
            }
            else
            {
                down = -1;
            }

            if (randomX != 0 && (playerPlayfield[randomY, randomX - 1] != 'T' || playerPlayfield[randomY, randomX - 1] != 'V'))
            {
                left = randomX - 1;
            }
            else
            {
                left = -1;
            }

            if (randomX != 9 && (playerPlayfield[randomY, randomX + 1] != 'T' || playerPlayfield[randomY, randomX + 1] != 'V'))
            {
                right = randomX + 1;
            }
            else
            {
                right = -1;
            }

        }
    }
}