using System;
using System.Collections.Generic;

namespace Model.Core
{
    public class King : Piece
    {
        public King(bool isBlack, (int, int) position) : base(isBlack, position) { }

        public override List<(int, int)> AvailibleMoves(int[,] board)
        {
            var moveSet = new List<(int, int)>();
            moveSet.Add(Position);
            var directions = new List<(int, int)> { (1, 1), (1, -1), (-1, 1), (-1, -1) };

            foreach ((int dx, int dy) in directions)
            {
                int x = Position.Item1 + dx;
                int y = Position.Item2 + dy;
                while (IsOnBoard(x, y) && board[x, y] == 0)
                {
                    moveSet.Add((x, y));
                    x += dx;
                    y += dy;
                }
            }
            return moveSet;
        }

        public override List<(int, int)> AvailibleCaptures(int[,] board)
        {
            var moveSet = new List<(int, int)>();
            moveSet.Add(Position);
            var directions = new List<(int, int)> { (1, 1), (1, -1), (-1, 1), (-1, -1) };

            foreach ((int dx, int dy) in directions)
            {
                int x = Position.Item1 + dx;
                int y = Position.Item2 + dy;
                // ищем вражескую шашку
                while (IsOnBoard(x, y) && board[x, y] == 0)
                {
                    x += dx;
                    y += dy;
                }
                // нашли фигуру — проверяем что она вражеская
                if (IsOnBoard(x, y) && board[x, y] != 0 && (IsBlack == (board[x, y] < 0)))
                {
                    int ex = x + dx;
                    int ey = y + dy;
                    // все клетки за вражеской фигурой доступны для приземления
                    while (IsOnBoard(ex, ey) && board[ex, ey] == 0)
                    {
                        moveSet.Add((ex, ey));
                        ex += dx;
                        ey += dy;
                    }
                }
            }
            return moveSet;
        }
    }
}