namespace QTFK.Services
{
    public interface IEngineFeatures
    {
        bool SupportsTransactions { get; }
        bool SupportsStoredProcedures { get; }
    }
}