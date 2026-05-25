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

namespace Checkers
{
    /// <summary>
    /// Логика взаимодействия для GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public GameWindow()
        {
            InitializeComponent();
            this.Closed += SaveGame!;
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
                        Background = (row+column)%2==0? Brushes.White : Brushes.DimGray,
                        
                        
                    };
                    square.MouseLeftButtonDown += (o, e) => TrySelect(((int,int))square.Tag);
                    Panel.SetZIndex(square, 0);
                    Grid.SetRow(square, row);
                    Grid.SetColumn(square, column);
                    gameGrid.Children.Add(square);
                }
            }
        }
        private void UpdateBoard()
        {
            Capture.Text = $"Must capture:{Game.MustCapture.ToString()}";
            Checkers.Children.Clear();
            for(int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    if (Game.Gameboard[row, column] != 0)
                    {
                        Ellipse circle = new Ellipse()
                        {
                            Width = 30,
                            Height = 30,
                            Fill = Game.Gameboard[row, column] < 0 ? Brushes.OrangeRed:Brushes.Black,
                            IsHitTestVisible = false
                        };
                        Panel.SetZIndex(circle, 2);
                        Grid.SetRow(circle, row);
                        Grid.SetColumn(circle, column);
                        Checkers.Children.Add(circle);


                    }
                }
            }
        }
        public Model.Core.Game Game {  get; private set; }
        public void TrySelect((int,int) pos)
        {
            Game.AttemptAction(pos);
            //if (Game.Selected != null)
            //MessageBox.Show("Success");
            //if (Game.Selected != null)
                //MessageBox.Show(Game.Selected.AvailibleCaptures(Game.Gameboard).Count().ToString());
            ShowAvailibleMoves();
            UpdateBoard();
            //MessageBox.Show(pos.ToString());
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
        private void SaveGame(object sender, EventArgs e)
        {
            
        }
        public void TestMethod(object sender, EventArgs e)
        {
            MessageBox.Show("Success");
        }
    }
}
