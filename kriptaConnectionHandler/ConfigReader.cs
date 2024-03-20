using System.Configuration;

namespace Helper
{
    public class ConfigReader
    {
        public static string GetConfigurationValue(string key)
        {
            var KeyValue = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("KriptaKeySettings")[key];
            if (KeyValue != null)
                return KeyValue;
            throw new InvalidDataException($"Configuration key '{key}' not found.");
        }
    }
}
