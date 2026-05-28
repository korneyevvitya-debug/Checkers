using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Model.Data
{
    public class GameState
    {
        public List<PieceState> Pieces { get; set; } = new();
        public bool IsBlackTurn { get; set; }
        public bool VsComputer { get; set; }
        public bool MustCapture { get; set; }
    }

    public class PieceState
    {
        public bool IsBlack { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public bool IsKing { get; set; }
    }

    public class GameSerializer : GameSerializerBase
    {
        public override void Save(Model.Core.Game game, string path)
        {
            var state = new GameState();
            state.IsBlackTurn = game.IsBlackTurn;
            state.VsComputer = game is Model.Core.GameVsComputer;
            state.MustCapture = game.MustCapture;

            foreach (var piece in game.Pieces)
            {
                state.Pieces.Add(new PieceState
                {
                    IsBlack = piece.IsBlack,
                    Row = piece.Position.Item1,
                    Col = piece.Position.Item2,
                    IsKing = piece is Model.Core.King
                });
            }

            var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }

        public override Model.Core.Game? Load(string path)
        {
            if (!File.Exists(path)) return null;
            try
            {
                var json = File.ReadAllText(path);
                var state = JsonSerializer.Deserialize<GameState>(json);
                if (state == null) return null;

                var game = state.VsComputer
                    ? (Model.Core.Game)new Model.Core.GameVsComputer(false)
                    : new Model.Core.Game(false);

                game.IsBlackTurn = state.IsBlackTurn;
                game.MustCapture = state.MustCapture;

                foreach (var p in state.Pieces)
                {
                    if (p.IsKing)
                        game.Pieces.Add(new Model.Core.King(p.IsBlack, (p.Row, p.Col)));
                    else
                        game.Pieces.Add(new Model.Core.Checker(p.IsBlack, (p.Row, p.Col)));
                }
                
                game.UpdateGameBoard();
                return game;
            }
            catch
            {
                return null;
            }
        }

        public override bool IsValidSave(string path)
        {
            if (!File.Exists(path)) return false;
            try
            {
                var json = File.ReadAllText(path);
                var state = JsonSerializer.Deserialize<GameState>(json);
                return state != null && state.Pieces != null && state.Pieces.Count > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}