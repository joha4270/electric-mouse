using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace electric_mouse.Models
{
    public class LocalizationDataModel
    {
        private string lang;
        private List<SelectListItem> allLanguages;
        private Dictionary<string, string> dictionary;

        public LocalizationDataModel(string lang, List<SelectListItem> allLanguages, Generated.LocalizationStringContainer strings)
        {
            Country = lang;
            Countries = allLanguages;
            Strings = strings;
        }

        public string Country { get; }

        public List<SelectListItem> Countries { get; }

        public Generated.LocalizationStringContainer Strings { get; }
    }
}
