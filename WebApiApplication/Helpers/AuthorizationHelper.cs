using Common;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApiApplication.Models;
using WebApiApplication.ViewModels;

namespace WebApiApplication.Helpers
{
    /// <summary>
    /// 授權助手
    /// </summary>
    public class AuthorizationHelper
    {
        /// <summary>
        /// 基本身份驗證管理器
        /// </summary>
        public class BasicAuthManager
        {
            /// <summary>
            /// 基本身份驗證授權檢查
            /// </summary>
            public class BasicAuthAuthorizationFilterAttribute : AuthorizationFilterAttribute
            {
                public override void OnAuthorization(HttpActionContext actionContext)
                {
                    WebApiDBContext db = new WebApiDBContext();
                    string authorizationHelper = string.Empty;

                    if (actionContext.Request.Headers.Authorization == null || actionContext.Request.Headers.Authorization.Scheme != "Basic")
                        setErrorResponse(actionContext, "驗證錯誤：授權資料錯誤");
                    else
                    {
                        try
                        {
                            authorizationHelper = actionContext.Request.Headers.Authorization.Parameter;

                            if (string.IsNullOrEmpty(authorizationHelper))
                                setErrorResponse(actionContext, "驗證錯誤：無授權資料");

                            //使用 Base64 反編碼 取得 auth
                            var auth = Encoding.Default.GetString(Convert.FromBase64String(authorizationHelper));

                            var authData = auth.Split(':');
                            if (authData.Length < 2)
                                setErrorResponse(actionContext, "驗證錯誤：授權資料格式錯誤");

                            var name = authData[0];
                            var password = authData[1];

                            //取得使用者資料
                            var userData = db.Users.Where(x => x.UserId == name).FirstOrDefault();
                            if (userData == null)
                                setErrorResponse(actionContext, "驗證錯誤:使用者不存在");

                            string hashPassword = CryptoHelper.GenerateHash(password, userData.ApiKey);
                            var userCheck = db.Users.Where(x => x.UserId == name && x.Password == hashPassword).Any();
                            if(!userCheck)
                                setErrorResponse(actionContext, "驗證錯誤:使用者帳號密碼錯誤");
                        }
                        catch (Exception ex)
                        {
                            setErrorResponse(actionContext, ex.Message);
                        }
                    }

                    base.OnAuthorization(actionContext);
                }
            }
        }

        /// <summary>
        /// 承載令牌管理器
        /// </summary>
        public class BearerTokenManager
        {
            /// <summary>
            /// 標題
            /// </summary>
            public class Header
            {
                /// <summary>
                /// 主要演算法
                /// </summary>
                public string alg { get; set; }

                /// <summary>
                /// 媒體類型
                /// </summary>
                public string typ { get; set; }

                /// <summary>
                /// 內容類型
                /// </summary>
                public SubHeader cty { get; set; }

                public class SubHeader
                {
                    /// <summary>
                    /// 使用者編號
                    /// </summary>
                    public string userId { get; set; }

                    /// <summary>
                    /// 初始化向量
                    /// </summary>
                    public string iv { get; set; }
                }
            }

            /// <summary>
            /// 驗證資訊
            /// </summary>
            public class Payload
            {
                /// <summary>
                /// 使用者資訊
                /// </summary>
                public User userInfo { get; set; }

                /// <summary>
                /// 過期時間
                /// </summary>
                public int exp { get; set; }
            }

            /// <summary>
            /// 創建令牌
            /// </summary>
            public TokenResponseData CreateToken(User user)
            {
                var iv = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);

                var header = new Header
                {
                    alg = "HS256",
                    typ = "JWT",
                    cty = new Header.SubHeader
                    {
                        userId = user.UserId,
                        iv = iv
                    }
                };

                //使用 Base64 編碼 Header
                var headerJson = JsonConvert.SerializeObject(header);
                var headerBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(headerJson));

                var payload = new Payload
                {
                    userInfo = user,
                    exp = Convert.ToInt32((DateTime.Now.AddHours(3) - new DateTime(1970, 1, 1)).TotalSeconds)
                };

                //使用 Base64 編碼 Payload
                var payloadJson = JsonConvert.SerializeObject(payload);
                var payloadBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(payloadJson));

                //使用 AES 加密 Payload
                var payloadEncrypt = CryptoHelper.AESEncode(payloadBase64, user.ApiKey, iv, CipherMode.CBC, PaddingMode.PKCS7, 0, null);

