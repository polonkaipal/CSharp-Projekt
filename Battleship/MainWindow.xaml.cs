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

        public MainWindow()
        {
            InitializeComponent();

            createTable();
        }

        private void createTable()
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
                    border.Name = _characters[row].ToString() + _nums[column].ToString();
                    table.Children.Add(border);
                }
            }
        }
    }
}
