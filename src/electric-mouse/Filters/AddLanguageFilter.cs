using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace electric_mouse.Filters
{
    public class AddLanguageFilter : IActionFilter
    {
        private readonly ILogger<AddLanguageFilter> _logger;
        private readonly LanguageCache _cache;
        public AddLanguageFilter(ILogger<AddLanguageFilter> logger, LanguageCache languageCache)
        {
            _logger = logger;
            _cache = languageCache;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Controller controller = context.Controller as Controller;
            if(controller != null)
            {
                string lang = context.HttpContext.Request.Cookies["lang"] ?? "en-uk"; //TODO: Configure default language?
                controller.ViewBag.Strings = _cache.GetLanguage(lang);
            }


            
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        
    }
}
