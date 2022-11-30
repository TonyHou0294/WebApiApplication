using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace WebApiApplication.ViewModels.Response
{
    /// <summary>
    /// API呼叫結果
    /// </summary>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Http狀態碼
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// 訊息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 資料
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 總頁數
        /// </summary>
        public int TotalPage { get; set; }
    }
}