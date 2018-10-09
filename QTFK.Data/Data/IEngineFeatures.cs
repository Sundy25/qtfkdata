namespace QTFK.Data
{
    public interface IEngineFeatures
    {
        bool SupportsTransactions { get; }
        bool SupportsStoredProcedures { get; }
    }
}