                //取得雜湊簽章
                var signature = CryptoHelper.ComputeHMACSHA256(headerBase64 + "." + payloadEncrypt, user.ApiKey);

                return new TokenResponseData
                {
                    AccessToken = headerBase64 + "." + payloadEncrypt + "." + signature,
                    RefreshToken = Guid.NewGuid().ToString().Replace("-", "")
                };
            }

            /// <summary>
            /// 取得使用者資訊
            /// </summary>
            public User GetUser()
            {
                WebApiDBContext db = new WebApiDBContext();
                var authorizationHelper = HttpContext.Current.Request.Headers["Authorization"];

                var token = authorizationHelper.Split('.');
                if (token.Length < 3)
                    return null;

                var headerBase64 = token[0].Replace("Bearer ", "");
                var payloadEncrypt = token[1];
                var signature = token[2];

                //使用 Base64 反編碼取得 Header
                var headerJson = Encoding.UTF8.GetString(Convert.FromBase64String(headerBase64));
                var header = JsonConvert.DeserializeObject<Header>(headerJson);

                //取得使用者資料
                var user = db.Users.Where(x => x.UserId == header.cty.userId).FirstOrDefault();
                if (user == null)
                    return null;

                //檢查簽章是否正確
                if (signature != CryptoHelper.ComputeHMACSHA256(headerBase64 + "." + payloadEncrypt, user.ApiKey))
                    return null;

                //使用 AES 解密 Payload
                var payloadBase64 = CryptoHelper.AESDecode(payloadEncrypt, user.ApiKey, header.cty.iv, CipherMode.CBC, PaddingMode.PKCS7, 0, null);

                //使用 Base64 反編碼取得 Payload
                var payloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(payloadBase64));
                var payload = JsonConvert.DeserializeObject<Payload>(payloadJson);

                //檢查是否過期
                if (payload.exp < Convert.ToInt32((DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds))
                    return null;

                return payload.userInfo;
            }

            /// <summary>
            /// 承載令牌授權檢查
            /// </summary>
            public class BearerTokenAuthorizationFilterAttribute : AuthorizationFilterAttribute
            {
                public override void OnAuthorization(HttpActionContext actionContext)
                {
                    WebApiDBContext db = new WebApiDBContext();                    

                    if (actionContext.Request.Headers.Authorization == null || actionContext.Request.Headers.Authorization.Scheme != "Bearer")
                        setErrorResponse(actionContext, "驗證錯誤：授權資料錯誤");
                    else
                    {
                        try
                        {
                            var token = actionContext.Request.Headers.Authorization.Parameter.Split('.');
                            if (token.Length < 3)
                                setErrorResponse(actionContext, "驗證錯誤：授權資料格式錯誤");

                            var headerBase64 = token[0];
                            var payloadEncrypt = token[1];
                            var signature = token[2];

                            //使用 Base64 反編碼取得 Header
                            var headerJson = Encoding.UTF8.GetString(Convert.FromBase64String(headerBase64));
                            var header = JsonConvert.DeserializeObject<Header>(headerJson);

                            //取得使用者資料
                            var user = db.Users.Where(x => x.UserId == header.cty.userId).FirstOrDefault();
                            if (user == null)
                                setErrorResponse(actionContext, "驗證錯誤:使用者不存在");

                            //檢查簽章是否正確
                            if (signature != CryptoHelper.ComputeHMACSHA256(headerBase64 + "." + payloadEncrypt, user.ApiKey))
                                setErrorResponse(actionContext, "驗證錯誤:簽章不正確");

                            //使用 AES 解密 Payload
                            var payloadBase64 = CryptoHelper.AESDecode(payloadEncrypt, user.ApiKey, header.cty.iv, CipherMode.CBC, PaddingMode.PKCS7, 0, null);

                            //使用 Base64 反編碼取得 Payload
                            var payloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(payloadBase64));
                            var payload = JsonConvert.DeserializeObject<Payload>(payloadJson);

                            //檢查是否過期
                            if (payload.exp < Convert.ToInt32((DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds))
                                setErrorResponse(actionContext, "驗證錯誤:驗證已過期");
                        }
                        catch (Exception ex)
                        {
                            setErrorResponse(actionContext, ex.Message);
                        }
                    }

                    base.OnAuthorization(actionContext);
                }
            }
        }

        /// <summary>
        /// 設置錯誤響應
        /// </summary>
        private static void setErrorResponse(HttpActionContext actionContext, string message)
        {
            var response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, message);
            actionContext.Response = response;
        }
    }
}