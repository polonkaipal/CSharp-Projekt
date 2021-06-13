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
            
        }
    }
}
