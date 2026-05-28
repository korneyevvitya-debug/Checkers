using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Core
{
    public partial class Game
    {
        public int[,] Gameboard { get; private set; }
        public List<Piece> Pieces { get; set; }
        public Piece? Selected { get; private set; }
        public bool IsBlackTurn { get; set; }
        public List<(int, int)> MoveSet { get; private set; }
        public bool MustCapture { get; set; }
        public Action<(int, int)>? OnKingPromotion { get; set; }
        public Action<bool?>? OnGameOver { get; set; }
        

        public Game() : this(true) { }

        public Game(bool withPieces)
        {
            Pieces = new List<Piece>();
            Gameboard = new int[8, 8];
            Selected = null!;
            IsBlackTurn = false;
            MustCapture = false;
            MoveSet = new List<(int, int)>();

            if (withPieces)
            {
                for (int row = 5; row <= 7; row++)
                    for (int col = 0; col < 8; col++)
                        if ((row + col) % 2 != 0)
                            Pieces.Add(new Checker(false, (row, col)));

                for (int row = 0; row <= 2; row++)
                    for (int col = 0; col < 8; col++)
                        if ((row + col) % 2 != 0)
                            Pieces.Add(new Checker(true, (row, col)));

                UpdateGameBoard();
            }
        }

        public void UpdateGameBoard()
        {
            Gameboard = new int[8, 8];
            foreach (IMovable movable in Pieces)
            {
                Gameboard[movable.Position.Item1, movable.Position.Item2] = movable.IsBlack ? 1 : -1;
            }
        }

        public void Select((int, int) pos)
        {
            foreach (Piece piece in Pieces)
            {
                if (piece.IsBlack == IsBlackTurn && piece.Position == pos)
                {
                    Selected = piece;
                    break;
                }
            }
        }

        public void Select((int, int) pos, bool IgnoreColor)
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
            foreach (IMovable piece in Pieces)
            {
                if (piece.IsBlack == IsBlackTurn && piece.AvailibleCaptures(Gameboard).Count() > 1)
                    MustCapture = true;
            }

            CheckWinCondition();
        }

        private void PromoteToKings()
        {
            //for (int i = 0; i < Pieces.Count; i++)
            //{
            //    var p = Pieces[i];
            //    if (p is Checker)
            //    {
            //        if ((!p.IsBlack && p.Position.Item1 == 0) || (p.IsBlack && p.Position.Item1 == 7))
            //        {
            //            Pieces[i] = new King(p.IsBlack, p.Position);
            //            OnKingPromotion?.Invoke(p.Position);
            //        }
            //    }
            //}
            if (Selected != null && Selected is Checker)
            {
                if ((!Selected.IsBlack && Selected.Position.Item1 == 0) || (Selected.IsBlack && Selected.Position.Item1 == 7))
                {
                    Pieces.Add(new King(Selected.IsBlack, Selected.Position));
                    Pieces.Remove(Selected);
                    OnKingPromotion?.Invoke(Selected.Position);
                    Select(Selected.Position, true);
                }
            }
        }

        private void Deselect()
        {
            Selected = null!;
            MoveSet.Clear();
        }

        private void Capture((int, int) beginning, (int, int) end)
        {
            Selected!.MoveTo(end);
            PromoteToKings();
            Deselect();
            int i = (Math.Sign(end.Item1 - beginning.Item1));
            int j = (Math.Sign(end.Item2 - beginning.Item2));
            (int, int) deletion = (beginning.Item1 + i, beginning.Item2 + j);
            while (deletion != end)
            {
                Select(deletion, true);
                if (Selected != null)
                {
                    Pieces.Remove(Selected);
                    Deselect();
                }
                deletion.Item1 += i;
                deletion.Item2 += j;
            }
            Select(end);
            UpdateGameBoard();
        }

        public void AttemptAction((int, int) pos)
        {
            if (Selected == null || !MoveSet.Contains(pos))
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
                    PromoteToKings();
                    PassMove();
                }
            }
        }

        public Game Clone()
        {
            var clone = new Game(false);
            clone.Pieces = this.Pieces.Select(p =>
            {
                if (p is King) return (Piece)new King(p.IsBlack, p.Position);
                return (Piece)new Checker(p.IsBlack, p.Position);
            }).ToList();
            clone.IsBlackTurn = this.IsBlackTurn;
            clone.MustCapture = this.MustCapture;
            clone.UpdateGameBoard();
            return clone;
        }

        public List<((int, int) from, (int, int) to)> GetAllMoves()
        {
            var result = new List<((int, int), (int, int))>();
            foreach (Piece piece in Pieces.Where(p => p.IsBlack == IsBlackTurn))
            {
                var moves = MustCapture
                    ? piece.AvailibleCaptures(Gameboard)
                    : piece.AvailibleMoves(Gameboard);

                foreach (var to in moves.Skip(1))
                    result.Add((piece.Position, to));
            }
            return result;
        }

        public bool WhiteWantsDraw { get; set; } = false;
        public bool BlackWantsDraw { get; set; } = false;

        public bool RequestDraw(bool isBlack)
        {
            if (isBlack)
                BlackWantsDraw = true;
            else
                WhiteWantsDraw = true;

            if (WhiteWantsDraw && BlackWantsDraw)
            {
                OnGameOver?.Invoke(null); // null = ничья
                return true;
            }
            return false;
        }

        public void DeclineDraw(bool isBlack)
        {
            // сбрасываем оба флага при отказе
            WhiteWantsDraw = false;
            BlackWantsDraw = false;
        }
    }
}