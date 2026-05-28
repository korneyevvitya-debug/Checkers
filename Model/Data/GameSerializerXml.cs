using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Model.Data
{
    public class GameSerializerXml : GameSerializerBase
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

        var serializer = new XmlSerializer(typeof(GameState));
        using var writer = new StreamWriter(path);
        serializer.Serialize(writer, state);
    }

    public override Model.Core.Game? Load(string path)
    {
        if (!File.Exists(path)) return null;
        try
        {
            var serializer = new XmlSerializer(typeof(GameState));
            using var reader = new StreamReader(path);
            var state = (GameState?)serializer.Deserialize(reader);
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
            var serializer = new XmlSerializer(typeof(GameState));
            using var reader = new StreamReader(path);
            var state = (GameState?)serializer.Deserialize(reader);
            return state != null && state.Pieces != null && state.Pieces.Count > 0;
        }
        catch
        {
            return false;
        }
    }
}
}