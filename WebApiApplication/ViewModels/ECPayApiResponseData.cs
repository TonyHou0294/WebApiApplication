using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiApplication.ViewModels
{
    /// <summary>
    /// 綠界科技物流服務API回傳參數
    /// </summary>
    public class ECPayApiResponseData
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

    /// <summary>
    /// 綠界科技物流服務API回傳參數-基礎資料
    /// </summary>
    public class ECPayApiResponseData_BaseResponseData
    {
        /// <summary>
        /// 回應代碼
        /// </summary>
        public int RtnCode { get; set; }

        /// <summary>
        /// 回應訊息
        /// </summary>
        public string RtnMsg { get; set; }
    }

    /// <summary>
    /// 綠界科技物流服務API回傳參數-暫存頁面
    /// </summary>
    public class ECPayApiResponseData_TempPage
    {
        /// <summary>
        /// 臨時頁面網址
        /// </summary>
        public string TempPageUrl { get; set; }

        /// <summary>
        /// Html內容
        /// </summary>
        public string HtmlContent { get; set; }
    }

    /// <summary>
    /// 綠界科技物流服務API回傳參數-一段標測試資料產生(B2C)
    /// </summary>
    public class ECPayApiResponseData_CreateTestData : ECPayApiResponseData_BaseResponseData
    {
        /// <summary>
        /// 特店編號
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 廠商訂單編號
        /// </summary>
        public string MerchantTradeNo { get; set; }

        /// <summary>
        /// 綠界訂單編號
        /// </summary>
        public string LogisticsID { get; set; }

        /// <summary>
        /// 貨態代碼
        /// </summary>
        public string LogisticsStatus { get; set; }

        /// <summary>
        /// 貨態代碼訊息
        /// </summary>
        public string LogisticsStatusName { get; set; }

        /// <summary>
        /// 物流類型
        /// </summary>
        public string LogisticsType { get; set; }

        /// <summary>
        /// 物流子類型
        /// </summary>
        public string LogisticsSubType { get; set; }

        /// <summary>
        /// 商品金額
        /// </summary>
        public int GoodsAmount { get; set; }

        /// <summary>
        /// 物流狀態更新時間
        /// </summary>
        public string UpdateStatusDate { get; set; }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string ReceiverName { get; set; }

        /// <summary>
        /// 收件人電話
        /// </summary>
        public string ReciverPhone { get; set; }

        /// <summary>
        /// 收件人手機
        /// </summary>
        public string ReceiverCellPhone { get; set; }

        /// <summary>
        /// 收件人Email
        /// </summary>
        public string ReceiverEmail { get; set; }

        /// <summary>
        /// 收件人地址
        /// </summary>
        public string ReceiverAddress { get; set; }

        /// <summary>
        /// 托運單號
        /// </summary>
        public string BookingNote { get; set; }

        /// <summary>
        /// 寄貨編號
        /// </summary>
        public string CVSPaymentNo { get; set; }

        /// <summary>
        /// 驗證碼
        /// </summary>
        public string CVSValidationNo { get; set; }
    }

    /// <summary>
    /// 綠界科技物流服務API回傳參數-建立暫存物流訂單結果通知
    /// </summary>
    public class ECPayApiResponseData_CreateTempTradeResult : ECPayApiResponseData_BaseResponseData
    {
        /// <summary>
        /// 暫存物流訂單編號
        /// </summary>
        public string TempLogisticsID { get; set; }

        /// <summary>
        /// 物流類型
        /// </summary>
        public string LogisticsType { get; set; }

        /// <summary>
        /// 物流子類型
        /// </summary>
        public string LogisticsSubType { get; set; }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string ReceiverName { get; set; }

        /// <summary>
        /// 收件人電話
        /// </summary>
        public string ReceiverPhone { get; set; }

        /// <summary>
        /// 收件人手機
        /// </summary>
        public string ReceiverCellphone { get; set; }

        /// <summary>
        /// 收件人地址
        /// </summary>
        public string ReceiverAddress { get; set; }

        /// <summary>
        /// 收件人郵遞區號
        /// </summary>
        public string ReceiverZipCode { get; set; }

        /// <summary>
        /// 預定送達日期
        /// </summary>
        public string ScheduledDeliveryDate { get; set; }

        /// <summary>
        /// 預定送達時段
        /// </summary>
        public string ScheduledDeliveryTime { get; set; }

        /// <summary>
        /// 收件人門市代號
        /// </summary>
        public string ReceiverStoreID { get; set; }

        /// <summary>
        /// 收件門市名稱
        /// </summary>
        public string ReceiverStoreName { get; set; }
    }

    /// <summary>
    /// 綠界科技物流服務API回傳參數-建立正式物流訂單
    /// </summary>
    public class ECPayApiResponseData_CreateByTempTrade : ECPayApiResponseData_BaseResponseData
    {
        /// <summary>
        /// 綠界訂單編號
        /// </summary>
        public string LogisticsID { get; set; }
    }

    /// <summary>
    /// 綠界科技物流服務API回傳參數-逆物流訂單(B2C)
    /// </summary>
    public class ECPayApiResponseData_ReturnB2C : ECPayApiResponseData_BaseResponseData
    {
        /// <summary>
        /// 綠界科技逆物流交易編號
        /// </summary>
        public string RtnMerchantTradeNo { get; set; }

        /// <summary>
        /// 退貨編號
        /// </summary>
        public string RtnOrderNo { get; set; }
    }

    /// <summary>
    /// 綠界科技物流服務API回傳參數-查詢訂單
    /// </summary>
    public class ECPayApiResponseData_QueryLogisticsTradeInfo: ECPayApiResponseData_BaseResponseData
    {
        /// <summary>
        /// 廠商編號
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 廠商交易編號
        /// </summary>
        public string MerchantTradeNo { get; set; }

        /// <summary>
        /// 綠界訂單編號
        /// </summary>
        public string LogisticsID { get; set; }

        /// <summary>
        /// 商品金額
        /// </summary>
        public int GoodsAmount { get; set; }

        /// <summary>
        /// 會員選擇的物流方式
        /// </summary>
        public string LogisticsType { get; set; }

        /// <summary>
        /// 物流費用
        /// </summary>
        public int HandlingCharge { get; set; }

        /// <summary>
        /// 代收金額手續費
        /// </summary>
        public int CollectionChargeFee { get; set; }

        /// <summary>
        /// 訂單成立時間
        /// </summary>
        public string TradeDate { get; set; }

        /// <summary>
        /// 物流狀態
        /// </summary>
        public string LogisticsStatus { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 配送編號
        /// </summary>
        public string ShipmentNo { get; set; }

        /// <summary>
        /// 托運單號
        /// </summary>
        public string BookingNote { get; set; }
    }

    /// <summary>
    /// 綠界科技物流服務API回傳參數-物流狀態(貨態)通知
    /// </summary>
    public class ECPayApiResponseData_ForwardLogistics: ECPayApiResponseData_BaseResponseData
    {
        /// <summary>
        /// 廠商編號
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 物流類型
        /// </summary>
        public string LogisticsType { get; set; }

        /// <summary>
        /// 物流子類型
        /// </summary>
        public string LogisticsSubType { get; set; }

        /// <summary>
        /// 廠商交易編號
        /// </summary>
        public string MerchantTradeNo { get; set; }

        /// <summary>
        /// 綠界訂單編號
        /// </summary>
        public string LogisticsID { get; set; }

        /// <summary>
        /// 貨態代碼
        /// </summary>
        public string LogisticsStatus { get; set; }

        /// <summary>
        /// 貨態代碼訊息
        /// </summary>
        public string LogisticsStatusName { get; set; }

        /// <summary>
        /// 商品金額
        /// </summary>
        public int GoodsAmount { get; set; }

        /// <summary>
        /// 物流狀態更新時間
        /// </summary>
        public string UpdateStatusDate { get; set; }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string ReceiverName { get; set; }

        /// <summary>
        /// 收件人電話
        /// </summary>
        public string ReceiverPhone { get; set; }

        /// <summary>
        /// 收件人手機
        /// </summary>
        public string ReceiverCellphone { get; set; }

        /// <summary>
        /// 收件人Email
        /// </summary>
        public string ReceiverEmail { get; set; }

        /// <summary>
        /// 收件人地址
        /// </summary>
        public string ReceiverAddress { get; set; }

        /// <summary>
        /// 寄貨編號
        /// </summary>
        public string CVSPaymentNo { get; set; }

        /// <summary>
        /// 驗證碼
        /// </summary>
        public string CVSValidationNo { get; set; }

        /// <summary>
        /// 托運單號
        /// </summary>
        public string BookingNote { get; set; }
    }

    /// <summary>
    /// 綠界科技物流服務API回傳參數-物流狀態(逆物流)通知
    /// </summary>
    public class ECPayApiResponseData_ReverseLogistics: ECPayApiResponseData_BaseResponseData
    {
        /// <summary>
        /// 貨態代碼
        /// </summary>
        public string LogisticsStatus { get; set; }

        /// <summary>
        /// 貨態代碼訊息
        /// </summary>
        public string LogisticsStatusName { get; set; }

        /// <summary>
        /// 廠商編號
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 綠界科技逆物流交易編號
        /// </summary>
        public string RtnMerchantTradeNo { get; set; }

        /// <summary>
        /// 綠界訂單編號
        /// </summary>
        public string LogisticsID { get; set; }

        /// <summary>
        /// 商品金額
        /// </summary>
        public int GoodsAmount { get; set; }

        /// <summary>
        /// 物流狀態更新時間
        /// </summary>
        public string UpdateStatusDate { get; set; }

        /// <summary>
        /// 托運單號
        /// </summary>
        public string BookingNote { get; set; }
    }
}