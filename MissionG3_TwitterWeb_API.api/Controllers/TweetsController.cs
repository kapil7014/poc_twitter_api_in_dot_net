using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MissionG3_TwitterWeb_API.data.Models;
using MissionG3_TwitterWeb_API.logic;
using MissionG3_TwitterWeb_API.logic.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissionG3_TwitterWeb_API.api.Controllers
{
    [Route("api/[controller]")]
    public class TweetsController : Controller
    {
        #region 'Property Initialization'
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<TweetsController> _logger;
        private readonly IConfiguration _configuration;
        private Tweets twitterTweets;
        #endregion

        #region 'Constructors'
        public TweetsController(IHostingEnvironment hostingEnvironment, IConfiguration configuration, ILogger<TweetsController> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            twitterTweets = new Tweets(_configuration);
        }
        #endregion

        #region 'Apis'
        [AllowAnonymous]
        [HttpGet("GetReverseTimelineTweets")]
        public async Task<OperationResult<object>> GetReverseTimelineTweets([FromQuery] string userId)
        {
            try
            {
                string authHeader = HttpContext.Request.Headers["Authorization"];
                string accessToken = authHeader.Substring("Bearer ".Length).Trim();

                var processResult = await twitterTweets.GetReverseTimelineTweets(userId, accessToken);

                if (processResult != null)
                {
                    return new OperationResult<object>(true, "", processResult);
                } 
                else
                {
                    return new OperationResult<object>(false, "", processResult);
                }                    
            }
            catch (Exception ex)
            {
                _logger.LogError("GetTwitterAccessToken", ex);
                return new OperationResult<object>(false, CommonMessage.DefaultErrorMessage);
            }
        }
        #endregion
    }
}
