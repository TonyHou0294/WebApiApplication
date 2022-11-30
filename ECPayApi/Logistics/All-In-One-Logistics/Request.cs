using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECPayApi.Logistics.All_In_One_Logistics
{
    /// <summary>
    /// 綠界科技-全方位物流傳入參數
    /// </summary>
    public class Request
    {
        /// <summary>
        /// 特約合作平台商代號
        /// </summary>
        public string PlatformID { get; set; }

        /// <summary>
        /// 特店編號
        /// </summary>
        [Required]
        public string MerchantID { get; set; }

        /// <summary>
        /// 傳入資料
        /// </summary>
        [Required]
        public RqHeaderData RqHeader { get; set; }

        /// <summary>
        /// 加密資料
        /// </summary>
        [Required]
        public string Data { get; set; }

        public class RqHeaderData
        {
            /// <summary>
            /// 傳入時間
            /// </summary>
            [Required]
            public string Timestamp { get; set; }

            /// <summary>
            /// 串接版號 
            /// </summary>
            [Required]
            public string Revision { get; set; }
        }

    }
}
