using LinqToDB.Data;

namespace SeniorAssistant.Data
{
    public interface IDataContextFactory<T>
        where T : DataConnection
    {
        T Create();
    }
}
