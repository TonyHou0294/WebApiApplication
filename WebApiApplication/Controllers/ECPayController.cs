using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using WebApiApplication.Models;
using static WebApiApplication.Helpers.AuthorizationHelper.BearerTokenManager;

namespace WebApiApplication.Controllers
{
    /// <summary>
    /// ECPay會員資料操作(REST + Bearer Token)
    /// </summary>
    [RoutePrefix("api/ECPay")]
    [BearerTokenAuthorizationFilterAttribute]
    public class ECPayController : BaseApiController
    {
        /// <summary>
        /// 瀏覽全部ECPay會員資料
        /// </summary>
        /// <returns>ECPay會員清單</returns>
        public IQueryable<ECPay> Get()
        {
            return db.ECPays;
        }

        /// <summary>
        /// 瀏覽特定ECPay會員資料
        /// </summary>
        /// <param name="id">ECPay會員ID</param>
        /// <returns>ECPay會員資料</returns>
        [ResponseType(typeof(ECPay))]
        public IHttpActionResult Get(long id)
        {
            ECPay eCPay = db.ECPays.Find(id);
            if (eCPay == null)
            {
                return NotFound();
            }

            return Ok(eCPay);
        }

        /// <summary>
        /// 新增一筆ECPay會員資料
        /// </summary>
        /// <param name="eCPay">ECPay會員資料</param>
        /// <returns>埦行結果</returns>
        [ResponseType(typeof(ECPay))]
        public IHttpActionResult Post(ECPay eCPay)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var eCPayData = db.ECPays.Where(x => x.MerchantID == eCPay.MerchantID).FirstOrDefault();
            if(eCPayData!=null)
            {
                return BadRequest("會員編號已存在");
            }

            db.ECPays.Add(eCPay);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = eCPay.ID }, eCPay);
        }

        /// <summary>
        /// 修改特定ECPay會員資料
        /// </summary>
        /// <param name="id">ECPay會員ID</param>
        /// <param name="eCPay">ECPay會員資料</param>
        /// <returns>埦行結果</returns>
        [ResponseType(typeof(ECPay))]
        public IHttpActionResult Put(long id, ECPay eCPay)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != eCPay.ID)
            {
                return BadRequest();
            }

            var eCPayData = db.ECPays.Where(x =>x.ID != eCPay.ID && x.MerchantID == eCPay.MerchantID).FirstOrDefault();
            if (eCPayData != null)
            {
                return BadRequest("會員編號已存在");
            }

            db.Entry(eCPay).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ECPayExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Content(HttpStatusCode.Accepted, eCPay);
        }

        /// <summary>
        /// 刪除指定ECPay會員資料
        /// </summary>
        /// <param name="id">ECPay會員ID</param>
        /// <returns>埦行結果</returns>
        [ResponseType(typeof(ECPay))]
        public IHttpActionResult Delete(long id)
        {
            ECPay eCPay = db.ECPays.Find(id);
            if (eCPay == null)
            {
                return NotFound();
            }

            db.ECPays.Remove(eCPay);
            db.SaveChanges();

            return Ok(eCPay);
        }

        private bool ECPayExists(long id)
        {
            return db.ECPays.Count(e => e.ID == id) > 0;
        }
    }
}
