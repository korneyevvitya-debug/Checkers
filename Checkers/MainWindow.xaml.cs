using System;
using System.IO;
using System.Windows;

namespace Checkers
{
    public partial class MainWindow : Window
    {
        private string _savePath = "savegame.json";
        private const string SavePathConfig = "savepath.txt";
        private bool _useXml = false;
        private bool _initialized = false;

        public MainWindow()
        {
            InitializeComponent();

            if (File.Exists(SavePathConfig))
            {
                var savedPath = File.ReadAllText(SavePathConfig).Trim();
                if (!string.IsNullOrEmpty(savedPath))
                {
                    _savePath = savedPath;
                    SavePathText.Text = Path.GetDirectoryName(savedPath) ?? "(не выбрана)";

                    if (savedPath.EndsWith(".xml"))
                    {
                        _useXml = true;
                        FormatComboBox.SelectedIndex = 1;
                    }
                }
            }

            _initialized = true;
            UpdateContinueButton();
        }

        private void FormatChanged(object sender, EventArgs e)
        {
            if (!_initialized) return;

            _useXml = FormatComboBox.SelectedIndex == 1;

            if (!string.IsNullOrEmpty(_savePath))
            {
                string dir = Path.GetDirectoryName(_savePath) ?? "";
                string oldPath = _savePath;
                _savePath = Path.Combine(dir, _useXml ? "savegame.xml" : "savegame.json");

                if (File.Exists(oldPath) && oldPath != _savePath)
                {
                    try
                    {
                        Model.Data.GameSerializerBase oldSerializer = oldPath.EndsWith(".xml")
                            ? new Model.Data.GameSerializerXml()
                            : new Model.Data.GameSerializer();

                        Model.Data.GameSerializerBase newSerializer = _useXml
                            ? new Model.Data.GameSerializerXml()
                            : new Model.Data.GameSerializer();

                        var game = oldSerializer.Load(oldPath);
                        if (game != null)
                            newSerializer.Save(game, _savePath);
                    }
                    catch { }
                }

                File.WriteAllText(SavePathConfig, _savePath);
                UpdateContinueButton();
            }
        }

        private void SelectSaveFolder(object sender, EventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = _useXml ? "savegame.xml" : "savegame.json";
                _savePath = Path.Combine(dialog.SelectedPath, fileName);
                SavePathText.Text = dialog.SelectedPath;
                File.WriteAllText(SavePathConfig, _savePath);
                UpdateContinueButton();
            }
        }

        private void UpdateContinueButton()
        {
            Model.Data.GameSerializerBase serializer = _useXml
                ? new Model.Data.GameSerializerXml()
                : new Model.Data.GameSerializer();
            bool valid = serializer.IsValidSave(_savePath);
            ContinueButton.IsEnabled = valid;
            if (File.Exists(_savePath) && !valid)
                SavePathText.Text += " — файл некорректного формата!";
        }

        private void StartTwoPlayer(object sender, EventArgs e)
        {
            var gameWindow = new GameWindow(false, _savePath, _useXml);
            gameWindow.Width = this.Width;
            gameWindow.Height = this.Height;
            gameWindow.WindowState = this.WindowState;
            gameWindow.Closed += GameWindowClosed!;
            gameWindow.Show();
            this.Hide();
        }

        private void StartVsComputer(object sender, EventArgs e)
        {
            var gameWindow = new GameWindow(true, _savePath, _useXml);
            gameWindow.Width = this.Width;
            gameWindow.Height = this.Height;
            gameWindow.WindowState = this.WindowState;
            gameWindow.Closed += GameWindowClosed!;
            gameWindow.Show();
            this.Hide();
        }

        private void ContinueGame(object sender, EventArgs e)
        {
            var gameWindow = new GameWindow(_savePath, true, _useXml);
            gameWindow.Width = this.Width;
            gameWindow.Height = this.Height;
            gameWindow.WindowState = this.WindowState;
            gameWindow.Closed += GameWindowClosed!;
            gameWindow.Show();
            this.Hide();
        }

        private void GameWindowClosed(object sender, EventArgs e)
        {
            if (sender is GameWindow gw)
            {
                this.Width = gw.Width;
                this.Height = gw.Height;
                this.WindowState = gw.WindowState;
            }
            UpdateContinueButton();
            this.Show();
        }
    }
}