using Newtonsoft.Json;

namespace MyShopApp.WebApi.Localization
{
    public class LocalizationManager
    {
        public static Dictionary<string, string> Languages = new Dictionary<string, string>
        {
            ["Русский"] = "ru",
            ["English"] = "en"
        };

        private readonly Dictionary<string, Dictionary<string, string>> _resources;
        public static string CurrentLanguage { get; set; } = "ru";

        public LocalizationManager(string basePath)
        {
            _resources = new Dictionary<string, Dictionary<string, string>>();

            foreach (var file in Directory.EnumerateFiles(basePath, "*.json"))
            {
                var language = Path.GetFileNameWithoutExtension(file);
                var json = File.ReadAllText(file);
                _resources[language] = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
        }

        public string GetString(string key, string language)
        {
            if (_resources.TryGetValue(language, out var languageResources))
            {
                if (languageResources.TryGetValue(key, out var value))
                {
                    return value;
                }
            }

            return key;
        }

        public string GetString(string key)
        {
            return GetString(key, CurrentLanguage);
        }
    }
}
