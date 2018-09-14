using LinqToDB;
using LinqToDB.Data;
using Microsoft.Data.Sqlite;
using System;
using System.Threading.Tasks;

namespace SeniorAssistant.Extensions
{
    public static class DataConnectionExtensions
    {
        public static void CreateTableIfNotExists<T>(this DataConnection db) => RunIgnoringException(() => db.CreateTable<T>());

        public static async Task CreateTableIfNotExistsAsync<T>(this DataConnection db) => await RunIgnoringExceptionAsync(async () => await db.CreateTableAsync<T>());

        static void RunIgnoringException(Action action)
        {
            if (action == null) return;

            try
            {
                action.Invoke();
            }
            catch (SqliteException)
            {
                // Ignore Exception
            }
        }

        static async Task RunIgnoringExceptionAsync(Func<Task> action)
        {
            if (action == null) await Task.Yield();

            try
            {
                await action.Invoke();
            }
            catch
            { }
        }
    }
}
