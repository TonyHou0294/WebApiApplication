using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECPayApi.Logistics.All_In_One_Logistics
{
    /// <summary>
    /// 綠界科技-全方位物流回傳參數
    /// </summary>
    public class Response
    {
        /// <summary>
        /// 特約合作平台商代號
        /// </summary>
        public string PlatformID { get; set; }

        /// <summary>
        /// 特店編號
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 回傳資料
        /// </summary>
        public RpHeaderData RpHeader { get; set; }

        /// <summary>
        /// 回傳代碼
        /// </summary>
        public int TransCode { get; set; }

        /// <summary>
        /// 回傳訊息
        /// </summary>
        public string TransMsg { get; set; }

        /// <summary>
        /// 加密資料
        /// </summary>
        public string Data { get; set; }

        public class RpHeaderData
        {
            /// <summary>
            /// 回傳時間 
            /// </summary>
            public string Timestamp { get; set; }
        }
    }
}
