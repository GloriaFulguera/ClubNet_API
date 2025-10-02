using Newtonsoft.Json;
using Npgsql;
using System.Data;
using System.Text.Json.Serialization;

namespace ClubNet.Services.Handlers
{
    public class PostgresHandler
    {
        public static string ConnectionString = string.Empty;

        public static bool Exec(string query, params (string, object)[] parameters)
        {
            try
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    var cmd = new NpgsqlCommand(query, conn);

                    foreach (var (name, value) in parameters)
                    {
                        cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
                    }

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetScalar(string query, params (string, object)[] parameters)
        {
            string scalarResult = string.Empty;

            try
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    var cmd = new NpgsqlCommand(query, conn);

                    foreach (var (name, value) in parameters)
                    {
                        cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
                    }

                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        scalarResult = result.ToString();
                    }
                }
                return scalarResult;
            }
            catch (Exception)
            {
                return scalarResult;
            }
        }

        public static DataTable GetDt(string query, params (string, object)[] parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    var cmd = new NpgsqlCommand(query, conn);
                    foreach (var (name, value) in parameters)
                    {
                        cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
                    }
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
                return dt;
            }
            catch (Exception)
            {
                return dt;
            }
        }

        public static string GetJson(string query, params (string, object)[] parameters)
        {
            return JsonConvert.SerializeObject(GetDt(query, parameters), Formatting.Indented);
        }
    }
}
