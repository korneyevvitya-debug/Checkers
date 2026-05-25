using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Panel = System.Windows.Controls.Panel;
using Button = System.Windows.Controls.Button;
using Color = System.Windows.Media.Color;
using System.IO;

namespace Checkers
{
    public partial class GameWindow : Window
    {
        private bool _vsComputer;
        private string _savePath;
        private bool _useXml;
        private bool _gameEnded = false;

        public GameWindow(bool vsComputer, string savePath, bool useXml)
        {
            InitializeComponent();
            _vsComputer = vsComputer;
            _savePath = savePath;
            _useXml = useXml;
            this.Closed += SaveGame!;

            if (vsComputer)
                Game = new Model.Core.GameVsComputer(true);
            else
                Game = new Model.Core.Game();

            SetupGameEvents();
            InitGameGrid();
            UpdateBoard();
        }

        public GameWindow(string savePath, bool isLoad, bool useXml)
        {
            InitializeComponent();
            _savePath = savePath;
            _useXml = useXml;
            this.Closed += SaveGame!;

            Model.Data.GameSerializerBase serializer = useXml
                ? new Model.Data.GameSerializerXml()
                : new Model.Data.GameSerializer();

            var loaded = serializer.Load(savePath);
            Game = loaded ?? new Model.Core.Game();
            _vsComputer = Game is Model.Core.GameVsComputer;

            SetupGameEvents();
            InitGameGrid();
            UpdateBoard();
        }

        private void SetupGameEvents()
        {
            Game.OnKingPromotion = (pos) =>
            {
                string color = Game.Pieces.Find(p => p.Position == pos)?.IsBlack == true ? "Чёрная" : "Белая";
                ShowNotification($"{color} шашка стала дамкой!");
            };

            Game.OnGameOver = (blackWon) =>
            {
                _gameEnded = true;
                if (blackWon == null)
                {
                    Notification.Text = "Ничья!";
                    Notification.Foreground = Brushes.Gold;
                }
                else if (_vsComputer)
                {
                    if (blackWon.Value)
                    {
                        Notification.Text = "Компьютер победил!";
                        Notification.Foreground = Brushes.Red;
                    }
                    else
                    {
                        Notification.Text = "Вы победили!";
                        Notification.Foreground = Brushes.LimeGreen;
                    }
                }
                else
                {
                    Notification.Text = blackWon.Value ? "Чёрные победили!" : "Белые победили!";
                    Notification.Foreground = Brushes.Gold;
                }

                gameGrid.IsHitTestVisible = false;
                DrawButton1.IsEnabled = false;
                DrawButton2.IsEnabled = false;
                SurrenderButton1.IsEnabled = false;
                if (!_vsComputer)
                {
                    DeclineButton1.IsEnabled = false;
                    DeclineButton2.IsEnabled = false;
                    SurrenderButton2.IsEnabled = false;
                }
            };

           

            if (_vsComputer)
            {
                BlackPanel.Visibility = Visibility.Collapsed;
                DeclineButton1.Visibility = Visibility.Collapsed;
                DeclineButton2.Visibility = Visibility.Collapsed;
            }
            else
            {
                DeclineButton1.Visibility = Visibility.Collapsed;
                DeclineButton2.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowNotification(string text)
        {
            Notification.Text = text;
            Notification.Foreground = Brushes.Gold;
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, e) => { Notification.Text = ""; timer.Stop(); };
            timer.Start();
        }

        private void InitGameGrid()
        {
            gameGrid.Children.Clear();
            gameGrid.RowDefinitions.Clear();
            gameGrid.ColumnDefinitions.Clear();

            for (int row = 0; row < 8; row++)
            {
                gameGrid.RowDefinitions.Add(new RowDefinition());
                Checkers.RowDefinitions.Add(new RowDefinition());
                Moves.RowDefinitions.Add(new RowDefinition());
            }
            for (int column = 0; column < 8; column++)
            {
                gameGrid.ColumnDefinitions.Add(new ColumnDefinition());
                Checkers.ColumnDefinitions.Add(new ColumnDefinition());
                Moves.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    Border square = new Border()
                    {
                        Tag = (row, column),
                        Background = (row + column) % 2 == 0
                            ? new SolidColorBrush(Color.FromRgb(240, 217, 181))
                            : new SolidColorBrush(Color.FromRgb(181, 136, 99)),
                    };
                    square.MouseLeftButtonDown += (o, e) => TrySelect(((int, int))square.Tag);
                    Panel.SetZIndex(square, 0);
                    Grid.SetRow(square, row);
                    Grid.SetColumn(square, column);
                    gameGrid.Children.Add(square);
                }
            }
        }

        private void UpdateBoard()
        {
            Capture.Text = "";
            Checkers.Children.Clear();

            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    if (Game.Gameboard[row, column] != 0)
                    {
                        bool isKing = Game.Pieces.Exists(p =>
                            p.Position == (row, column) && p is Model.Core.King);

                        Ellipse circle = new Ellipse()
                        {
                            Width = 30,
                            Height = 30,
                            Fill = Game.Gameboard[row, column] < 0
                                ? Brushes.White
                                : new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                            Stroke = Game.Gameboard[row, column] < 0
                                ? new SolidColorBrush(Color.FromRgb(100, 100, 100))
                                : new SolidColorBrush(Color.FromRgb(80, 80, 80)),
                            StrokeThickness = 2,
                            IsHitTestVisible = false
                        };

                        if (isKing)
                        {
                            circle.Stroke = Brushes.Gold;
                            circle.StrokeThickness = 3;
                        }

                        Panel.SetZIndex(circle, 2);
                        Grid.SetRow(circle, row);
                        Grid.SetColumn(circle, column);
                        Checkers.Children.Add(circle);
                    }
                }
            }

