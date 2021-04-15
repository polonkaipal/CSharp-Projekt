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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Battleship
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly char[] _characters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
        private static readonly short[] _nums = { 1, 2, 3, 4, 5, 6, 7, 8 };
        private static readonly int rows = _characters.Length;
        private static readonly int columns = _nums.Length;
        Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();

            createTable(leftTable, 'l');
            createTable(rightTable, 'r');

            shipAI(rightTable, rnd);
        }

        private void createTable(Grid table, char tId)
        {
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    Border border = new()
                    {
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(2)
                    };
                    Grid.SetRow(border, row);
                    Grid.SetColumn(border, column);
                    border.Name = tId.ToString() + _characters[row].ToString() + _nums[column].ToString();
                    table.Children.Add(border);
                }
            }
        }

        private void shipAI(Grid table, Random rnd)
        {
            //ship 3

            int randomOrient = (int)rnd.Next(0, 2);
            int randomPosX = (int)rnd.Next(0, 9);
            int randomPosY = (int)rnd.Next(0, 9);

            if (randomOrient == 0) // vízsintes
            {
                for (int col = 0; col < 3; col++)
                {
                    var ship = new Rectangle();
                    ship.Fill = Brushes.Red;
                    var Y = table.Width / rows;
                    var X = table.Height / columns;
                    ship.Width = Y;
                    ship.Height = X;

                    if (randomPosY + 3 > 8)
                    {
                        randomPosY = (int)rnd.Next(0, 9);
                    }

                    Grid.SetRow(ship, randomPosX);
                    Grid.SetColumn(ship, col + randomPosY);
                    table.Children.Add(ship);
                }
            }
            else if (randomOrient == 1) //függőleges
            {
                for (int row = 0; row < 3; row++)
                {
                    var ship = new Rectangle();
                    ship.Fill = Brushes.Red;
                    var Y = table.Width / rows;
                    var X = table.Height / columns;
                    ship.Width = Y;
                    ship.Height = X;

                    if (randomPosX + 3 > 8)
                    {
                        randomPosX = (int)rnd.Next(0, 9);
                    }

                    Grid.SetRow(ship, row + randomPosX);
                    Grid.SetColumn(ship, randomPosY);
                    table.Children.Add(ship);
                }
            }
        }
    }
}
