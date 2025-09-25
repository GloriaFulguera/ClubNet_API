using Npgsql;

namespace ClubNet.Services.Handlers
{
    public class PostgresHandler
    {
        public static string ConnectionString = string.Empty;

        public static bool Exec(string query, params (string, object)[] parameters)
        {
            bool result = false;
            try
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    var cmd = new NpgsqlCommand(query, conn);

                    foreach (var (name,value) in parameters)
                    {
                        cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
                    }

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
