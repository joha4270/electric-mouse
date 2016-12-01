using System.Security.Cryptography.X509Certificates;
using electric_mouse.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace electric_mouse.Filters
{
    public class AddLanguageFilter : IActionFilter
    {
        private readonly LanguageCache _cache;
        private readonly string _defaultLanguage;

        public AddLanguageFilter(ILogger<AddLanguageFilter> logger, LanguageCache languageCache)//, IConfigurationRoot configuration)
        {
            _cache = languageCache;
            _defaultLanguage = "en-GB"; //TODO: Load from appsettings.json
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Controller controller = context.Controller as Controller;
            if(controller != null)
            {
                string lang = context.HttpContext.Request.Cookies["lang"];

                //If language is null or a not supported
                if (string.IsNullOrWhiteSpace(lang) || !_cache.AllLanguageSet.Contains(lang))
                {
                    //Write a (new?) cookie to the user to keep a default
                    //TODO: examine "Accept-Language" header
                    context.HttpContext.Response.Cookies.Append("lang", _defaultLanguage);
                    lang = _defaultLanguage;
                }

                controller.ViewBag.Language = _cache.GetLocalizationData(lang);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
