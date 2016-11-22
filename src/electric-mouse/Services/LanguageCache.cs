using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;

namespace electric_mouse
{
    public class LanguageCache
    {
        private readonly ILogger<LanguageCache> _logger;
        private readonly Dictionary<string, Dictionary<string, string>> loadedLanguages = new Dictionary<string, Dictionary<string, string>>();
        private readonly IHostingEnvironment _env;
        public LanguageCache(ILogger<LanguageCache> logger, IHostingEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public Dictionary<string, string> GetLanguage(string languageName)
        {
            Dictionary<string, string> instance;
            if(!loadedLanguages.TryGetValue(languageName, out instance))
            {
                instance = LoadLanguage(languageName);
                loadedLanguages[languageName] = instance;
            }

            return instance;
        }

        private Dictionary<string, string> LoadLanguage(string languageName)
        {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation($"Loading new language {languageName}");

            string appPath = _env.ContentRootPath;
            string languageFilePath = Path.Combine(appPath, "Lang", languageName + ".json");
            
            if(!File.Exists(languageFilePath))
            {
                throw new FileNotFoundException($"Language file not found. No language named {languageName}");
            }


            Dictionary<string, string> result = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(languageFilePath));
            _logger.LogInformation($"Successfully loaded language {languageName} in {sw.ElapsedMilliseconds}ms containing {result.Count} strings");
            return result;
        }
    }
}
