using System;
using System.Net.Http;
using System.Web.Http;
using WebApiApplication.Models;
using static WebApiApplication.Helpers.AuthorizationHelper;

namespace WebApiApplication.Controllers
{
    /// <summary>
    /// 基礎API
    /// </summary>
    public class BaseApiController : ApiController
    {
        #region 共用參數
        protected WebApiDBContext db = new WebApiDBContext();
        protected BearerTokenManager bearerTokenManager = new BearerTokenManager();
        protected int timestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        protected string host, path, url, jsonString, responseResult;
        protected HttpRequestMessage requestMessage;
        protected HttpResponseMessage responseMessage;
        #endregion
    }
}
