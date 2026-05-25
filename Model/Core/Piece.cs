using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Core
{
    public abstract class Piece : IMovable
    {

        public bool IsBlack { get; private set; }

        public (int, int) Position { get; protected set; }
        public Piece(bool isBlack, (int, int) position)
        {
            IsBlack = isBlack;
            Position = position;
        }

        public abstract List<(int, int)> AvailibleCaptures(int[,] board);

        public abstract List<(int, int)> AvailibleMoves(int[,] board);

        public void MoveTo((int, int) position)
        {
            Position = position;
        }
        protected bool IsOnBoard(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < 8 && y < 8);
        }
    }
}
