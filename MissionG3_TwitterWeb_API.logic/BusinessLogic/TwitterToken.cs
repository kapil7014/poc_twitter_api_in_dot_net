using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MissionG3_TwitterWeb_API.data.Models;
using Newtonsoft.Json;

namespace MissionG3_TwitterWeb_API.logic
{
    public class TwitterToken
    {
        #region 'Property Initialization'
        private static readonly HttpClient client = new HttpClient();
        private readonly IConfiguration _configuration;
        #endregion

        #region 'Constructors'
        public TwitterToken(IConfiguration configuration)
        {
            _configuration = configuration;

            if (client.BaseAddress == null)
                client.BaseAddress = new Uri(_configuration["Twitter:TwitterApiUrl"]);
        }
        #endregion

        #region 'Get Functions'
        //Get Twitter Access Token Using Authentication
        public async Task<TwitterAccessToken> GetTwitterAccountAccessToken(string code)
        {
            client.DefaultRequestHeaders.Clear();

            //Client credentials
            var encodedClientKey = WebUtility.UrlEncode(_configuration["Twitter:ClientKeyId"]);
            var encodedClientSecret = WebUtility.UrlEncode(_configuration["Twitter:ClientSecretKey"]);

            var combinedKeys = String.Format("{0}:{1}", encodedClientKey, encodedClientSecret);
            var utfBytes = System.Text.Encoding.UTF8.GetBytes(combinedKeys);
            var encodedString = Convert.ToBase64String(utfBytes);
            client.DefaultRequestHeaders.Add("Authorization", string.Format("Basic {0}", encodedString));

            var param = new Dictionary<string, string>();
            param.Add("code", code);
            param.Add("grant_type", "authorization_code");
            param.Add("client_id", _configuration["Twitter:ClientKeyId"]);
            param.Add("redirect_uri", _configuration["Twitter:CallBackUrl"]);
            param.Add("code_verifier", "challenge");

            var response = await client.PostAsync("2/oauth2/token", new FormUrlEncodedContent(param));

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;
                var twitterAccessToken = JsonConvert.DeserializeObject<TwitterAccessToken>(responseResult);
                return twitterAccessToken;
            }

            return null;
        }

        public async Task<CommonModel<TwitterUser>> GetTwitterAuthenticateUser(string accessToken)
        {
            client.DefaultRequestHeaders.Clear();

            //Client credentials
            client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", accessToken));

            var response = await client.GetAsync("2/users/me?user.fields=profile_image_url");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<CommonModel<TwitterUser>>(responseResult);
            }

            return null;
        }
        #endregion

    }
}
