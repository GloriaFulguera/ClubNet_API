using Npgsql;

namespace ClubNet.Services.Handlers
{
    public class PostgresHandler
    {
        public static string ConnectionString = string.Empty;

        public static bool Exec(string query)
        {
            bool result = false;
            try
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    var cmd = new NpgsqlCommand(query, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
    }
}
