using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Checkers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public void StartGame(object sender, EventArgs e)
        {
            GameWindow gamewindow = new GameWindow();
            gamewindow.Closed += GameWindowClosed!;
            gamewindow.Show();
            this.Hide();
        }
        private void GameWindowClosed(object sender, EventArgs e)
        {
            this.Show();
        }

    }
}