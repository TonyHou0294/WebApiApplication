using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using WebApiApplication.Models;
using WebApiApplication.ViewModels;

namespace WebApiApplication.Controllers
{
    /// <summary>
    /// 綠界科技物流服務API
    /// </summary>
    [RoutePrefix("api/ECPayApi")]
    public class ECPayApiController : BaseApiController
    {
        #region 共用參數
        ECPayApiRequestData _RequestData;
        string encodeJsonString, _CipherText, _ClearText, _MerchantID, _HashKey, _HashIV;
        #endregion

        public ECPayApiController()
        {
            _RequestData = new ECPayApiRequestData();
        }

        /// <summary>
        /// 設定ECPay資料
        /// </summary>
        /// <param name="MerchantID">會員編號</param>
        /// <returns></returns>
        private bool _SetECPay(string MerchantID)
        {
            var ECPayData = db.ECPays.Where(x => x.MerchantID == MerchantID).FirstOrDefault();
            if (ECPayData != null)
            {
                _MerchantID = ECPayData.MerchantID;
                _HashKey = ECPayData.HashKey;
                _HashIV = ECPayData.HashIV;

                if (ECPayData.IsStage)
                    host = ConfigurationManager.AppSettings["ECPayApiStageHost_URL"];
                else
                    host = ConfigurationManager.AppSettings["ECPayApiHost_URL"];

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 綠界科技物流服務API加密
        /// </summary>
        /// <param name="ClearText">明文</param>
        /// <param name="HashKey">雜湊金鑰</param>
        /// <param name="HashIV">雜湊初始化向量</param>
        /// <returns>密文</returns>
        private string ECPayApiEncode(string ClearText, string HashKey, string HashIV)
        {
            string ECPayEncodeStr = string.Empty;
            var UrlEncodeStr = CryptoHelper.UrlEncode(ClearText, Encoding.UTF8);
            ECPayEncodeStr = CryptoHelper.AESEncode(UrlEncodeStr, HashKey, HashIV, CipherMode.CBC, PaddingMode.PKCS7, 0, 128);
            return ECPayEncodeStr;
        }

        /// <summary>
        /// 綠界科技物流服務API解密
        /// </summary>
        /// <param name="CipherText">密文</param>
        /// <param name="HashKey">雜湊金鑰</param>
        /// <param name="HashIV">雜湊初始化向量</param>
        /// <returns>密文</returns>
        public static string ECPayApiDecode(string CipherText, string HashKey, string HashIV)
        {
            string ECPayDecodeStr = string.Empty;
            var AESDecodeStr = CryptoHelper.AESDecode(CipherText, HashKey, HashIV, CipherMode.CBC, PaddingMode.PKCS7, 0, 128);
            ECPayDecodeStr = CryptoHelper.UrlDecode(AESDecodeStr, Encoding.UTF8);
            return ECPayDecodeStr;
        }

        /// <summary>
        /// 產生綠界科技暫存頁
        /// </summary>
        /// <param name="HtmlContent">Html內容</param>
        /// <param name="FileName">檔名</param>
        /// <returns>物流選擇頁</returns>
        public static string CreateECPayTempPage(string HtmlContent, string FileName)
        {
            string fileName = FileName + ".html";
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/ECPayApiTempPage");

            if (!Directory.Exists(fileSaveLocation))
            {
                Directory.CreateDirectory(fileSaveLocation);
            }

            string path = fileSaveLocation + "\\" + fileName;

            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                try
                {
                    StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                    sw.WriteLine(HtmlContent);
                    sw.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return fileName;
        }


        /// <summary>
        /// 設定綠界科技物流服務API傳入參數
        /// </summary>
        /// <param name="CipherText">加密資料</param>
        private ECPayApiRequestData SetECPayApiRequestData(string CipherText)
        {
            ECPayApiRequestData data = new ECPayApiRequestData
            {
                MerchantID = _MerchantID,
                RqHeader = new ECPayApiRequestData.RqHeaderData
                {
                    Timestamp = timestamp.ToString(),
                    Revision = "1.0.0"
                },
                Data = CipherText
            };

            return data;
        }

        /// <summary>
        /// 設定綠界科技物流服務API回傳參數
        /// </summary>
        /// <param name="CipherText">加密資料</param>
        private ECPayApiResponseData SetECPayApiResponseData(string CipherText)
        {
            ECPayApiResponseData data = new ECPayApiResponseData
            {
                MerchantID = _MerchantID,
                RpHeader = new ECPayApiResponseData.RpHeaderData
                {
                    Timestamp = timestamp.ToString()
                },
                TransCode = 1,
                TransMsg = "",
                Data = CipherText
            };

            return data;
        }

        /// <summary>
        /// 一段標測試資料產生(B2C)
        /// </summary>
        /// <param name="data">一段標測試資料產生(B2C)傳入參數</param>
        /// <returns>一段標測試資料</returns>
        [HttpPost]
        [Route("CreateTestData")]
        [ResponseType(typeof(ECPayApiResponseData_CreateTestData))]
        public IHttpActionResult CreateTestData(ECPayApiRequestData_CreateTestData data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _SetECPay(data.MerchantID);

                path = ConfigurationManager.AppSettings["ECPayApiPath_CreateTestData"];

                using (HttpClient client = new HttpClient())
                {
                    //1.設定API Url
                    url = host + path;

                    //2.設定加密資料
                    var encodeData = new ECPayApiRequestData_CreateTestData
                    {
                        MerchantID = _MerchantID,
                        LogisticsSubType = data.LogisticsSubType
                    };

                    encodeJsonString = JsonConvert.SerializeObject(encodeData);
                    _CipherText = ECPayApiEncode(encodeJsonString, _HashKey, _HashIV);

                    //3.設定綠界科技請求參數
                    _RequestData = SetECPayApiRequestData(_CipherText);

                    //4.將Json物件轉換成字串
                    jsonString = JsonConvert.SerializeObject(_RequestData);

                    //5.設定Request格式
                    requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                    //6.設定請求時間
                    client.Timeout = TimeSpan.FromSeconds(900000);

                    //7.放入Request參數
                    requestMessage.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    //8.發出API請求
                    responseMessage = client.SendAsync(requestMessage).GetAwaiter().GetResult();
                    responseResult = responseMessage.Content.ReadAsStringAsync().Result.ToString();
                    var result = JsonConvert.DeserializeObject<ECPayApiResponseData>(responseResult);

                    if (responseMessage.StatusCode.ToString() == "OK")
                    {
                        if (result.TransCode == 1)
                        {
                            //解密資料
                            _ClearText = ECPayApiDecode(result.Data, _HashKey, _HashIV);
                            var resultData = JsonConvert.DeserializeObject<ECPayApiResponseData_CreateTestData>(_ClearText);

                            if (resultData.RtnCode == 1)
                                return Ok(resultData);
                            else
                                return Content(HttpStatusCode.BadGateway, resultData.RtnMsg);
                        }
                        else
                            return Content(HttpStatusCode.BadGateway, result.TransMsg);
                    }
                    else
                        return Content(HttpStatusCode.NotImplemented, "綠界科技物流服務API呼叫失敗-一段標測試資料產生(B2C)");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// 列印託運單
        /// </summary>
        /// <param name="data">列印託運單傳入參數</param>
        /// <returns>列印託運單頁</returns>
        [HttpPost]
        [Route("PrintTradeDocument")]
        public IHttpActionResult PrintTradeDocument(ECPayApiRequestData_PrintTradeDocument data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _SetECPay(data.MerchantID);

                path = ConfigurationManager.AppSettings["ECPayApiPath_PrintTradeDocument"];

                using (HttpClient client = new HttpClient())
                {
                    //1.設定API Url
                    url = host + path;

                    //2.設定加密資料
                    var encodeData = new ECPayApiRequestData_PrintTradeDocument
                    {
                        MerchantID = _MerchantID,
                        LogisticsID = data.LogisticsID,
                        LogisticsSubType = data.LogisticsSubType
                    };

                    encodeJsonString = JsonConvert.SerializeObject(encodeData);
                    _CipherText = ECPayApiEncode(encodeJsonString, _HashKey, _HashIV);

                    //3.設定綠界科技請求參數
                    _RequestData = SetECPayApiRequestData(_CipherText);

                    //4.將Json物件轉換成字串
                    jsonString = JsonConvert.SerializeObject(_RequestData);

                    //5.設定Request格式
                    requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                    //6.設定請求時間
                    client.Timeout = TimeSpan.FromSeconds(900000);

                    //7.放入Request參數
                    requestMessage.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    //8.發出API請求
                    responseMessage = client.SendAsync(requestMessage).GetAwaiter().GetResult();
                    responseResult = responseMessage.Content.ReadAsStringAsync().Result.ToString();

                    if (responseMessage.StatusCode.ToString() == "OK")
                    {
                        string PageFile = CreateECPayTempPage(responseResult, "PrintTradeDocument");
                        string PageUrl = ConfigurationManager.AppSettings["LocalApiHost_URL"] + "/ECPayApiTempPage/" + PageFile;

                        var tempPage = new ECPayApiResponseData_TempPage
                        {
                            TempPageUrl = PageUrl,
                            HtmlContent = responseResult.Replace("\r\n", "")
                        };

                        return Ok(tempPage);
                    }
                    else
                        return Content(HttpStatusCode.NotImplemented, "綠界科技物流服務API呼叫失敗-列印託運單");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// 開啟物流選擇頁
        /// </summary>
        /// <param name="data">開啟物流選擇頁傳入參數</param>
        /// <returns>物流選擇頁或執行錯誤訊息</returns>
        [HttpPost]
        [Route("RedirectToLogisticsSelection")]
        [ResponseType(typeof(ECPayApiResponseData_TempPage))]
        public IHttpActionResult RedirectToLogisticsSelection([FromBody] ECPayApiRequestData_RedirectToLogisticsSelection data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _SetECPay(data.MerchantID);

                path = ConfigurationManager.AppSettings["ECPayApiPath_RedirectToLogisticsSelection"];

                using (HttpClient client = new HttpClient())
                {
                    //1.設定API Url
                    url = host + path;

                    //2.設定加密資料
                    string ServerReplyURL = string.IsNullOrWhiteSpace(data.ServerReplyURL) ?
                        ConfigurationManager.AppSettings["LocalApiHost_URL"] + ConfigurationManager.AppSettings["LocalApiPath_ForwardServerReplyURL"] :
                        data.ServerReplyURL;
                    string ClientReplyURL = string.IsNullOrWhiteSpace(data.ClientReplyURL) ?
                        ConfigurationManager.AppSettings["LocalApiHost_URL"] + ConfigurationManager.AppSettings["LocalApiPath_ClientReplyURL"] + "?MerchantID=" + data.MerchantID :
                        data.ClientReplyURL;

                    var encodeData = new ECPayApiRequestData_RedirectToLogisticsSelection
                    {
                        MerchantID = data.MerchantID,
                        TempLogisticsID = data.TempLogisticsID,
                        GoodsAmount = data.GoodsAmount,
                        IsCollection = data.IsCollection,
                        GoodsName = data.GoodsName,
                        SenderName = data.SenderName,
                        SenderZipCode = data.SenderZipCode,
                        SenderAddress = data.SenderAddress,
                        Remark = data.Remark,
                        ServerReplyURL = ServerReplyURL,
                        ClientReplyURL = ClientReplyURL,
                        Temperature = data.Temperature,
                        Specification = data.Specification,
                        ScheduledPickupTime = data.ScheduledPickupTime,
                        PackageCount = data.PackageCount,
                        ReceiverAddress = data.ReceiverAddress,
                        ReceiverCellPhone = data.ReceiverCellPhone,
                        ReceiverPhone = data.ReceiverPhone,
                        ReceiverName = data.ReceiverName,
                        EnableSelectDeliveryTime = data.EnableSelectDeliveryTime,
                        EshopMemberID = data.EshopMemberID
                    };

                    encodeJsonString = JsonConvert.SerializeObject(encodeData);
                    _CipherText = ECPayApiEncode(encodeJsonString, _HashKey, _HashIV);

                    //3.設定綠界科技請求參數
                    _RequestData = SetECPayApiRequestData(_CipherText);

                    //4.將Json物件轉換成字串
                    jsonString = JsonConvert.SerializeObject(_RequestData);

                    //5.設定Request格式
                    requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                    //6.設定請求時間
                    client.Timeout = TimeSpan.FromSeconds(900000);

                    //7.放入Request參數
                    requestMessage.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    //8.發出API請求
                    responseMessage = client.SendAsync(requestMessage).GetAwaiter().GetResult();
                    responseResult = responseMessage.Content.ReadAsStringAsync().Result.ToString();

                    if (responseMessage.StatusCode.ToString() == "OK")
                    {
                        if (responseResult.IndexOf("<!DOCTYPE html>") >= 0)
                        {
                            string PageFile = CreateECPayTempPage(responseResult, "RedirectToLogisticsSelection");
                            string PageUrl = ConfigurationManager.AppSettings["LocalApiHost_URL"] + "/ECPayApiTempPage/" + PageFile;

                            var tempPage = new ECPayApiResponseData_TempPage
                            {
                                TempPageUrl = PageUrl,
                                HtmlContent = responseResult.Replace("\r\n", "")
                            };

                            return Ok(tempPage);
                        }
                        else
                        {
                            var result = JsonConvert.DeserializeObject<ECPayApiResponseData>(responseResult);
                            if (result.TransCode == 1)
                            {
                                //解密資料
                                _ClearText = ECPayApiDecode(result.Data, _HashKey, _HashIV);
                                var resultData = JsonConvert.DeserializeObject<ECPayApiResponseData_CreateByTempTrade>(_ClearText);

                                return Content(HttpStatusCode.BadGateway, resultData.RtnMsg);
                            }
                            return Content(HttpStatusCode.BadGateway, result.TransMsg);

                        }
                    }
                    else
                        return Content(HttpStatusCode.NotImplemented, "綠界科技物流服務API呼叫失敗-開啟物流選擇頁");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// 取得綠界暫存物流訂單結果列表
        /// </summary>
        /// <param name="data">取得綠界暫存物流訂單結果列表傳入參數</param>
        /// <returns>綠界暫存物流訂單結果列表</returns>
        [HttpPost]
        [Route("GetECPayTempTradeResult")]
        public IQueryable<ECPay_TempTradeResult> GetECPayTempTradeResult([FromBody] ECPayApiRequestData_BaseRequestData data)
        {
            return db.ECPay_TempTradeResults.Where(x => x.MerchantID == data.MerchantID);
        }

        /// <summary>
        /// 建立暫存物流訂單結果
        /// </summary>
        /// <param name="MerchantID">特店編號</param>
        /// <param name="data">建立暫存物流訂單結果通知傳入參數</param>
        /// <returns>暫存物流訂單結果</returns>
        [HttpPost]
        [Route("CreateTempTradeResult")]
        public IHttpActionResult CreateTempTradeResult([FromUri] string MerchantID, [FromBody] ECPayApiRequestData_CreateTempTradeResult data)
        {
            try
            {
                _SetECPay(MerchantID);

                var result = JsonConvert.DeserializeObject<ECPayApiResponseData>(data.ResultData);

                if (result.TransCode == 1)
                {
                    //解密資料
                    _ClearText = ECPayApiDecode(result.Data, _HashKey, _HashIV);
                    var resultData = JsonConvert.DeserializeObject<ECPayApiResponseData_CreateTempTradeResult>(_ClearText);

                    if (resultData.RtnCode == 1)
                    {
                        var ECPay_TempLogisticsResultData = db.ECPay_TempTradeResults
                            .Where(x => x.MerchantID == MerchantID && x.TempLogisticsID == resultData.TempLogisticsID)
                            .FirstOrDefault();

                        if (ECPay_TempLogisticsResultData == null)
                        {
                            //新增暫存物流訂單
                            var newECPay_TempTradeResult = new ECPay_TempTradeResult
                            {
                                MerchantID = MerchantID,
                                TempLogisticsID = resultData.TempLogisticsID,
                                LogisticsType = resultData.LogisticsType,
                                LogisticsSubType = resultData.LogisticsSubType,
                                ReceiverName = resultData.ReceiverName,
                                ReceiverPhone = resultData.ReceiverPhone,
                                ReceiverCellphone = resultData.ReceiverCellphone,
                                ReceiverAddress = resultData.ReceiverAddress,
                                ReceiverZipCode = resultData.ReceiverZipCode,
                                ScheduledDeliveryDate = resultData.ScheduledDeliveryDate,
                                ScheduledDeliveryTime = resultData.ScheduledDeliveryTime,
                                ReceiverStoreID = resultData.ReceiverStoreID,
                                ReceiverStoreName = resultData.ReceiverStoreName
                            };

                            db.ECPay_TempTradeResults.Add(newECPay_TempTradeResult);
                        }
                        else
                        {
                            //修改暫存物流訂單
                            ECPay_TempLogisticsResultData.LogisticsType = resultData.LogisticsType;
                            ECPay_TempLogisticsResultData.LogisticsSubType = resultData.LogisticsSubType;
                            ECPay_TempLogisticsResultData.ReceiverName = resultData.ReceiverName;
                            ECPay_TempLogisticsResultData.ReceiverPhone = resultData.ReceiverPhone;
                            ECPay_TempLogisticsResultData.ReceiverCellphone = resultData.ReceiverCellphone;
                            ECPay_TempLogisticsResultData.ReceiverAddress = resultData.ReceiverAddress;
                            ECPay_TempLogisticsResultData.ReceiverZipCode = resultData.ReceiverZipCode;
                            ECPay_TempLogisticsResultData.ScheduledDeliveryDate = resultData.ScheduledDeliveryDate;
                            ECPay_TempLogisticsResultData.ScheduledDeliveryTime = resultData.ScheduledDeliveryTime;
                            ECPay_TempLogisticsResultData.ReceiverStoreID = resultData.ReceiverStoreID;
                            ECPay_TempLogisticsResultData.ReceiverStoreName = resultData.ReceiverStoreName;

                            db.Entry(ECPay_TempLogisticsResultData).State = EntityState.Modified;
                        }

                        db.SaveChanges();

                        return Ok(resultData);
                    }
                    else
                        return Content(HttpStatusCode.BadGateway, resultData.RtnMsg);
                }
                else
                    return Content(HttpStatusCode.BadGateway, result.TransMsg);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// 更新暫存物流訂單
        /// </summary>
        /// <param name="data">更新暫存物流訂單傳入參數</param>
        /// <returns>執行結果</returns>
        [HttpPost]
        [Route("UpdateTempTrade")]
        [ResponseType(typeof(ECPayApiResponseData_BaseResponseData))]
        public IHttpActionResult UpdateTempTrade([FromBody] ECPayApiRequestData_UpdateTempTrade data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _SetECPay(data.MerchantID);

                path = ConfigurationManager.AppSettings["ECPayApiPath_UpdateTempTrade"];

                using (HttpClient client = new HttpClient())
                {
                    //1.設定API Url
                    url = host + path;

                    //2.設定加密資料
                    string ServerReplyURL = string.IsNullOrWhiteSpace(data.ServerReplyURL) ?
                        ConfigurationManager.AppSettings["LocalApiHost_URL"] + ConfigurationManager.AppSettings["LocalApiPath_ForwardServerReplyURL"] :
                        data.ServerReplyURL;

                    var encodeData = new ECPayApiRequestData_UpdateTempTrade
                    {
                        MerchantID = data.MerchantID,
                        TempLogisticsID = data.TempLogisticsID,
                        GoodsAmount = data.GoodsAmount,
                        GoodsName = data.GoodsName,
                        SenderName = data.SenderName,
                        SenderZipCode = data.SenderZipCode,
                        SenderAddress = data.SenderAddress,
                        Remark = data.Remark,
                        ReturnStoreID = data.ReturnStoreID,
                        Specification = data.Specification,
                        PackageCount = data.PackageCount,
                        ReceiverAddress = data.ReceiverAddress,
                        ReceiverZipCode = data.ReceiverZipCode,
                        ReceiverCellPhone = data.ReceiverCellPhone,
                        ReceiverPhone = data.ReceiverPhone,
                        ReceiverName = data.ReceiverName
                    };

                    encodeJsonString = JsonConvert.SerializeObject(encodeData);
                    _CipherText = ECPayApiEncode(encodeJsonString, _HashKey, _HashIV);

                    //3.設定綠界科技請求參數
                    _RequestData = SetECPayApiRequestData(_CipherText);

                    //4.將Json物件轉換成字串
                    jsonString = JsonConvert.SerializeObject(_RequestData);

                    //5.設定Request格式
                    requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                    //6.設定請求時間
                    client.Timeout = TimeSpan.FromSeconds(900000);

                    //7.放入Request參數
                    requestMessage.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    //8.發出API請求
                    responseMessage = client.SendAsync(requestMessage).GetAwaiter().GetResult();
                    responseResult = responseMessage.Content.ReadAsStringAsync().Result.ToString();
                    var result = JsonConvert.DeserializeObject<ECPayApiResponseData>(responseResult);

                    if (responseMessage.StatusCode.ToString() == "OK")
                    {
                        if (result.TransCode == 1)
                        {
                            //解密資料
                            _ClearText = ECPayApiDecode(result.Data, _HashKey, _HashIV);
                            var resultData = JsonConvert.DeserializeObject<ECPayApiResponseData_BaseResponseData>(_ClearText);

                            if (resultData.RtnCode == 1)
                                return Ok(resultData);
                            else
                                return Content(HttpStatusCode.BadGateway, resultData.RtnMsg);
                        }
                        else
                            return Content(HttpStatusCode.BadGateway, result.TransMsg);
                    }
                    else
                        return Content(HttpStatusCode.NotImplemented, "綠界科技物流服務API呼叫失敗-更新暫存物流訂單");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// 建立正式物流訂單
        /// </summary>
        /// <param name="data">建立正式物流訂單傳入參數</param>
        /// <returns>正式物流訂單Id</returns>
        [HttpPost]
        [Route("CreateByTempTrade")]
        [ResponseType(typeof(ECPayApiResponseData_CreateByTempTrade))]
        public IHttpActionResult CreateByTempTrade([FromBody] ECPayApiRequestData_CreateByTempTrade data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _SetECPay(data.MerchantID);

                path = ConfigurationManager.AppSettings["ECPayApiPath_CreateByTempTrade"];

                using (HttpClient client = new HttpClient())
                {
                    //1.設定API Url
                    url = host + path;

                    //2.設定加密資料
                    var encodeData = new ECPayApiRequestData_CreateByTempTrade
                    {
                        MerchantID = _MerchantID,
                        TempLogisticsID = data.TempLogisticsID,
                        MerchantTradeNo = data.MerchantTradeNo
                    };

                    encodeJsonString = JsonConvert.SerializeObject(encodeData);
                    _CipherText = ECPayApiEncode(encodeJsonString, _HashKey, _HashIV);

                    //3.設定綠界科技請求參數
                    _RequestData = SetECPayApiRequestData(_CipherText);

                    //4.將Json物件轉換成字串
                    jsonString = JsonConvert.SerializeObject(_RequestData);

                    //5.設定Request格式
                    requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                    //6.設定請求時間
                    client.Timeout = TimeSpan.FromSeconds(900000);

                    //7.放入Request參數
                    requestMessage.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    //8.發出API請求
                    responseMessage = client.SendAsync(requestMessage).GetAwaiter().GetResult();
                    responseResult = responseMessage.Content.ReadAsStringAsync().Result.ToString();
                    var result = JsonConvert.DeserializeObject<ECPayApiResponseData>(responseResult);

                    if (responseMessage.StatusCode.ToString() == "OK")
                    {
                        if (result.TransCode == 1)
                        {
                            //解密資料
                            _ClearText = ECPayApiDecode(result.Data, _HashKey, _HashIV);
                            var resultData = JsonConvert.DeserializeObject<ECPayApiResponseData_CreateByTempTrade>(_ClearText);

                            if (resultData.RtnCode == 1)
                            {
                                //更新綠界暫存物流訂單結果
                                var ECPay_TempLogisticsResultData = db.ECPay_TempTradeResults
                                    .Where(x => x.MerchantID == data.MerchantID && x.TempLogisticsID == data.TempLogisticsID)
                                    .FirstOrDefault();

                                if (ECPay_TempLogisticsResultData != null)
                                {
                                    ECPay_TempLogisticsResultData.MerchantTradeNo = data.MerchantTradeNo;
                                    ECPay_TempLogisticsResultData.LogisticsID = resultData.LogisticsID;
                                    db.Entry(ECPay_TempLogisticsResultData).State = EntityState.Modified;
                                    db.SaveChanges();
                                }

                                return Ok(resultData);
                            }
                            else
                                return Content(HttpStatusCode.BadGateway, resultData.RtnMsg);
                        }
                        else
                            return Content(HttpStatusCode.BadGateway, result.TransMsg);
                    }
                    else
                        return Content(HttpStatusCode.NotImplemented, "綠界科技物流服務API呼叫失敗-建立正式物流訂單");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// (B2C)7-ELEVEN逆物流訂單
        /// </summary>
        /// <param name="data">(B2C)7-ELEVEN逆物流訂單傳入參數</param>
        /// <returns>逆物流訂單資訊</returns>
        [HttpPost]
        [Route("ReturnUniMartCVS")]
        public IHttpActionResult ReturnUniMartCVS([FromBody] ECPayApiRequestData_ReturnB2C data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _SetECPay(data.MerchantID);

                path = ConfigurationManager.AppSettings["ECPayApiPath_ReturnUniMartCVS"];

                using (HttpClient client = new HttpClient())
                {
                    //1.設定API Url
                    url = host + path;

                    //2.設定加密資料
                    string ServerReplyURL = string.IsNullOrWhiteSpace(data.ServerReplyURL) ?
                        ConfigurationManager.AppSettings["LocalApiHost_URL"] + ConfigurationManager.AppSettings["LocalApiPath_ReverseServerReplyURL"] :
                        data.ServerReplyURL;

                    var encodeData = new ECPayApiRequestData_ReturnB2C
                    {
                        MerchantID = _MerchantID,
                        LogisticsID = data.LogisticsID,
                        ServerReplyURL = ServerReplyURL,
                        GoodsName = data.GoodsName,
                        GoodsAmount = data.GoodsAmount,
                        ServiceType = data.ServiceType,
                        SenderName = data.SenderName,
                        SenderPhone = data.SenderPhone,
                        Remark = data.Remark
                    };

                    encodeJsonString = JsonConvert.SerializeObject(encodeData);
                    _CipherText = ECPayApiEncode(encodeJsonString, _HashKey, _HashIV);

                    //3.設定綠界科技請求參數
                    _RequestData = SetECPayApiRequestData(_CipherText);

                    //4.將Json物件轉換成字串
                    jsonString = JsonConvert.SerializeObject(_RequestData);

                    //5.設定Request格式
                    requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                    //6.設定請求時間
                    client.Timeout = TimeSpan.FromSeconds(900000);

                    //7.放入Request參數
                    requestMessage.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    //8.發出API請求
                    responseMessage = client.SendAsync(requestMessage).GetAwaiter().GetResult();
                    responseResult = responseMessage.Content.ReadAsStringAsync().Result.ToString();
                    var result = JsonConvert.DeserializeObject<ECPayApiResponseData>(responseResult);

                    if (responseMessage.StatusCode.ToString() == "OK")
                    {
                        //解密資料
                        _ClearText = ECPayApiDecode(result.Data, _HashKey, _HashIV);
                        var resultData = JsonConvert.DeserializeObject<ECPayApiResponseData_ReturnB2C>(_ClearText);

                        if (resultData.RtnCode == 1)
                        {
                            //更新綠界暫存物流訂單結果
                            var ECPay_TempLogisticsResultData = db.ECPay_TempTradeResults
                                .Where(x => x.MerchantID == data.MerchantID && x.LogisticsID == data.LogisticsID && x.LogisticsID != null)
                                .FirstOrDefault();

                            if (ECPay_TempLogisticsResultData != null)
                            {
                                ECPay_TempLogisticsResultData.RtnMerchantTradeNo = resultData.RtnMerchantTradeNo;
                                ECPay_TempLogisticsResultData.RtnOrderNo = resultData.RtnOrderNo;
                                db.Entry(ECPay_TempLogisticsResultData).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            return Ok(resultData);
                        }
                        else
                            return Content(HttpStatusCode.BadGateway, resultData.RtnMsg);
                    }
                    else
                        return Content(HttpStatusCode.NotImplemented, "綠界科技物流服務API呼叫失敗-(B2C)7-ELEVEN逆物流訂單");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// (B2C)全家逆物流訂單
        /// </summary>
        /// <param name="data">(B2C)全家逆物流訂單傳入參數</param>
        /// <returns>逆物流訂單資訊</returns>
        [HttpPost]
        [Route("ReturnCVS")]
        public IHttpActionResult ReturnCVS([FromBody] ECPayApiRequestData_ReturnB2C data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _SetECPay(data.MerchantID);

                path = ConfigurationManager.AppSettings["ECPayApiPath_ReturnCVS"];

                using (HttpClient client = new HttpClient())
                {
                    //1.設定API Url
                    url = host + path;

                    //2.設定加密資料
                    string ServerReplyURL = string.IsNullOrWhiteSpace(data.ServerReplyURL) ?
                        ConfigurationManager.AppSettings["LocalApiHost_URL"] + ConfigurationManager.AppSettings["LocalApiPath_ReverseServerReplyURL"] :
                        data.ServerReplyURL;

                    var encodeData = new ECPayApiRequestData_ReturnB2C
                    {
                        MerchantID = _MerchantID,
                        LogisticsID = data.LogisticsID,
                        ServerReplyURL = ServerReplyURL,
                        GoodsName = data.GoodsName,
                        GoodsAmount = data.GoodsAmount,
                        ServiceType = data.ServiceType,
                        SenderName = data.SenderName,
                        SenderPhone = data.SenderPhone,
                        Remark = data.Remark
                    };

                    encodeJsonString = JsonConvert.SerializeObject(encodeData);
                    _CipherText = ECPayApiEncode(encodeJsonString, _HashKey, _HashIV);

                    //3.設定綠界科技請求參數
                    _RequestData = SetECPayApiRequestData(_CipherText);

                    //4.將Json物件轉換成字串
                    jsonString = JsonConvert.SerializeObject(_RequestData);

                    //5.設定Request格式
                    requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                    //6.設定請求時間
                    client.Timeout = TimeSpan.FromSeconds(900000);

                    //7.放入Request參數
                    requestMessage.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    //8.發出API請求
                    responseMessage = client.SendAsync(requestMessage).GetAwaiter().GetResult();
                    responseResult = responseMessage.Content.ReadAsStringAsync().Result.ToString();
                    var result = JsonConvert.DeserializeObject<ECPayApiResponseData>(responseResult);

                    if (responseMessage.StatusCode.ToString() == "OK")
                    {
                        //解密資料
                        _ClearText = ECPayApiDecode(result.Data, _HashKey, _HashIV);
                        var resultData = JsonConvert.DeserializeObject<ECPayApiResponseData_ReturnB2C>(_ClearText);

                        if (resultData.RtnCode == 1)
                        {
                            //更新綠界暫存物流訂單結果
                            var ECPay_TempLogisticsResultData = db.ECPay_TempTradeResults
                                .Where(x => x.MerchantID == data.MerchantID && x.LogisticsID == data.LogisticsID && x.LogisticsID != null)
                                .FirstOrDefault();

                            if (ECPay_TempLogisticsResultData != null)
                            {
                                ECPay_TempLogisticsResultData.RtnMerchantTradeNo = resultData.RtnMerchantTradeNo;
                                ECPay_TempLogisticsResultData.RtnOrderNo = resultData.RtnOrderNo;
                                db.Entry(ECPay_TempLogisticsResultData).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            return Ok(resultData);
                        }
                        else
                            return Content(HttpStatusCode.BadGateway, resultData.RtnMsg);
                    }
                    else
                        return Content(HttpStatusCode.NotImplemented, "綠界科技物流服務API呼叫失敗-(B2C)全家逆物流訂單");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// (B2C)萊爾富逆物流訂單
        /// </summary>
        /// <param name="data">(B2C)萊爾富逆物流訂單傳入參數</param>
        /// <returns>逆物流訂單資訊</returns>
        [HttpPost]
        [Route("ReturnHilifeCVS")]
        public IHttpActionResult ReturnHilifeCVS([FromBody] ECPayApiRequestData_ReturnB2C data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _SetECPay(data.MerchantID);

                path = ConfigurationManager.AppSettings["ECPayApiPath_ReturnHilifeCVS"];

                using (HttpClient client = new HttpClient())
                {
                    //1.設定API Url
                    url = host + path;

                    //2.設定加密資料
                    string ServerReplyURL = string.IsNullOrWhiteSpace(data.ServerReplyURL) ?
                        ConfigurationManager.AppSettings["LocalApiHost_URL"] + ConfigurationManager.AppSettings["LocalApiPath_ReverseServerReplyURL"] :
                        data.ServerReplyURL;

                    var encodeData = new ECPayApiRequestData_ReturnB2C
                    {
                        MerchantID = _MerchantID,
                        LogisticsID = data.LogisticsID,
                        ServerReplyURL = ServerReplyURL,
                        GoodsName = data.GoodsName,
                        GoodsAmount = data.GoodsAmount,
                        ServiceType = data.ServiceType,
                        SenderName = data.SenderName,
                        SenderPhone = data.SenderPhone,
                        Remark = data.Remark
                    };

                    encodeJsonString = JsonConvert.SerializeObject(encodeData);
                    _CipherText = ECPayApiEncode(encodeJsonString, _HashKey, _HashIV);

                    //3.設定綠界科技請求參數
                    _RequestData = SetECPayApiRequestData(_CipherText);

                    //4.將Json物件轉換成字串
                    jsonString = JsonConvert.SerializeObject(_RequestData);

                    //5.設定Request格式
                    requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                    //6.設定請求時間
                    client.Timeout = TimeSpan.FromSeconds(900000);

                    //7.放入Request參數
                    requestMessage.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    //8.發出API請求
                    responseMessage = client.SendAsync(requestMessage).GetAwaiter().GetResult();
                    responseResult = responseMessage.Content.ReadAsStringAsync().Result.ToString();
                    var result = JsonConvert.DeserializeObject<ECPayApiResponseData>(responseResult);

                    if (responseMessage.StatusCode.ToString() == "OK")
                    {
                        //解密資料
                        _ClearText = ECPayApiDecode(result.Data, _HashKey, _HashIV);
                        var resultData = JsonConvert.DeserializeObject<ECPayApiResponseData_ReturnB2C>(_ClearText);

                        if (resultData.RtnCode == 1)
                        {
                            //更新綠界暫存物流訂單結果
                            var ECPay_TempLogisticsResultData = db.ECPay_TempTradeResults
                                .Where(x => x.MerchantID == data.MerchantID && x.LogisticsID == data.LogisticsID && x.LogisticsID != null)
                                .FirstOrDefault();

                            if (ECPay_TempLogisticsResultData != null)
                            {
                                ECPay_TempLogisticsResultData.RtnMerchantTradeNo = resultData.RtnMerchantTradeNo;
                                ECPay_TempLogisticsResultData.RtnOrderNo = resultData.RtnOrderNo;
                                db.Entry(ECPay_TempLogisticsResultData).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            return Ok(resultData);
                        }
                        else
                            return Content(HttpStatusCode.BadGateway, resultData.RtnMsg);
                    }
                    else
                        return Content(HttpStatusCode.NotImplemented, "綠界科技物流服務API呼叫失敗-(B2C)萊爾富逆物流訂單");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// 宅配逆物流
        /// </summary>
        /// <param name="data">宅配逆物流傳入參數</param>
        /// <returns>逆物流訂單資訊</returns>
        [HttpPost]
        [Route("ReturnHome")]
        public IHttpActionResult ReturnHome([FromBody] ECPayApiRequestData_ReturnHome data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _SetECPay(data.MerchantID);

                path = ConfigurationManager.AppSettings["ECPayApiPath_ReturnHome"];

                using (HttpClient client = new HttpClient())
                {
                    //1.設定API Url
                    url = host + path;

                    //2.設定加密資料
                    string ServerReplyURL = string.IsNullOrWhiteSpace(data.ServerReplyURL) ?
                        ConfigurationManager.AppSettings["LocalApiHost_URL"] + ConfigurationManager.AppSettings["LocalApiPath_ReverseServerReplyURL"] :
                        data.ServerReplyURL;

                    var encodeData = new ECPayApiRequestData_ReturnHome
                    {
                        MerchantID = _MerchantID,
                        LogisticsID = data.LogisticsID,
                        LogisticsSubType = data.LogisticsSubType,
                        ServerReplyURL = ServerReplyURL,
                        SenderName = data.SenderName,
                        SenderPhone = data.SenderPhone,
                        SenderCellPhone = data.SenderCellPhone,
                        SenderZipCode = data.SenderZipCode,
                        SenderAddress = data.SenderAddress,
                        ReceiverName = data.ReceiverName,
                        ReceiverPhone = data.ReceiverPhone,
                        ReceiverZipCode = data.ReceiverZipCode,
                        ReceiverAddress = data.ReceiverAddress,
                        ReceiverEmail = data.ReceiverEmail,
                        GoodsAmount = data.GoodsAmount,
                        GoodsName = data.GoodsName,
                        Temperature = data.Temperature,
                        Distance = data.Distance,
                        Specification = data.Specification,
                        ScheduledDeliveryTime = data.ScheduledDeliveryTime,
                        ScheduledDeliveryDate = data.ScheduledDeliveryDate,
                        Remark = data.Remark,
                        PlatformID = data.PlatformID
                    };

                    encodeJsonString = JsonConvert.SerializeObject(encodeData);
                    _CipherText = ECPayApiEncode(encodeJsonString, _HashKey, _HashIV);

                    //3.設定綠界科技請求參數
                    _RequestData = SetECPayApiRequestData(_CipherText);

                    //4.將Json物件轉換成字串
                    jsonString = JsonConvert.SerializeObject(_RequestData);

                    //5.設定Request格式
                    requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                    //6.設定請求時間
                    client.Timeout = TimeSpan.FromSeconds(900000);

                    //7.放入Request參數
                    requestMessage.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    //8.發出API請求
                    responseMessage = client.SendAsync(requestMessage).GetAwaiter().GetResult();
                    responseResult = responseMessage.Content.ReadAsStringAsync().Result.ToString();
                    var result = JsonConvert.DeserializeObject<ECPayApiResponseData>(responseResult);

                    if (responseMessage.StatusCode.ToString() == "OK")
                    {
                        //解密資料
                        _ClearText = ECPayApiDecode(result.Data, _HashKey, _HashIV);
                        var resultData = JsonConvert.DeserializeObject<ECPayApiResponseData_BaseResponseData>(_ClearText);

                        if (resultData.RtnCode == 1)
                        {
                            return Ok(resultData);
                        }
                        else
                            return Content(HttpStatusCode.BadGateway, resultData.RtnMsg);
                    }
                    else
                        return Content(HttpStatusCode.NotImplemented, "綠界科技物流服務API呼叫失敗-宅配逆物流");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// 查詢訂單
        /// </summary>
        /// <param name="data">查詢訂單傳入參數</param>
        /// <returns>訂單資訊</returns>
        [HttpPost]
        [Route("QueryLogisticsTradeInfo")]
        public IHttpActionResult QueryLogisticsTradeInfo([FromBody] ECPayApiRequestData_QueryLogisticsTradeInfo data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _SetECPay(data.MerchantID);

                path = ConfigurationManager.AppSettings["ECPayApiPath_QueryLogisticsTradeInfo"];

                using (HttpClient client = new HttpClient())
                {
                    //1.設定API Url
                    url = host + path;

                    //2.設定加密資料
                    var encodeData = new ECPayApiRequestData_QueryLogisticsTradeInfo
                    {
                        MerchantID = _MerchantID,
                        LogisticsID = data.LogisticsID,
                        MerchantTradeNo = data.MerchantTradeNo
                    };

                    encodeJsonString = JsonConvert.SerializeObject(encodeData);
                    _CipherText = ECPayApiEncode(encodeJsonString, _HashKey, _HashIV);

                    //3.設定綠界科技請求參數
                    _RequestData = SetECPayApiRequestData(_CipherText);

                    //4.將Json物件轉換成字串
                    jsonString = JsonConvert.SerializeObject(_RequestData);

                    //5.設定Request格式
                    requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                    //6.設定請求時間
                    client.Timeout = TimeSpan.FromSeconds(900000);

                    //7.放入Request參數
                    requestMessage.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    //8.發出API請求
                    responseMessage = client.SendAsync(requestMessage).GetAwaiter().GetResult();
                    responseResult = responseMessage.Content.ReadAsStringAsync().Result.ToString();
                    var result = JsonConvert.DeserializeObject<ECPayApiResponseData>(responseResult);

                    if (responseMessage.StatusCode.ToString() == "OK")
                    {
                        //解密資料
                        _ClearText = ECPayApiDecode(result.Data, _HashKey, _HashIV);
                        var resultData = JsonConvert.DeserializeObject<ECPayApiResponseData_QueryLogisticsTradeInfo>(_ClearText);

                        if (resultData.RtnCode == 1)
                        {
                            return Ok(resultData);
                        }
                        else
                            return Content(HttpStatusCode.BadGateway, resultData.RtnMsg);
                    }
                    else
                        return Content(HttpStatusCode.NotImplemented, "綠界科技物流服務API呼叫失敗-查詢訂單");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// 取得綠界物流狀態列表
        /// </summary>
        /// <param name="data">取得綠界物流狀態列表傳入參數</param>
        /// <returns>綠界物流狀態列表</returns>
        [HttpPost]
        [Route("GetECPayLogisticsStatus")]
        public IQueryable<ECPay_LogisticsStatus> GetECPayLogisticsStatus([FromBody] ECPayApiRequestData_BaseRequestData data)
        {
            return db.ECPay_LogisticsStatus.Where(x => x.MerchantID == data.MerchantID);
        }

        /// <summary>
        /// 物流狀態(貨態)通知
        /// </summary>
        /// <param name="data">綠界科技物流服務API傳入參數(ServerPost)</param>
        /// <returns>執行結果</returns>
        [HttpPost]
        [Route("ForwardLogistics")]
        public IHttpActionResult ForwardLogistics([FromBody] ECPayApiRequestData_ServerPost data)
        {
            try
            {
                if (data.TransCode == 1)
                {
                    _SetECPay(data.MerchantID);

                    //解密資料
                    _ClearText = ECPayApiDecode(data.Data, _HashKey, _HashIV);
                    var resultData = JsonConvert.DeserializeObject<ECPayApiResponseData_ForwardLogistics>(_ClearText);

                    var ECPay_LogisticsStatusData = db.ECPay_LogisticsStatus
                        .Where(x => x.MerchantID == resultData.MerchantID && x.LogisticsID == resultData.LogisticsID)
                        .FirstOrDefault();

                    if(ECPay_LogisticsStatusData==null)
                    {
                        //新增綠界物流狀態
                        var newECPay_LogisticsStatus = new ECPay_LogisticsStatus
                        {
                            MerchantID= resultData.MerchantID,
                            LogisticsType = resultData.LogisticsType,
                            LogisticsSubType = resultData.LogisticsSubType,
                            MerchantTradeNo = resultData.MerchantTradeNo,
                            LogisticsID = resultData.LogisticsID,
                            LogisticsStatus = resultData.LogisticsStatus,
                            LogisticsStatusName = resultData.LogisticsStatusName,
                            GoodsAmount = resultData.GoodsAmount,
                            UpdateStatusDate = resultData.UpdateStatusDate,
                            ReceiverName = resultData.ReceiverName,
                            ReceiverPhone = resultData.ReceiverPhone,
                            ReceiverCellphone = resultData.ReceiverCellphone,
                            ReceiverEmail = resultData.ReceiverEmail,
                            ReceiverAddress = resultData.ReceiverAddress,
                            CVSPaymentNo = resultData.CVSPaymentNo,
                            CVSValidationNo = resultData.CVSValidationNo,
                            BookingNote = resultData.BookingNote
                        };

                        db.ECPay_LogisticsStatus.Add(newECPay_LogisticsStatus);
                    }
                    else
                    {
                        //編輯綠界物流狀態
                        ECPay_LogisticsStatusData.LogisticsType = resultData.LogisticsType;
                        ECPay_LogisticsStatusData.LogisticsSubType = resultData.LogisticsSubType;
                        ECPay_LogisticsStatusData.MerchantTradeNo = resultData.MerchantTradeNo;
                        ECPay_LogisticsStatusData.LogisticsStatus = resultData.LogisticsStatus;
                        ECPay_LogisticsStatusData.LogisticsStatusName = resultData.LogisticsStatusName;
                        ECPay_LogisticsStatusData.GoodsAmount = resultData.GoodsAmount;
                        ECPay_LogisticsStatusData.UpdateStatusDate = resultData.UpdateStatusDate;
                        ECPay_LogisticsStatusData.ReceiverName = resultData.ReceiverName;
                        ECPay_LogisticsStatusData.ReceiverPhone = resultData.ReceiverPhone;
                        ECPay_LogisticsStatusData.ReceiverCellphone = resultData.ReceiverCellphone;
                        ECPay_LogisticsStatusData.ReceiverEmail = resultData.ReceiverEmail;
                        ECPay_LogisticsStatusData.ReceiverAddress = resultData.ReceiverAddress;
                        ECPay_LogisticsStatusData.CVSPaymentNo = resultData.CVSPaymentNo;
                        ECPay_LogisticsStatusData.CVSValidationNo = resultData.CVSValidationNo;
                        ECPay_LogisticsStatusData.BookingNote = resultData.BookingNote;

                        db.Entry(ECPay_LogisticsStatusData).State = EntityState.Modified;
                    }

                    db.SaveChanges();

                    var encodeData = new ECPayApiResponseData_BaseResponseData
                    {
                        RtnCode = 1,
                        RtnMsg = "成功"
                    };

                    encodeJsonString = JsonConvert.SerializeObject(encodeData);
                    _CipherText = ECPayApiEncode(encodeJsonString, _HashKey, _HashIV);

                    return Ok(SetECPayApiResponseData(_CipherText));
                }
                else
                    return Content(HttpStatusCode.NotImplemented, "綠界科技物流服務API呼叫失敗-物流狀態(貨態)通知");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// 物流狀態(逆物流)通知
        /// </summary>
        /// <param name="data">綠界科技物流服務API傳入參數(ServerPost)</param>
        /// <returns>執行結果</returns>
        [HttpPost]
        [Route("ReverseLogistics")]
        public IHttpActionResult ReverseLogistics([FromBody] ECPayApiRequestData_ServerPost data)
        {
            try
            {
                if (data.TransCode == 1)
                {
                    _SetECPay(data.MerchantID);

                    //解密資料
                    _ClearText = ECPayApiDecode(data.Data, _HashKey, _HashIV);
                    var resultData = JsonConvert.DeserializeObject<ECPayApiResponseData_ReverseLogistics>(_ClearText);

                    var ECPay_LogisticsStatusData = db.ECPay_LogisticsStatus
                        .Where(x => x.MerchantID == resultData.MerchantID && x.LogisticsID == resultData.LogisticsID)
                        .FirstOrDefault();

                    if (ECPay_LogisticsStatusData == null)
                    {
                        //新增綠界物流狀態
                        var newECPay_LogisticsStatus = new ECPay_LogisticsStatus
                        {
                            LogisticsStatus = resultData.LogisticsStatus,
                            LogisticsStatusName = resultData.LogisticsStatusName,
                            MerchantID = resultData.MerchantID,
                            RtnMerchantTradeNo = resultData.RtnMerchantTradeNo,
                            LogisticsID = resultData.LogisticsID,
                            GoodsAmount = resultData.GoodsAmount,
                            UpdateStatusDate = resultData.UpdateStatusDate,
                            BookingNote = resultData.BookingNote,
                        };

                        db.ECPay_LogisticsStatus.Add(newECPay_LogisticsStatus);
                    }
                    else
                    {
                        //編輯綠界物流狀態
                        ECPay_LogisticsStatusData.LogisticsStatus = resultData.LogisticsStatus;
                        ECPay_LogisticsStatusData.LogisticsStatusName = resultData.LogisticsStatusName;
                        ECPay_LogisticsStatusData.RtnMerchantTradeNo = resultData.RtnMerchantTradeNo;
                        ECPay_LogisticsStatusData.GoodsAmount = resultData.GoodsAmount;
                        ECPay_LogisticsStatusData.UpdateStatusDate = resultData.UpdateStatusDate;
                        ECPay_LogisticsStatusData.BookingNote = resultData.BookingNote;

                        db.Entry(ECPay_LogisticsStatusData).State = EntityState.Modified;
                    }

                    db.SaveChanges();

                    var encodeData = new ECPayApiResponseData_BaseResponseData
                    {
                        RtnCode = 1,
                        RtnMsg = "成功"
                    };

                    encodeJsonString = JsonConvert.SerializeObject(encodeData);
                    _CipherText = ECPayApiEncode(encodeJsonString, _HashKey, _HashIV);

                    return Ok(SetECPayApiResponseData(_CipherText));
                }
                else
                    return Content(HttpStatusCode.NotImplemented, "綠界科技物流服務API呼叫失敗-物流狀態(逆物流)通知");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// (B2C)物流訂單異動
        /// </summary>
        /// <param name="data">(B2C)物流訂單異動傳入參數</param>
        /// <returns>執行結果</returns>
        [HttpPost]
        [Route("UpdateShipmentInfo")]
        public IHttpActionResult UpdateShipmentInfo([FromBody] ECPayApiRequestData_UpdateShipmentInfo data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _SetECPay(data.MerchantID);

                path = ConfigurationManager.AppSettings["ECPayApiPath_UpdateShipmentInfo"];

                using (HttpClient client = new HttpClient())
                {
                    //1.設定API Url
                    url = host + path;

                    //2.設定加密資料
                    var encodeData = new ECPayApiRequestData_UpdateShipmentInfo
                    {
                        MerchantID = _MerchantID,
                        LogisticsID = data.LogisticsID,
                        ShipmentDate = data.ShipmentDate,
                        ReceiverStoreID = data.ReceiverStoreID
                    };

                    encodeJsonString = JsonConvert.SerializeObject(encodeData);
                    _CipherText = ECPayApiEncode(encodeJsonString, _HashKey, _HashIV);

                    //3.設定綠界科技請求參數
                    _RequestData = SetECPayApiRequestData(_CipherText);

                    //4.將Json物件轉換成字串
                    jsonString = JsonConvert.SerializeObject(_RequestData);

                    //5.設定Request格式
                    requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                    //6.設定請求時間
                    client.Timeout = TimeSpan.FromSeconds(900000);

                    //7.放入Request參數
                    requestMessage.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    //8.發出API請求
                    responseMessage = client.SendAsync(requestMessage).GetAwaiter().GetResult();
                    responseResult = responseMessage.Content.ReadAsStringAsync().Result.ToString();
                    var result = JsonConvert.DeserializeObject<ECPayApiResponseData>(responseResult);

                    if (responseMessage.StatusCode.ToString() == "OK")
                    {
                        //解密資料
                        _ClearText = ECPayApiDecode(result.Data, _HashKey, _HashIV);
                        var resultData = JsonConvert.DeserializeObject<ECPayApiResponseData_BaseResponseData>(_ClearText);

                        if (resultData.RtnCode == 1)
                        {
                            return Ok(resultData);
                        }
                        else
                            return Content(HttpStatusCode.BadGateway, resultData.RtnMsg);
                    }
                    else
                        return Content(HttpStatusCode.NotImplemented, "綠界科技物流服務API呼叫失敗-(B2C)物流訂單異動");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// (C2C)物流訂單異動
        /// </summary>
        /// <param name="data">(C2C)物流訂單異動傳入參數</param>
        /// <returns>執行結果</returns>
        [HttpPost]
        [Route("UpdateStoreInfo")]
        public IHttpActionResult UpdateStoreInfo([FromBody] ECPayApiRequestData_UpdateStoreInfo data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _SetECPay(data.MerchantID);

                path = ConfigurationManager.AppSettings["ECPayApiPath_UpdateStoreInfo"];

                using (HttpClient client = new HttpClient())
                {
                    //1.設定API Url
                    url = host + path;

                    //2.設定加密資料
                    var encodeData = new ECPayApiRequestData_UpdateStoreInfo
                    {
                        MerchantID = _MerchantID,
                        LogisticsID = data.LogisticsID,
                        CVSPaymentNo = data.CVSPaymentNo,
                        CVSValidationNo = data.CVSValidationNo,
                        StoreType = data.StoreType,
                        ReceiverStoreID = data.ReceiverStoreID,
                        ReturnStoreID = data.ReturnStoreID
                    };

                    encodeJsonString = JsonConvert.SerializeObject(encodeData);
                    _CipherText = ECPayApiEncode(encodeJsonString, _HashKey, _HashIV);

                    //3.設定綠界科技請求參數
                    _RequestData = SetECPayApiRequestData(_CipherText);

                    //4.將Json物件轉換成字串
                    jsonString = JsonConvert.SerializeObject(_RequestData);

                    //5.設定Request格式
                    requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                    //6.設定請求時間
                    client.Timeout = TimeSpan.FromSeconds(900000);

                    //7.放入Request參數
                    requestMessage.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    //8.發出API請求
                    responseMessage = client.SendAsync(requestMessage).GetAwaiter().GetResult();
                    responseResult = responseMessage.Content.ReadAsStringAsync().Result.ToString();
                    var result = JsonConvert.DeserializeObject<ECPayApiResponseData>(responseResult);

                    if (responseMessage.StatusCode.ToString() == "OK")
                    {
                        //解密資料
                        _ClearText = ECPayApiDecode(result.Data, _HashKey, _HashIV);
                        var resultData = JsonConvert.DeserializeObject<ECPayApiResponseData_BaseResponseData>(_ClearText);

                        if (resultData.RtnCode == 1)
                        {
                            return Ok(resultData);
                        }
                        else
                            return Content(HttpStatusCode.BadGateway, resultData.RtnMsg);
                    }
                    else
                        return Content(HttpStatusCode.NotImplemented, "綠界科技物流服務API呼叫失敗-(C2C)物流訂單異動");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        /// <summary>
        /// 取消訂單(7-EVEVEN超商C2C)
        /// </summary>
        /// <param name="data">取消訂單(7-EVEVEN超商C2C)傳入參數</param>
        /// <returns>執行結果</returns>
        [HttpPost]
        [Route("CancelC2COrder")]
        public IHttpActionResult CancelC2COrder([FromBody] ECPayApiRequestData_CancelC2COrder data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _SetECPay(data.MerchantID);

                path = ConfigurationManager.AppSettings["ECPayApiPath_CancelC2COrder"];

                using (HttpClient client = new HttpClient())
                {
                    //1.設定API Url
                    url = host + path;

                    //2.設定加密資料
                    var encodeData = new ECPayApiRequestData_UpdateStoreInfo
                    {
                        MerchantID = _MerchantID,
                        LogisticsID = data.LogisticsID,
                        CVSPaymentNo = data.CVSPaymentNo,
                        CVSValidationNo = data.CVSValidationNo,
                    };
                    encodeJsonString = JsonConvert.SerializeObject(encodeData);
                    _CipherText = ECPayApiEncode(encodeJsonString, _HashKey, _HashIV);

                    //3.設定綠界科技請求參數
                    _RequestData = SetECPayApiRequestData(_CipherText);

                    //4.將Json物件轉換成字串
                    jsonString = JsonConvert.SerializeObject(_RequestData);

                    //5.設定Request格式
                    requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                    //6.設定請求時間
                    client.Timeout = TimeSpan.FromSeconds(900000);

                    //7.放入Request參數
                    requestMessage.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    //8.發出API請求
                    responseMessage = client.SendAsync(requestMessage).GetAwaiter().GetResult();
                    responseResult = responseMessage.Content.ReadAsStringAsync().Result.ToString();
                    var result = JsonConvert.DeserializeObject<ECPayApiResponseData>(responseResult);

                    if (responseMessage.StatusCode.ToString() == "OK")
                    {
                        //解密資料
                        _ClearText = ECPayApiDecode(result.Data, _HashKey, _HashIV);
                        var resultData = JsonConvert.DeserializeObject<ECPayApiResponseData_BaseResponseData>(_ClearText);

                        if (resultData.RtnCode == 1)
                        {
                            return Ok(resultData);
                        }
                        else
                            return Content(HttpStatusCode.BadGateway, resultData.RtnMsg);
                    }
                    else
                        return Content(HttpStatusCode.NotImplemented, "綠界科技物流服務API呼叫失敗-取消訂單(7-EVEVEN超商C2C)");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
