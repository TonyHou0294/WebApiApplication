using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApiApplication.ViewModels
{
    /// <summary>
    /// 綠界科技物流服務API傳入參數
    /// </summary>
    public class ECPayApiRequestData
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
        public RqHeaderData RqHeader { get; set; }

        /// <summary>
        /// 加密資料
        /// </summary>
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

    /// <summary>
    /// 綠界科技物流服務API傳入參數(ServerPost)
    /// </summary>
    public class ECPayApiRequestData_ServerPost: ECPayApiRequestData
    {
        /// <summary>
        /// 回傳代碼
        /// </summary>
        public int TransCode { get; set; }

        /// <summary>
        /// 回傳訊息
        /// </summary>
        public string TransMsg { get; set; }
    }
    /// <summary>
    /// 綠界科技物流服務API傳入參數-基礎資料
    /// </summary>
    public class ECPayApiRequestData_BaseRequestData
    {
        /// <summary>
        /// 特店編號
        /// </summary>
        [Required]
        public string MerchantID { get; set; }
    }

    /// <summary>
    /// 綠界科技物流服務API傳入參數-一段標測試資料產生(B2C)
    /// </summary>
    public class ECPayApiRequestData_CreateTestData : ECPayApiRequestData_BaseRequestData
    {
        /// <summary>
        /// 特約合作平台商代號
        /// </summary>
        public string PlatformID { get; set; }

        /// <summary>
        /// 物流子類型[FAMI：全家物流(B2C)/UNIMART：7-ELEVEN 超商物流(B2C)/UNIMARTFREEZE：7-ELEVEN 冷凍店取(B2C)]
        /// </summary>
        [Required]
        public string LogisticsSubType { get; set; }
    }

    /// <summary>
    /// 綠界科技物流服務API傳入參數-列印託運單
    /// </summary>
    public class ECPayApiRequestData_PrintTradeDocument : ECPayApiRequestData_BaseRequestData
    {
        /// <summary>
        /// 綠界訂單編號
        /// </summary>
        [Required]
        public string[] LogisticsID { get; set; }

        /// <summary>
        /// 物流子類型
        /// </summary>
        [Required]
        public string LogisticsSubType { get; set; }
    }


    /// <summary>
    /// 綠界科技物流服務API傳入參數-開啟物流選擇頁
    /// </summary>
    public class ECPayApiRequestData_RedirectToLogisticsSelection : ECPayApiRequestData_BaseRequestData
    {
        /// <summary>
        /// 暫存物流訂單編號
        /// </summary>
        [Required]
        public string TempLogisticsID { get; set; }

        /// <summary>
        /// 商品金額(商品金額範圍為 1~20,000 元。)
        /// </summary>
        [Required]
        public int GoodsAmount { get; set; }

        /// <summary>
        /// 是否代收貨款[N：不代收貨款，為預設值。/Y：代收貨款]
        /// </summary>
        public string IsCollection { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        [Required]
        public string GoodsName { get; set; }

        /// <summary>
        /// 寄件人姓名
        /// </summary>
        [Required]
        public string SenderName { get; set; }

        /// <summary>
        /// 寄件人郵遞區號
        /// </summary>
        [Required]
        public string SenderZipCode { get; set; }

        /// <summary>
        /// 寄件人地址
        /// </summary>
        [Required]
        public string SenderAddress { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// Server端回覆網址(無指定網址時使用預設網址)
        /// </summary>
        public string ServerReplyURL { get; set; }

        /// <summary>
        /// Client端回覆網址(無指定網址時使用預設網址)
        /// </summary>
        public string ClientReplyURL { get; set; }

        /// <summary>
        /// 溫層[0001:常溫(預設值)/0002:冷藏/0003:冷凍]
        /// </summary>
        public string Temperature { get; set; }

        /// <summary>
        /// 規格[0001: 60cm(預設值)/0002: 90cm/0003: 120cm/0004: 150cm]
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 預定取件時段[1: 9~12/2: 12~17/4: 不限時(固定填 4 不限時)]
        /// </summary>
        public string ScheduledPickupTime { get; set; }

        /// <summary>
        /// 包裹件數
        /// </summary>
        public int PackageCount { get; set; }

        /// <summary>
        /// 收件人地址
        /// </summary>
        public string ReceiverAddress { get; set; }

        /// <summary>
        /// 收件人手機
        /// </summary>
        public string ReceiverCellPhone { get; set; }

        /// <summary>
        /// 收件人電話
        /// </summary>
        public string ReceiverPhone { get; set; }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string ReceiverName { get; set; }

        /// <summary>
        /// 是否允許選擇送達時間[N：不允許(預設值)。/Y：允許。]
        /// </summary>
        public string EnableSelectDeliveryTime { get; set; }

        /// <summary>
        /// 廠商平台的會員ID
        /// </summary>
        public string EshopMemberID { get; set; }
    }

    /// <summary>
    /// 綠界科技物流服務API傳入參數-建立暫存物流訂單結果通知
    /// </summary>
    public class ECPayApiRequestData_CreateTempTradeResult
    {
        /// <summary>
        /// 回傳參數
        /// </summary>
        public string ResultData { get; set; }
    }

    /// <summary>
    /// 綠界科技物流服務API傳入參數-更新暫存物流訂單
    /// </summary>
    public class ECPayApiRequestData_UpdateTempTrade : ECPayApiRequestData_BaseRequestData
    {
        /// <summary>
        /// 暫存物流訂單編號
        /// </summary>
        [Required]
        public string TempLogisticsID { get; set; }

        /// <summary>
        /// 商品金額(商品金額範圍為 1~20,000 元。)
        /// </summary>
        public int GoodsAmount { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 寄件人姓名
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// 寄件人郵遞區號
        /// </summary>
        public string SenderZipCode { get; set; }

        /// <summary>
        /// 寄件人地址
        /// </summary>
        public string SenderAddress { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 退貨門市代號
        /// </summary>
        public string ReturnStoreID { get; set; }

        /// <summary>
        /// Server端回覆網址
        /// </summary>
        public string ServerReplyURL { get; set; }

        /// <summary>
        /// 規格[0001: 60cm(預設值)/0002: 90cm/0003: 120cm/0004: 150cm]
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 包裹件數
        /// </summary>
        public int PackageCount { get; set; }

        /// <summary>
        /// 收件人地址
        /// </summary>
        public string ReceiverAddress { get; set; }

        /// <summary>
        /// 收件人郵遞區號
        /// </summary>
        public string ReceiverZipCode { get; set; }

        /// <summary>
        /// 收件人手機
        /// </summary>
        public string ReceiverCellPhone { get; set; }

        /// <summary>
        /// 收件人電話
        /// </summary>
        public string ReceiverPhone { get; set; }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string ReceiverName { get; set; }
    }

    /// <summary>
    /// 綠界科技物流服務API傳入參數-建立正式物流訂單
    /// </summary>
    public class ECPayApiRequestData_CreateByTempTrade : ECPayApiRequestData_BaseRequestData
    {
        /// <summary>
        /// 暫存物流訂單編號
        /// </summary>
        [Required]
        public string TempLogisticsID { get; set; }

        /// <summary>
        /// 廠商交易編號
        /// </summary>
        public string MerchantTradeNo { get; set; }
    }

    /// <summary>
    /// 綠界科技物流服務API傳入參數-逆物流訂單(B2C)
    /// </summary>
    public class ECPayApiRequestData_ReturnB2C : ECPayApiRequestData_BaseRequestData
    {
        /// <summary>
        /// 綠界訂單編號
        /// </summary>
        public string LogisticsID { get; set; }

        /// <summary>
        /// Server端回覆網址(無指定網址時使用預設網址)
        /// </summary>
        public string ServerReplyURL { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品金額(商品金額範圍為 1~20,000 元。)
        /// </summary>
        [Required]
        public int GoodsAmount { get; set; }

        /// <summary>
        /// 服務型態代碼
        /// </summary>
        [Required]
        public string ServiceType { get; set; }

        /// <summary>
        /// 退貨人姓名
        /// </summary>
        [Required]
        public string SenderName { get; set; }

        /// <summary>
        /// 退貨人手機
        /// </summary>
        public string SenderPhone { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 綠界科技物流服務API傳入參數-逆物流訂單(宅配)
    /// </summary>
    public class ECPayApiRequestData_ReturnHome: ECPayApiRequestData_BaseRequestData
    {
        /// <summary>
        /// 綠界訂單編號
        /// </summary>
        public string LogisticsID { get; set; }

        /// <summary>
        /// 物流子類型
        /// </summary>
        public string LogisticsSubType { get; set; }

        /// <summary>
        /// Server端回覆網址(無指定網址時使用預設網址)
        /// </summary>
        public string ServerReplyURL { get; set; }

        /// <summary>
        /// 退貨人姓名
        /// </summary>
        [Required]
        public string SenderName { get; set; }

        /// <summary>
        /// 退貨人電話
        /// </summary>
        public string SenderPhone { get; set; }

        /// <summary>
        /// 退貨人手機
        /// </summary>
        public string SenderCellPhone { get; set; }

        /// <summary>
        /// 退貨人郵遞區號
        /// </summary>
        public string SenderZipCode { get; set; }

        /// <summary>
        /// 退貨人地址
        /// </summary>
        public string SenderAddress { get; set; }

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
        public string ReceiverCellPhone { get; set; }

        /// <summary>
        /// 收件人郵遞區號
        /// </summary>
        public string ReceiverZipCode { get; set; }

        /// <summary>
        /// 收件人地址
        /// </summary>
        public string ReceiverAddress { get; set; }

        /// <summary>
        /// 收件人Email
        /// </summary>
        public string ReceiverEmail { get; set; }

        /// <summary>
        /// 商品金額
        /// </summary>
        public int GoodsAmount { get; set; }

        /// <summary>
        /// 物品名稱
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 溫層[0001:常溫(預設值)/0002:冷藏/0003:冷凍]
        /// </summary>
        public string Temperature { get; set; }

        /// <summary>
        /// 距離[00:同縣市(預設值)/01:外縣市/02:離島]
        /// </summary>
        public string Distance { get; set; }

        /// <summary>
        /// 規格[0001: 60cm(預設值)/0002: 90cm/0003: 120cm/0004: 150cm]
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 預定送達時段[1:13 前/2:14~18/4:不限時]
        /// </summary>
        public string ScheduledDeliveryTime { get; set; }

        /// <summary>
        /// 指定送達日
        /// </summary>
        public string ScheduledDeliveryDate { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 特約合作平台商代號
        /// </summary>
        public string PlatformID { get; set; }
    }

    /// <summary>
    /// 綠界科技物流服務API傳入參數-查詢訂單
    /// </summary>
    public class ECPayApiRequestData_QueryLogisticsTradeInfo: ECPayApiRequestData_BaseRequestData
    {
        /// <summary>
        /// 綠界訂單編號
        /// </summary>
        public string LogisticsID { get; set; }

        /// <summary>
        /// 廠商交易編號
        /// </summary>
        public string MerchantTradeNo { get; set; }
    }

    /// <summary>
    /// 綠界科技物流服務API傳入參數-(B2C)物流訂單異動
    /// </summary>
    public class ECPayApiRequestData_UpdateShipmentInfo: ECPayApiRequestData_BaseRequestData
    {
        /// <summary>
        /// 綠界訂單編號
        /// </summary>
        public string LogisticsID { get; set; }

        /// <summary>
        /// 物流訂單出貨日期
        /// </summary>
        public string ShipmentDate { get; set; }

        /// <summary>
        /// 物流訂單取貨門市
        /// </summary>
        public string ReceiverStoreID { get; set; }
    }

    /// <summary>
    /// 綠界科技物流服務API傳入參數-(C2C)物流訂單異動
    /// </summary>
    public class ECPayApiRequestData_UpdateStoreInfo: ECPayApiRequestData_BaseRequestData
    {
        /// <summary>
        /// 綠界訂單編號
        /// </summary>
        public string LogisticsID { get; set; }

        /// <summary>
        /// 寄貨編號
        /// </summary>
        public string CVSPaymentNo { get; set; }

        /// <summary>
        /// 驗證碼
        /// </summary>
        public string CVSValidationNo { get; set; }

        /// <summary>
        /// 門市類型[01：取件門市更新/02：退件門市更新]
        /// </summary>
        public string StoreType { get; set; }

        /// <summary>
        /// 物流訂單取貨門市
        /// </summary>
        public string ReceiverStoreID { get; set; }

        /// <summary>
        /// 物流訂單退貨門市
        /// </summary>
        public string ReturnStoreID { get; set; }
    }

    /// <summary>
    /// 綠界科技物流服務API傳入參數-取消訂單(7-EVEVEN超商C2C)
    /// </summary>
    public class ECPayApiRequestData_CancelC2COrder : ECPayApiRequestData_BaseRequestData 
    {
        /// <summary>
        /// 綠界訂單編號
        /// </summary>
        public string LogisticsID { get; set; }

        /// <summary>
        /// 寄貨編號
        /// </summary>
        public string CVSPaymentNo { get; set; }

        /// <summary>
        /// 驗證碼
        /// </summary>
        public string CVSValidationNo { get; set; }
    }
}