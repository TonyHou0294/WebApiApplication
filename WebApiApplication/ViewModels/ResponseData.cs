using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiApplication.ViewModels
{
    /// <summary>
    /// 令牌響應資料
    /// </summary>
    public class TokenResponseData
    {
        /// <summary>
        /// 訪問令牌
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string RefreshToken { get; set; }
    }
}