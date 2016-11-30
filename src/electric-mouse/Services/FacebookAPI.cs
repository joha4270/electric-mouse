using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using electric_mouse.Models;
using Microsoft.Extensions.Logging;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace electric_mouse.Services
{
    public class FacebookAPI
    {
        private readonly ILogger<FacebookAPI> _logger;

        private const string token =
            "EAAX1ZBZArFrxwBABPjRMYpEJFlx5VpMElRs5ZCQaGz7ZCi8VnXWu7hQhOweBZC4bfn3u11bcWfZAuwSlZAsxKfB6JrhgrnYIUHvrM7p9IRAoznTRBZAIRmtBsJmOo1W8QVfZCOYZBBsNHZBnaL7TOzQsmzaZBmW6kEsItmbZBngmwZAAUREwZDZD";

        private HttpClient _client = new HttpClient(new HttpClientHandler(){AllowAutoRedirect = false});

        public FacebookAPI(ILoggerFactory loggerFactory )
        {
            _logger = loggerFactory.CreateLogger<FacebookAPI>();
        }

        public async Task<string> GetDisplayName(string userid, string token)
        {
            var resp = await _client.GetAsync($"https://graph.facebook.com/v2.8/{userid}/?access_token={token}");
            if (resp.IsSuccessStatusCode)
            {
                dynamic response = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(await resp.Content.ReadAsStringAsync());
                return response.name;
            }

            return "Error fetching name from facebook";
        }

        public async Task<string> GetAvatarURL(string userid, string notused)
        {
            HttpResponseMessage resp = await _client.GetAsync($"https://graph.facebook.com/v2.8/{userid}/picture");
            if (resp.StatusCode == HttpStatusCode.Redirect )
            {
                return resp.Headers.Location.ToString();
            }

            _logger.LogError("Facebook avatar request returned bad status (not 302). Using error image. Responce = {resp}", resp);

            return "/images/noavatar.jpg"; //TODO: load from config?
        }

        public async Task RefreshFacebookUserData(ApplicationUser user)
        {
            if (user.AuthTokenExpiration > DateTime.Now)
            {
                user.DisplayName = await GetDisplayName(user.FacebookID, user.AuthToken);
                user.URLPath = await GetAvatarURL(user.FacebookID, user.AuthToken);
            }
        }
    }
}