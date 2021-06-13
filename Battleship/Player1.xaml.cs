using System.Windows;

namespace Battleship
{
    /// <summary>
    /// Interaction logic for Player1.xaml
    /// </summary>
    public partial class Player1 : Window
    {
        Player2 player2Window = new Player2();

        public Player1()
        {
            InitializeComponent();

            player2Window.OnMessage += new Player2.Message(this.sendingInformation);
        }

        private void newgameBtn_Click(object sender, RoutedEventArgs e)
        {
            player2Window.sendingInformation("From P1");
            player2Window.Show();
        }

        public void sendingInformation(string content)
        {
            newgameBtn.Content = content;
        }
    }
}
