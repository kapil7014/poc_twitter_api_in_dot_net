using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MissionG3_TwitterWeb_API.data;
using MissionG3_TwitterWeb_API.logic.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using MissionG3_TwitterWeb_API.logic;
using Microsoft.Extensions.Options;
using MissionG3_TwitterWeb_API.data.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;

namespace MissionG3_TwitterWeb_API.api.Controllers
{
    [Route("api/[controller]")]
    public class TwitterTokenController : Controller
    {
        #region 'Property Initialization'
        private readonly ILogger<TwitterTokenController> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private TwitterToken twitterTokenLogic;
        #endregion

        #region 'Constructors'
        public TwitterTokenController(IHostingEnvironment hostingEnvironment, IConfiguration configuration, ILogger<TwitterTokenController> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            twitterTokenLogic = new TwitterToken(_configuration);
        }
        #endregion

        #region 'Apis'
        [AllowAnonymous]
        [HttpGet("TwitterLoginUrl")]
        public Task<TwitterLogin> TwitterLoginUrl()
        {
            var twitterLogin = new TwitterLogin();

            var loginUrl = _configuration["Twitter:TwitterAuthorizeUrl"] + "?response_type=code&client_id=" + _configuration["Twitter:ClientKeyId"] + "&redirect_uri=" + _configuration["Twitter:CallBackUrl"] +
                "&scope=" + _configuration["Twitter:TwitterScopes"].ToString().Replace(" ", "%20") + "&state=state&code_challenge=challenge&code_challenge_method=plain";
            twitterLogin.LoginUrl = loginUrl;

            return Task.FromResult(twitterLogin);
        }

        [AllowAnonymous]
        [HttpGet("GetTwitterAccessToken")]
        public async Task<OperationResult<TwitterAccessToken>> GetTwitterAccessToken([FromQuery] string code)
        {
            try
            {
                var processResult = await twitterTokenLogic.GetTwitterAccountAccessToken(code);

                if (processResult != null)
                {
                    return new OperationResult<TwitterAccessToken>(true, "", processResult);
                }
                else
                {
                    return new OperationResult<TwitterAccessToken>(false, "", processResult);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("GetTwitterAccessToken", ex);
                return new OperationResult<TwitterAccessToken>(false, CommonMessage.DefaultErrorMessage);
            }
        }

        [AllowAnonymous]
        [HttpGet("GetTwitterAuthUser")]
        public async Task<OperationResult<CommonModel<TwitterUser>>> GetTwitterAuthUser()
        {
            try
            {
                string authHeader = HttpContext.Request.Headers["Authorization"];
                string accessToken = authHeader.Substring("Bearer ".Length).Trim();

                var processResult = await twitterTokenLogic.GetTwitterAuthenticateUser(accessToken);

                if (processResult != null)
                {
                    return new OperationResult<CommonModel<TwitterUser>>(true, "", processResult);
                }
                else
                {
                    return new OperationResult<CommonModel<TwitterUser>>(false, "", processResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GetTwitterAccessToken", ex);
                return new OperationResult<CommonModel<TwitterUser>>(false, CommonMessage.DefaultErrorMessage);
            }
        }
        #endregion

    }
}