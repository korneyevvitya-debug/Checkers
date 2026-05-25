namespace Model.Core
{
    public partial class Game
    {
        public void CheckWinCondition()
        {
            bool blackExists = Pieces.Exists(p => p.IsBlack);
            bool whiteExists = Pieces.Exists(p => !p.IsBlack);

            if (!blackExists) { OnGameOver?.Invoke(false); return; }
            if (!whiteExists) { OnGameOver?.Invoke(true); return; }

            // проверяем есть ли ходы у текущего игрока
            var moves = GetAllMoves();
            if (moves.Count == 0)
            {
                // текущий игрок не может походить — он проигрывает
                OnGameOver?.Invoke(!IsBlackTurn); // побеждает противник
            }
        }
    }
}