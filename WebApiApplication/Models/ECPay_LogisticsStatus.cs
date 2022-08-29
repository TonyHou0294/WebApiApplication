using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApiApplication.Models
{
    /// <summary>
    /// 綠界物流狀態
    /// </summary>
    public class ECPay_LogisticsStatus
    {
        /// <summary>
        /// ID
        /// </summary>
        [DisplayName("ID")]
        [Key]
        [Required]
        public long ID { get; set; }

        /// <summary>
        /// 廠商編號
        /// </summary>
        [DisplayName("廠商編號")]
        [Required]
        [StringLength(10)]
        public string MerchantID { get; set; }

        /// <summary>
        /// 物流類型
        /// </summary>
        [DisplayName("物流類型")]
        [StringLength(20)]
        public string LogisticsType { get; set; }

        /// <summary>
        /// 物流子類型
        /// </summary>
        [DisplayName("物流子類型")]
        [StringLength(20)]
        public string LogisticsSubType { get; set; }

        /// <summary>
        /// 廠商交易編號
        /// </summary>
        [DisplayName("廠商交易編號")]
        [StringLength(20)]
        public string MerchantTradeNo { get; set; }

        /// <summary>
        /// 廠商逆交易編號
        /// </summary>
        [DisplayName("廠商逆交易編號")]
        [StringLength(20)]
        public string RtnMerchantTradeNo { get; set; }

        /// <summary>
        /// 綠界訂單編號
        /// </summary>
        [DisplayName("綠界訂單編號")]
        [StringLength(20)]
        public string LogisticsID { get; set; }

        /// <summary>
        /// 貨態代碼
        /// </summary>
        [DisplayName("貨態代碼")]
        [StringLength(8)]
        public string LogisticsStatus { get; set; }

        /// <summary>
        /// 貨態代碼訊息
        /// </summary>
        [DisplayName("貨態代碼訊息")]
        [StringLength(100)]
        public string LogisticsStatusName { get; set; }

        /// <summary>
        /// 商品金額
        /// </summary>
        [DisplayName("商品金額")]
        public int GoodsAmount { get; set; }

        /// <summary>
        /// 物流狀態更新時間
        /// </summary>
        [DisplayName("物流狀態更新時間")]
        [StringLength(20)]
        public string UpdateStatusDate { get; set; }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        [DisplayName("收件人姓名")]
        [StringLength(100)]
        public string ReceiverName { get; set; }

        /// <summary>
        /// 收件人電話
        /// </summary>
        [DisplayName("收件人電話")]
        [StringLength(20)]
        public string ReceiverPhone { get; set; }

        /// <summary>
        /// 收件人手機
        /// </summary>
        [DisplayName("收件人手機")]
        [StringLength(20)]
        public string ReceiverCellphone { get; set; }

        /// <summary>
        /// 收件人Email
        /// </summary>
        [DisplayName("收件人Email")]
        [StringLength(50)]
        public string ReceiverEmail { get; set; }

        /// <summary>
        /// 收件人地址
        /// </summary>
        [DisplayName("收件人地址")]
        [StringLength(200)]
        public string ReceiverAddress { get; set; }

        /// <summary>
        /// 寄貨編號
        /// </summary>
        [DisplayName("寄貨編號")]
        [StringLength(15)]
        public string CVSPaymentNo { get; set; }

        /// <summary>
        /// 驗證碼
        /// </summary>
        [DisplayName("驗證碼")]
        [StringLength(10)]
        public string CVSValidationNo { get; set; }

        /// <summary>
        /// 托運單號
        /// </summary>
        [DisplayName("托運單號")]
        [StringLength(50)]
        public string BookingNote { get; set; }
    }
}