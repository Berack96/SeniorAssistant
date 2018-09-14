using LinqToDB.DataProvider;

namespace SeniorAssistant.Data
{
    public class SeniorDataContextFactory : IDataContextFactory<SeniorDataContext>
    {
        readonly IDataProvider dataProvider;

        readonly string connectionString;

        public SeniorDataContextFactory(IDataProvider dataProvider, string connectionString)
        {
            this.dataProvider = dataProvider;
            this.connectionString = connectionString;
        }

        public SeniorDataContext Create() => new SeniorDataContext(dataProvider, connectionString);
    }
}
