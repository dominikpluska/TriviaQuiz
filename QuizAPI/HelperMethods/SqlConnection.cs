using System.Data.SQLite;

namespace QuizAPI.HelperMethods
{
    public static class SqlConnection
    {
        public static SQLiteConnection CreateConnection (string connectionString)
        {
            return new SQLiteConnection(connectionString);
        }
    }
}
