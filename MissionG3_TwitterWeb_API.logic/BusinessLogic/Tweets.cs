using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using MissionG3_TwitterWeb_API.data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MissionG3_TwitterWeb_API.logic
{
    public class Tweets
    {
        #region 'Property Initialization'
        private static readonly HttpClient client = new HttpClient();
        private readonly IConfiguration _configuration;
        #endregion

        #region 'Constructors'
        public Tweets(IConfiguration configuration)
        {
            _configuration = configuration;

            if (client.BaseAddress == null)
                client.BaseAddress = new Uri(_configuration["Twitter:TwitterApiUrl"]);
        }
        #endregion

        #region 'Get Functions'
        public async Task<object> GetReverseTimelineTweets(string userId, string accessToken)
        {
            client.DefaultRequestHeaders.Clear();

            //Client credentials
            client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", accessToken));

            var queryParam = new Dictionary<string, string>()
            {
                ["tweet.fields"] = "entities,created_at,attachments",
                ["expansions"] = "attachments.media_keys,author_id",
                ["user.fields"] = "profile_image_url,name,username",
                ["media.fields"] = "preview_image_url,url",
                ["place.fields"] = "full_name"
            };

            var tweetsApiUri = QueryHelpers.AddQueryString("2/users/" + userId + "/timelines/reverse_chronological", queryParam);

            var response = await client.GetAsync(tweetsApiUri);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseResult = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<object>(responseResult);
            }

            return null;
        }
        #endregion
    }
}
