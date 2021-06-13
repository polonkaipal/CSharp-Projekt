using System.Windows;

namespace Battleship
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();

            playerInitialize();
        }

        private void playerInitialize()
        {
            vsComputerRadioBtn.IsChecked = true;
            player2GridRow.Height = new GridLength(0);
        }

        private void vsComputerRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            vsPlayerRadioBtn.IsChecked = false;
            player2GridRow.Height = new GridLength(0);
        }

        private void vsPlayerRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            vsComputerRadioBtn.IsChecked = false;
            player2GridRow.Height = new GridLength(1, GridUnitType.Star);
        }

        private void playBtn_Click(object sender, RoutedEventArgs e)
        {
            if (vsComputerRadioBtn.IsChecked == true)
            {
                if (!string.IsNullOrWhiteSpace(player1TextBox.Text))
                {
                    ShipPlacement player1ShipPlacementWindow = new ShipPlacement(player1TextBox.Text);
                    this.Close();
                    player1ShipPlacementWindow.Show();
                }
            }
            else if (vsPlayerRadioBtn.IsChecked == true)
            {
                if (!string.IsNullOrWhiteSpace(player1TextBox.Text) && !string.IsNullOrWhiteSpace(player2TextBox.Text))
                {
                    ShipPlacement player1ShipPlacementWindow = new ShipPlacement(player1TextBox.Text, player2TextBox.Text);
                    App.Current.MainWindow = player1ShipPlacementWindow;
                    this.Close();
                    player1ShipPlacementWindow.Show();
                }
            }
        }
    }
}
