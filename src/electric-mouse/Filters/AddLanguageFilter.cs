using electric_mouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace electric_mouse.Filters
{
    public class AddLanguageFilter : IActionFilter
    {
        private readonly LanguageCache _cache;
        public AddLanguageFilter(ILogger<AddLanguageFilter> logger, LanguageCache languageCache)
        {
            _cache = languageCache;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Controller controller = context.Controller as Controller;
            if(controller != null)
            {
                string lang = context.HttpContext.Request.Cookies["lang"] ?? "en-GB"; //TODO: Configure default language?
                controller.ViewBag.Lozalization = new LocalizationDataModel(lang, _cache.AllLanguages, _cache.GetLanguage(lang));
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