            UpdateCaptureCounters();
            if (!_gameEnded)
            {
                Notification.Text = Game.IsBlackTurn ? "Ход чёрных" : "Ход белых";
                Notification.Foreground = Brushes.Gold;
            }
        }

        private void UpdateCaptureCounters()
        {
            int blackLeft = Game.Pieces.Count(p => p.IsBlack);
            int whiteLeft = Game.Pieces.Count(p => !p.IsBlack);

            WhiteCaptured.Text = $"Съедено: {12 - blackLeft}";
            BlackCaptured.Text = $"Съедено: {12 - whiteLeft}";
        }

        public Model.Core.Game Game { get; private set; }

        public void TrySelect((int, int) pos)
        {
            if (_vsComputer && ((Model.Core.GameVsComputer)Game).IsComputerTurn) return;

            Game.AttemptAction(pos);
            ShowAvailibleMoves();
            UpdateBoard();

            if (_vsComputer && ((Model.Core.GameVsComputer)Game).IsComputerTurn)
            {
                ((Model.Core.GameVsComputer)Game).MakeComputerMove();
                ShowAvailibleMoves();
                UpdateBoard();
            }
        }

        public void ShowAvailibleMoves()
        {
            Moves.Children.Clear();
            foreach ((int x, int y) in Game.MoveSet)
            {
                Ellipse circle = new Ellipse()
                {
                    Width = 35,
                    Height = 35,
                    Fill = Brushes.Green,
                    IsHitTestVisible = false
                };
                Panel.SetZIndex(circle, 1);
                Grid.SetRow(circle, x);
                Grid.SetColumn(circle, y);
                Moves.Children.Add(circle);
            }
        }

        private void RequestDraw(object sender, EventArgs e)
        {
            if (_vsComputer)
            {
                int computerScore = 0;
                int playerScore = 0;
                foreach (var piece in Game.Pieces)
                {
                    int points = piece is Model.Core.King ? 3 : 1;
                    if (piece.IsBlack) computerScore += points;
                    else playerScore += points;
                }

                if (computerScore < playerScore)
                {
                    Notification.Text = "Компьютер согласился на ничью!";
                    Notification.Foreground = Brushes.Gold;
                    gameGrid.IsHitTestVisible = false;
                    DrawButton1.IsEnabled = false;
                    SurrenderButton1.IsEnabled = false;
                }
                else
                {
                    ShowNotification("Компьютер отказался от ничьей!");
                }
            }
            else
            {
                bool isBlack = (sender as Button)?.Name == "DrawButton2";
                bool isDraw = Game.RequestDraw(isBlack);

                if (!isDraw)
                {
                    string who = isBlack ? "Чёрные" : "Белые";
                    ShowNotification($"{who} предлагают ничью!");
                    if (isBlack)
                        DeclineButton1.Visibility = Visibility.Visible;
                    else
                        DeclineButton2.Visibility = Visibility.Visible;
                }
            }
        }

        private void DeclineDraw(object sender, EventArgs e)
        {
            bool isBlack = (sender as Button)?.Name == "DeclineButton2";
            Game.DeclineDraw(isBlack);
            ShowNotification(isBlack ? "Чёрные отказали в ничьей!" : "Белые отказали в ничьей!");
            if (isBlack)
                DeclineButton2.Visibility = Visibility.Collapsed;
            else
                DeclineButton1.Visibility = Visibility.Collapsed;
        }

        private void Surrender(object sender, EventArgs e)
        {

            bool isBlack = (sender as Button)?.Name == "SurrenderButton2";
            Notification.Text = isBlack
                ? "Чёрные сдались! Белые победили!"
                : "Белые сдались! Чёрные победили!";
            Notification.Foreground = Brushes.OrangeRed;
            gameGrid.IsHitTestVisible = false;
            DrawButton1.IsEnabled = false;
            DrawButton2.IsEnabled = false;
            SurrenderButton1.IsEnabled = false;
            if (!_vsComputer)
            {
                SurrenderButton2.IsEnabled = false;
                DeclineButton1.IsEnabled = false;
                DeclineButton2.IsEnabled = false;
            }
            _gameEnded = true;
            if (File.Exists(_savePath))
                File.Delete(_savePath);

            // удаляем сохранение чтобы нельзя было продолжить
            if (File.Exists(_savePath))
                File.Delete(_savePath);
        }

        private void GoToMenu(object sender, EventArgs e)
        {
            // если игра не завершена — сохраняем
            if (!_gameEnded)
            {
                Model.Data.GameSerializerBase serializer = _useXml
                    ? new Model.Data.GameSerializerXml()
                    : new Model.Data.GameSerializer();
                serializer.Save(Game, _savePath);
                _gameEnded = true; // чтобы SaveGame не сохранял повторно
            }
            this.Close();
        }

        private void SaveGame(object sender, EventArgs e)
        {
            if (_gameEnded) return; // не сохраняем если игра завершена

            Model.Data.GameSerializerBase serializer = _useXml
                ? new Model.Data.GameSerializerXml()
                : new Model.Data.GameSerializer();
            serializer.Save(Game, _savePath);
        }
    }
}