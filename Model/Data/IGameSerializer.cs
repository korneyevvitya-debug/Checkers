namespace Model.Data
{
    public interface IGameSerializer
    {
        void Save(Model.Core.Game game, string path);
        Model.Core.Game? Load(string path);
        bool IsValidSave(string path);
    }
}