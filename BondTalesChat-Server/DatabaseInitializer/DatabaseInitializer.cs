using Microsoft.Data.SqlClient;

namespace BondTalesChat_Server.DatabaseInitializer
{
    public class DatabaseInitializer
    {
        public static void Initializer(string connectionString,string sqlFolderPath)
        {
            using var connection=new SqlConnection(connectionString);
            connection.Open();

            foreach(var file in Directory.GetFiles(sqlFolderPath, "*.sql"))
            {
                Console.WriteLine($"Executing script:{Path.GetFileName(file)}");
                var script=File.ReadAllText(file);
                using var command=new SqlCommand(script,connection);
                command.ExecuteNonQuery();
            }
            Console.WriteLine("DataBase initialization complete");
        }
    }
}
