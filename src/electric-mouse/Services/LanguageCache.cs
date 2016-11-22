using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace electric_mouse
{
    public class LanguageCache
    {
        private readonly ILogger<LanguageCache> _logger;
        private readonly Dictionary<string, Generated.LocalizationStringContainer> loadedLanguages = new Dictionary<string, Generated.LocalizationStringContainer>();
        private readonly IHostingEnvironment _env;

        public List<SelectListItem> AllLanguages { get; private set; }

        public LanguageCache(ILogger<LanguageCache> logger, IHostingEnvironment env)
        {
            _logger = logger;
            _env = env;

            AllLanguages = GenerateLanguageList().OrderByDescending(x => x.Text).ToList();
        }

        private static readonly Regex languageStringRegex = new Regex("[a-z]{2}\\-[A-Z]{2}\\.json");
        private IEnumerable<SelectListItem> GenerateLanguageList()
        {
            string languageFilesPath = Path.Combine(_env.ContentRootPath, "Lang");
            foreach(string languageFile in Directory.EnumerateFiles(languageFilesPath).Where(s =>languageStringRegex.IsMatch(s)))
            {
                string rawpart = Path.GetFileNameWithoutExtension(languageFile);

                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(rawpart);
                yield return new SelectListItem { Value = rawpart, Text = culture.DisplayName };
            }
        }

        public Generated.LocalizationStringContainer GetLanguage(string languageName)
        {
            Generated.LocalizationStringContainer  instance;
            if(!loadedLanguages.TryGetValue(languageName, out instance))
            {
                instance = LoadLanguage(languageName);
                loadedLanguages[languageName] = instance;
            }

            return instance;
        }

        private Generated.LocalizationStringContainer LoadLanguage(string languageName)
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

            _logger.LogInformation($"Successfully parsed language {languageName} in {sw.ElapsedMilliseconds}ms containing {result.Count} strings");
            
            return new Generated.LocalizationStringContainer(result, languageName, _logger);
        }
    }
}
