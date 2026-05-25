using System;
using System.IO;
using System.Windows;

namespace Checkers
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // кнопка продолжить неактивна если нет сохранения
            ContinueButton.IsEnabled = File.Exists("savegame.json");
        }

        private void StartTwoPlayer(object sender, EventArgs e)
        {
            var gameWindow = new GameWindow(false);
            gameWindow.Closed += GameWindowClosed!;
            gameWindow.Show();
            this.Hide();
        }

        private void StartVsComputer(object sender, EventArgs e)
        {
            var gameWindow = new GameWindow(true);
            gameWindow.Closed += GameWindowClosed!;
            gameWindow.Show();
            this.Hide();
        }

        private void ContinueGame(object sender, EventArgs e)
        {
            var gameWindow = new GameWindow("savegame.json");
            gameWindow.Closed += GameWindowClosed!;
            gameWindow.Show();
            this.Hide();
        }

        private void GameWindowClosed(object sender, EventArgs e)
        {
            // обновляем кнопку — вдруг появилось сохранение
            ContinueButton.IsEnabled = File.Exists("savegame.json");
            this.Show();
        }
    }
}