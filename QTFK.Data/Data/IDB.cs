namespace QTFK.Data
{
    public interface IDB
    {
        IEngineFeatures EngineFeatures { get; }
        void save();
    }
}
