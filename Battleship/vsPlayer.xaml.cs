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
    /// Interaction logic for vsPlayer.xaml
    /// </summary>
    public partial class vsPlayer : Window
    {
        public vsPlayer()
        {
            InitializeComponent();

            shipStatHpInit();
        }

        private void shipStatHpInit()
        {
            for (int ship = 5; ship > 0; ship--)
            {
                for (int unit = 0; unit < ship; unit++)
                {
                    Rectangle player1HpUnit = shipHpSettings(ship);
                    Rectangle player2HpUnit = shipHpSettings(ship);

                    Grid.SetColumn(player1HpUnit, unit);
                    Grid.SetColumn(player2HpUnit, unit);

                    switch (ship)
                    {
                        case 5:
                            player1CarrierHpGrid.Children.Add(player1HpUnit);
                            player2CarrierHpGrid.Children.Add(player2HpUnit);
                            break;
                        case 4:
                            player1BattleshipHpGrid.Children.Add(player1HpUnit);
                            player2BattleshipHpGrid.Children.Add(player2HpUnit);
                            break;
                        case 3:
                            player1CruiserHpGrid.Children.Add(player1HpUnit);
                            player2CruiserHpGrid.Children.Add(player2HpUnit);
                            break;
                        case 2:
                            player1SubmarineHpGrid.Children.Add(player1HpUnit);
                            player2SubmarineHpGrid.Children.Add(player2HpUnit);
                            break;
                        case 1:
                            player1DestroyerHpGrid.Children.Add(player1HpUnit);
                            player2DestroyerHpGrid.Children.Add(player2HpUnit);
                            break;
                    }
                }
            }
        }

        private Rectangle shipHpSettings(int shipLength)
        {
            Rectangle hpUnit = new Rectangle();
            hpUnit.Fill = Brushes.Green;
            var Y = player1CarrierHpGrid.Width;
            var X = player1CarrierHpGrid.Height / shipLength;
            hpUnit.Width = Y;
            hpUnit.Height = X;

            return hpUnit;
        }
    }
}
