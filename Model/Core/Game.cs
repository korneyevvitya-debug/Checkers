using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Core
{
    public class Game
    {
        public int[,] Gameboard { get; private set; }
        public List<Piece> Pieces { get; private set; }
        public Piece Selected { get; private set; }
        public bool IsBlackTurn {  get; private set; }
        public List<(int,int)> MoveSet { get; private set; }
        public bool MustCapture { get; private set; }
        public Game()
        {
            Pieces = new List<Piece>();
            Gameboard = new int[8, 8];
            Selected = null!;
            IsBlackTurn = false;
            MustCapture = false;
            MoveSet = new List<(int, int)>();
            Pieces.Add(new Checker(false, (5, 4)));
            Pieces.Add(new Checker(true, (3, 4)));
            Pieces.Add(new Checker(true, (1, 4)));
            UpdateGameBoard();
        }
        private void UpdateGameBoard()
        {
            Gameboard = new int[8, 8];
            foreach (IMovable movable in Pieces)
            {
                Gameboard[movable.Position.Item1, movable.Position.Item2] = movable.IsBlack ? 1 : -1;
            }
        }
        
        public void Select((int, int) pos)
        {
            foreach(Piece piece in Pieces)
            {
                if (piece.IsBlack == IsBlackTurn && piece.Position == pos)
                {
                    Selected = piece;
                    break;
                }
            }
        }
        public void Select((int,int) pos, bool IgnoreColor)
        {
            foreach (Piece piece in Pieces)
            {
                if (piece.Position == pos)
                {
                    Selected = piece;
                    break;
                }
            }
        }
        private void PassMove()
        {
            UpdateGameBoard();
            Deselect();
            MoveSet.Clear();
            IsBlackTurn = !IsBlackTurn;
            MustCapture = false;
            foreach(IMovable piece in Pieces)
            {
                if (piece.IsBlack == IsBlackTurn && piece.AvailibleCaptures(Gameboard).Count() > 1)
                    MustCapture = true;
            }
        }
        private void Deselect()
        {
            Selected = null!;
            MoveSet.Clear();
        }
        private void Capture((int,int) beginning, (int,int) end)
        {
            Selected.MoveTo(end);
            Deselect();
            int i = (Math.Sign(end.Item1 - beginning.Item1));
            int j = (Math.Sign(end.Item2 - beginning.Item2));
            (int,int) deletion = (beginning.Item1 + i, beginning.Item2 + j);
            while (deletion != end)
            {
                Select(deletion, true);
                Pieces.Remove(Selected);
                Deselect();
                deletion.Item1 += i;
                deletion.Item2 += j;
            }
            Select(end);
            UpdateGameBoard();
        }
        public void AttemptAction((int, int) pos)
        {
            if (Selected == null)
            {
                Select(pos);
                if (MustCapture && Selected != null)
                {
                    MoveSet = Selected.AvailibleCaptures(Gameboard);
                }
                else if (Selected != null)
                {
                    MoveSet = Selected.AvailibleMoves(Gameboard);
                }
            }
            else if (MoveSet.Contains(pos))
            {
                if (pos == Selected.Position)
                {
                    Deselect();
                    
                }
                else if (MustCapture)
                {
                    Capture(Selected.Position, pos);
                    MoveSet = Selected.AvailibleCaptures(Gameboard);
                    MoveSet.Remove(pos);
                    if (MoveSet.Count == 0)
                    {
                        PassMove();
                    }
                }
                else
                {
                    Selected.MoveTo(pos);
                    PassMove();
                }

            }
            
        }
    }
}
