using System.Data.SQLite;

namespace AuthAPI.HelperMethods
{
    public static class SqlConnection
    {
        public static SQLiteConnection CreateConnection(string connectionString)
        {
            return new SQLiteConnection(connectionString);
        }
    }
}
