using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using electric_mouse.Models;
using Microsoft.Extensions.Logging;

namespace electric_mouse.Services
{
    public class FacebookAPI
    {
        private readonly ILogger<FacebookAPI> _logger;

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

        public async Task<string> GetAvatarURL(string userid)
        {
            HttpResponseMessage resp = await _client.GetAsync($"https://graph.facebook.com/v2.8/{userid}/picture?width=64&height=64&redirect=false");

            if (resp.StatusCode == HttpStatusCode.OK)
            {
                string re = await resp.Content.ReadAsStringAsync();
                dynamic d = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(re);

                if (d.data != null)
                    return d.data.url;
                else
                    _logger.LogError("Facebook avatar request returned error. Content = {resp}", re);
            }
            else
            {
                _logger.LogError("Facebook avatar request returned bad status (not 200). Using error image. Responce = {resp}", resp);
            }

            return "/images/noavatar.jpg"; //TODO: load from config?
        }

        public async Task RefreshFacebookUserData(ApplicationUser user)
        {
            if (user.AuthTokenExpiration > DateTime.Now)
            {
                user.DisplayName = await GetDisplayName(user.FacebookID, user.AuthToken);
                user.URLPath = await GetAvatarURL(user.FacebookID);
            }
        }
    }
}