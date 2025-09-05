using Npgsql;

namespace BondTalesChat_Server.DatabaseInitializer
{
    public class DatabaseInitializer
    {
        public static void Initializer(string connectionString, string sqlFolderPath)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("⚠️ Connection string is empty, skipping database initialization");
                return;
            }

            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                connection.Open();

            foreach (var file in Directory.GetFiles(sqlFolderPath, "*.sql"))
            {
                Console.WriteLine($"Executing script: {Path.GetFileName(file)}");
                var script = File.ReadAllText(file);

                using var command = new NpgsqlCommand(script, connection);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (PostgresException ex)
                {
                    // Log but don't stop
                    Console.WriteLine($"⚠️ Skipped (already exists or harmless): {ex.Message}");
                }
            }

                Console.WriteLine("✅ Database initialization complete");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database initialization failed: {ex.Message}");
                Console.WriteLine("⚠️ Continuing without database initialization...");
            }
        }
    }
}
