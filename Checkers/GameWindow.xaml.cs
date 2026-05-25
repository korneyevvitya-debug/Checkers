using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Checkers
{
    public partial class GameWindow : Window
    {
        private bool _vsComputer;

        public GameWindow(bool vsComputer)
        {
            InitializeComponent();
            _vsComputer = vsComputer;
            this.Closed += SaveGame!;

            if (vsComputer)
                Game = new Model.Core.GameVsComputer();
            else
                Game = new Model.Core.Game();

            Game.OnKingPromotion = (pos) =>
            {
                string color = Game.Pieces.Find(p => p.Position == pos)?.IsBlack == true ? "Чёрная" : "Белая";
                Notification.Text = $"{color} шашка стала дамкой!";

                // убираем уведомление через 3 секунды
                var timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(3);
                timer.Tick += (s, e) => { Notification.Text = ""; timer.Stop(); };
                timer.Start();
            };

            InitGameGrid();
            UpdateBoard();
        }

        public GameWindow(string savePath)
        {
            InitializeComponent();
            this.Closed += SaveGame!;
            // загрузка из файла — добавим позже
            Game = new Model.Core.Game();
            InitGameGrid();
            UpdateBoard();
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
                        Background = (row + column) % 2 == 0 ? Brushes.White : Brushes.DimGray,
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
            Capture.Text = $"Must capture: {Game.MustCapture}";
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
                            Fill = Game.Gameboard[row, column] < 0 ? Brushes.OrangeRed : Brushes.Black,
                            IsHitTestVisible = false
                        };

                        // дамка — обводка
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
        }

        public Model.Core.Game Game { get; private set; }

        public void TrySelect((int, int) pos)
        {
            if (_vsComputer && ((Model.Core.GameVsComputer)Game).IsComputerTurn) return;

            Game.AttemptAction(pos);
            ShowAvailibleMoves();
            UpdateBoard();
            CheckWin();

            if (_vsComputer && ((Model.Core.GameVsComputer)Game).IsComputerTurn)
            {
                ((Model.Core.GameVsComputer)Game).MakeComputerMove();
                ShowAvailibleMoves();
                UpdateBoard();
                CheckWin();
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

        private void CheckWin()
        {
            bool blackExists = Game.Pieces.Exists(p => p.IsBlack);
            bool whiteExists = Game.Pieces.Exists(p => !p.IsBlack);

            if (!blackExists)
            {
                MessageBox.Show("Белые победили!");
                this.Close();
            }
            else if (!whiteExists)
            {
                MessageBox.Show("Чёрные победили!");
                this.Close();
            }
        }

        private void SaveGame(object sender, EventArgs e)
        {
            // сохранение добавим позже
        }
    }
}