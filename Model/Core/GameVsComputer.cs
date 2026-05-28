using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Core
{
	public class GameVsComputer : Game
	{
		private static readonly Random _random = new Random();
		public bool IsComputerTurn => IsBlackTurn; // компьютер играет за чёрных

        public void MakeComputerMove()
        {
			
            // делаем один ход
            var bestMove = GetBestMove(depth: 5);
            if (bestMove == null) return;

            AttemptAction(bestMove.Value.from);
            AttemptAction(bestMove.Value.to);

            // продолжаем только если идёт серия взятий той же фигурой
            int safety = 0;
            while (IsComputerTurn && MustCapture && MoveSet.Count > 0 && safety < 20)
            {
                var captureMove = GetBestMove(depth: 3);
                if (captureMove == null) break;
                AttemptAction(captureMove.Value.to);
                safety++;
            }
        }

        private ((int, int) from, (int, int) to)? GetBestMove(int depth)
		{
			var moves = GetAllMoves();
			if (moves.Count == 0) return null;

			((int, int) from, (int, int) to)? bestMove = null;
			int bestScore = int.MinValue;

			foreach (var move in moves)
			{
				var clone = this.Clone();
				clone.AttemptAction(move.from);
				clone.AttemptAction(move.to);

				int score = Minimax(clone, depth - 1, int.MinValue, int.MaxValue, false);
				if (score > bestScore)
				{
					bestScore = score;
					bestMove = move;
				}
			}

			return bestMove;
		}

		private int Minimax(Game game, int depth, int alpha, int beta, bool isMaximizing)
		{
			var moves = game.GetAllMoves();

			if (depth == 0 || moves.Count == 0)
				return Evaluate(game);

			if (isMaximizing)
			{
				int maxScore = int.MinValue;
				foreach (var move in moves)
				{
					var clone = game.Clone();
					clone.AttemptAction(move.from);
					clone.AttemptAction(move.to);

					int score = Minimax(clone, depth - 1, alpha, beta, false);
					maxScore = Math.Max(maxScore, score);
					alpha = Math.Max(alpha, score);
					if (beta <= alpha) break; // отсечение
				}
				return maxScore;
			}
			else
			{
				int minScore = int.MaxValue;
				foreach (var move in moves)
				{
					var clone = game.Clone();
					clone.AttemptAction(move.from);
					clone.AttemptAction(move.to);

					int score = Minimax(clone, depth - 1, alpha, beta, true);
					minScore = Math.Min(minScore, score);
					beta = Math.Min(beta, score);
					if (beta <= alpha) break; // отсечение
				}
				return minScore;
			}
		}

		private int Evaluate(Game game)
		{
			int score = 0;
			foreach (var piece in game.Pieces)
			{
				if (piece.IsBlack)
					score += 1; // очко за шашку компьютера
				else
					score -= 1; // минус очко за шашку игрока
			}
			return score;
		}
        public GameVsComputer(bool withPieces) : base(withPieces) { }
    }
}