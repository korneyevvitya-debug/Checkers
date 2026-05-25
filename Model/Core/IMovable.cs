using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Core
{
    public interface IMovable
    {
        bool IsBlack { get; }
        (int, int) Position { get; }
        public List<(int, int)> AvailibleMoves(int[,] board);
        public List<(int, int)> AvailibleCaptures(int[,] board);
        public void MoveTo((int, int) position);
    }
}
