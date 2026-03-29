using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Reflection;

namespace marker_dotnet.Models
{
    public class AppConfig
    {
        public string ProgramName { get; set; }
        public string PrinterName { get; set; }
        public string ApiUrl { get; set; }
        public string ApiUrlStatus { get; set; }
        public bool IsConfigured { get; set; }
        public DateTime InstalledDate { get; set; }
    }

    public class ConfigService
    {
        private static readonly string DatabasePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "MarkerCrystal", 
            "config.db"
        );

        private static readonly string DefaultApiUrl = "https://api-marker.crystalclean.co.id/api/get-print-batch-data";
        private static readonly string DefaultApiUrlStatus = "https://api-marker.crystalclean.co.id/api";
        private static readonly string RequiredPassword = "081999967373";

        private static bool _databaseInitialized = false;

        public static void InitializeDatabase()
        {
            if (_databaseInitialized) return;

            var directory = Path.GetDirectoryName(DatabasePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var connection = new SqliteConnection($"Data Source={DatabasePath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS config (
                    id INTEGER PRIMARY KEY,
                    program_name TEXT NOT NULL,
                    printer_name TEXT NOT NULL,
                    api_url TEXT NOT NULL,
                    api_url_status TEXT NOT NULL,
                    is_configured INTEGER NOT NULL DEFAULT 0,
                    installed_date TEXT NOT NULL,
                    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
                )";

            command.ExecuteNonQuery();

            _databaseInitialized = true;
        }

        static ConfigService()
        {
            InitializeDatabase();
        }

        public static AppConfig GetConfig()
        {
            InitializeDatabase();

            using var connection = new SqliteConnection($"Data Source={DatabasePath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM config WHERE id = 1";

            using var reader = command.ExecuteReader();
            
            if (reader.Read())
            {
                return new AppConfig
                {
                    ProgramName = reader.GetString(0),
                    PrinterName = reader.GetString(1),
                    ApiUrl = reader.GetString(2),
                    ApiUrlStatus = reader.GetString(3),
                    IsConfigured = reader.GetBoolean(4),
                    InstalledDate = DateTime.Parse(reader.GetString(5))
                };
            }

            return new AppConfig
            {
                ProgramName = "Printer marker",
                PrinterName = "EPSON TM-U220B",
                ApiUrl = DefaultApiUrl,
                ApiUrlStatus = DefaultApiUrlStatus,
                IsConfigured = false,
                InstalledDate = DateTime.Now
            };
        }

        public static bool SaveConfig(string programName, string printerName, string password)
        {
            if (password != RequiredPassword)
            {
                return false;
            }

            InitializeDatabase();

            using var connection = new SqliteConnection($"Data Source={DatabasePath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT OR REPLACE INTO config 
                (id, program_name, printer_name, api_url, api_url_status, is_configured, installed_date)
                VALUES (1, @program_name, @printer_name, @api_url, @api_url_status, 1, @installed_date)";

            command.Parameters.AddWithValue("@program_name", programName);
            command.Parameters.AddWithValue("@printer_name", printerName);
            command.Parameters.AddWithValue("@api_url", DefaultApiUrl);
            command.Parameters.AddWithValue("@api_url_status", DefaultApiUrlStatus);
            command.Parameters.AddWithValue("@installed_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            command.ExecuteNonQuery();
            return true;
        }

        public static bool ValidatePassword(string password)
        {
            return password == RequiredPassword;
        }

        public static void ResetConfig()
        {
            InitializeDatabase();

            using var connection = new SqliteConnection($"Data Source={DatabasePath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM config WHERE id = 1";
            command.ExecuteNonQuery();
        }

        public static bool IsFirstRun()
        {
            var config = GetConfig();
            return !config.IsConfigured;
        }

        public static void CreateDefaultConfig()
        {
            InitializeDatabase();
            
            var config = GetConfig();
            if (!config.IsConfigured)
            {
                SaveConfig("Printer marker", "EPSON TM-U220B", RequiredPassword);
            }
        }
    }
}
