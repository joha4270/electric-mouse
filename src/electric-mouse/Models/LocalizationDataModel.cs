using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace electric_mouse.Models
{
    public class LocalizationDataModel
    {
        public LocalizationDataModel(string lang, IReadOnlyList<SelectListItem> allLanguages, Dictionary<string, string> strings)
        {
            Country = lang;
            Countries = allLanguages;
            Strings = strings;
        }

        public string Country { get; }

        public IReadOnlyList<SelectListItem> Countries { get; }

        public Dictionary<string, string> Strings { get; }
    }
}
