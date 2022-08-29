using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApiApplication.Models
{
    /// <summary>
    /// 綠界暫存物流訂單結果
    /// </summary>
    public class ECPay_TempTradeResult
    {
        /// <summary>
        /// ID
        /// </summary>
        [DisplayName("ID")]
        [Key]
        [Required]
        public long ID { get; set; }

        /// <summary>
        /// 會員編號
        /// </summary>
        [DisplayName("會員編號")]
        [Required]
        [StringLength(50)]
        public string MerchantID { get; set; }

        /// <summary>
        /// 綠界暫存物流訂單編號
        /// </summary>
        [DisplayName("綠界暫存物流訂單編號")]
        [StringLength(20)]
        public string TempLogisticsID { get; set; }

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
        /// 收件人姓名
        /// </summary>
        [DisplayName("收件人姓名")]
        [StringLength(10)]
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
        /// 收件人地址
        /// </summary>
        [DisplayName("收件人地址")]
        [StringLength(60)]
        public string ReceiverAddress { get; set; }

        /// <summary>
        /// 收件人郵遞區號
        /// </summary>
        [DisplayName("收件人郵遞區號")]
        [StringLength(5)]
        public string ReceiverZipCode { get; set; }

        /// <summary>
        /// 預定送達日期
        /// </summary>
        [DisplayName("預定送達日期")]
        [StringLength(10)]
        public string ScheduledDeliveryDate { get; set; }

        /// <summary>
        /// 預定送達時段
        /// </summary>
        [DisplayName("預定送達時段")]
        [StringLength(2)]
        public string ScheduledDeliveryTime { get; set; }

        /// <summary>
        /// 收件人門市代號
        /// </summary>
        [DisplayName("收件人門市代號")]
        [StringLength(6)]
        public string ReceiverStoreID { get; set; }

        /// <summary>
        /// 收件門市名稱
        /// </summary>
        [DisplayName("收件門市名稱")]
        [StringLength(10)]
        public string ReceiverStoreName { get; set; }

        /// <summary>
        /// 廠商交易編號
        /// </summary>
        [DisplayName("廠商交易編號")]
        [StringLength(20)]
        public string MerchantTradeNo { get; set; }

        /// <summary>
        /// 綠界訂單編號
        /// </summary>
        [DisplayName("綠界訂單編號")]
        [StringLength(20)]
        public string LogisticsID { get; set; }

        /// <summary>
        /// 綠界科技逆物流交易編號
        /// </summary>
        [DisplayName("綠界科技逆物流交易編號")]
        [StringLength(20)]
        public string RtnMerchantTradeNo { get; set; }

        /// <summary>
        /// 退貨編號
        /// </summary>
        [DisplayName("退貨編號")]
        [StringLength(20)]
        public string RtnOrderNo { get; set; }
    }
}