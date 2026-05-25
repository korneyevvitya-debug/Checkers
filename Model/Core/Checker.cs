using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Core
{
    public class Checker : Piece
    {
        public Checker(bool isBlack, (int,int) position) : base(isBlack, position) { }

        public override List<(int, int)> AvailibleCaptures(int[,] board)
        {
            List<(int,int)> MoveSet = new List<(int,int)> ();
            MoveSet.Add(Position);
            List<(int, int)> Directions = new List<(int, int)> { (1, 1), (1, -1), (-1 ,1), (-1, -1)};
            foreach((int x, int y) in Directions)
            {
                if (IsOnBoard(Position.Item1 + x * 2, Position.Item2 + y * 2) && board[Position.Item1 + x, Position.Item2 + y] != 0
                    && board[Position.Item1 + x * 2, Position.Item2 + y * 2] == 0 && (IsBlack == (board[Position.Item1 + x, Position.Item2 + y]<0)))
                {
                    MoveSet.Add((Position.Item1 + x * 2, Position.Item2 + y * 2));
                }
            }
            return MoveSet;
        }

        public override List<(int, int)> AvailibleMoves(int[,] board)
        {
            List<(int, int)> Directions = IsBlack ? new List<(int, int)> { (1, 1), (1, -1) } : new List<(int, int)> { (-1, 1), (-1, -1) };
            
            List<(int, int)> MoveSet = new List<(int, int)>();
            MoveSet.Add(Position);
            foreach((int x, int y) in Directions)
            {
                if (IsOnBoard(Position.Item1 + x, Position.Item2 +y) && board[Position.Item1 + x, Position.Item2 + y] ==0)
                {
                    MoveSet.Add((Position.Item1 + x, Position.Item2 + y));
                }

            }
            return MoveSet;
        }
    }
}
