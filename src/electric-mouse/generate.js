var fs = require('fs');
var path = require('path');

var inpath = path.join(__dirname, "Lang", "en-GB.json");
var outpath = path.join(__dirname, "Generated", "LocalizationStringsContainer.cs");
var data = JSON.parse(fs.readFileSync(inpath,'utf8').replace(/^\uFEFF/,""));

var output = "using System.Collections.Generic;\n\
using Microsoft.Extensions.Logging;\n\
namespace electric_mouse.Generated\n\
{\n\
    public class LocalizationStringContainer\n\
    {\n";

for (var property in data) {
    if (data.hasOwnProperty(property)) {
        output += "\t\tpublic string " + property + "{ get; set; }\n";
    }
}
output += "\t\t\n\t\tpublic LocalizationStringContainer(Dictionary<string, string> languageDefinition, string language, ILogger<LanguageCache> log)\n\
\t\t{\n";

for (var property in data) {
    if (data.hasOwnProperty(property)) {
        output += "\t\t\t" + property + " = Get(\"" + property + "\", language, languageDefinition, log);\n"; //<#= kvp.Key #> = Get("<#= kvp.Key #>", language, languageDefinition, log);
    }
}

output += "\n\t\t}\n\
\t\tprivate string Get(string key, string language, Dictionary<string, string> languageDefinition, ILogger<LanguageCache> log)\n\
\t\t{\n\
\t\t    string result;\n\
\t\t    if(languageDefinition.TryGetValue(key, out result))\n\
\t\t\t{\n\
\t\t\t    return result;\n\
\t\t\t}\n\
\t\t\telse\n\
\t\t\t{\n\
\t\t\t    log.LogError($\"Missing key \\\"{key}\\\" during loading of \\\"{language}\");\n\
\t\t\t    return key;\n\
\t\t\t}\n\
\t\t}\t}\n}";
console.log("writing generated file to " +outpath);

fs.writeFileSync(outpath, output);