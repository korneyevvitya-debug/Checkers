namespace Model.Data
{
    public abstract class GameSerializerBase
    {
        public abstract void Save(Model.Core.Game game, string path);
        public abstract Model.Core.Game? Load(string path);
        public abstract bool IsValidSave(string path);
    }
}