using System.Windows;

namespace Battleship
{
    /// <summary>
    /// Interaction logic for Player2.xaml
    /// </summary>
    public partial class Player2 : Window
    {
        public delegate void Message(string content);

        public event Message OnMessage;

        public Player2()
        {
            InitializeComponent();
        }
 
        public void sendingInformation(string content)
        {
            newgameBtn.Content = content;
        }

        public void newgameBtn_Click(object sender, RoutedEventArgs e)
        {
            this.OnMessage("From P2");
        }
    }
}
