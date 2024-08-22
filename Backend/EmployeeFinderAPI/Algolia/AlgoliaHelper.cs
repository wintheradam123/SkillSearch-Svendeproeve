using Algolia.Search.Clients;
using Algolia.Search.Models.Search;
using Algolia.Search.Models.Settings;
using Shared.Models;

namespace EmployeeHangfireCron.Algolia
{
    public class AlgoliaHelper
    {
        public static AlgoliaSettings LoadAlgoliaSettingsUsers()
        {
            // Load settings
            IConfiguration config = new ConfigurationBuilder()
                // appsettings.json is required
                .AddJsonFile("appsettings.json", optional: false)
                // appsettings.Development.json" is optional, values override appsettings.json
                .AddJsonFile($"appsettings.Development.json", optional: true)
                // User secrets are optional, values override both JSON files
                .AddUserSecrets<Program>()
                .Build();

            return config.GetRequiredSection("AlgoliaSettings").Get<AlgoliaSettings>() ??
                   throw new Exception("Could not load app settings.");
        }

        public static AlgoliaSettings LoadAlgoliaSettingsSkills()
        {
            // Load settings
            IConfiguration config = new ConfigurationBuilder()
                // appsettings.json is required
                .AddJsonFile("appsettings.json", optional: false)
                // appsettings.Development.json" is optional, values override appsettings.json
                .AddJsonFile($"appsettings.Development.json", optional: true)
                // User secrets are optional, values override both JSON files
                .AddUserSecrets<Program>()
                .Build();

            return config.GetRequiredSection("AlgoliaSettingsSkills").Get<AlgoliaSettings>() ??
                   throw new Exception("Could not load app settings.");
        }

        public static AlgoliaSettings LoadAlgoliaSettingsProjects()
        {
            // Load settings
            IConfiguration config = new ConfigurationBuilder()
                // appsettings.json is required
                .AddJsonFile("appsettings.json", optional: false)
                // appsettings.Development.json" is optional, values override appsettings.json
                .AddJsonFile($"appsettings.Development.json", optional: true)
                // User secrets are optional, values override both JSON files
                .AddUserSecrets<Program>()
                .Build();

            return config.GetRequiredSection("AlgoliaSettingsProjects").Get<AlgoliaSettings>() ??
                   throw new Exception("Could not load app settings.");
        }

        //public static void IndexSettings()
        //{
        //    IndexSettings settings = new IndexSettings();
        //    settings.Ranking = new List<string>
        //    {
        //        "typo",
        //        "geo",
        //        "words",
        //        "filters",
        //        "attribute",
        //        "proximity",
        //        "exact",
        //        "custom"
        //    };

        //    index.SetSettings(settings);
        //}
    }
}
