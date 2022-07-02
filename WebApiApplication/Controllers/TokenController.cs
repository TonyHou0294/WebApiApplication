using Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using WebApiApplication.ViewModels;

namespace WebApiApplication.Controllers
{
    /// <summary>
    /// 令牌處理
    /// </summary>
    [RoutePrefix("api/Token")]
    public class TokenController : BaseApiController
    {
        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="data">登錄請求資料</param>
        /// <returns>令牌資料</returns>
        [HttpPost]
        [Route("SignIn")]
        [ResponseType(typeof(TokenResponseData))]
        public IHttpActionResult SignIn([FromBody] SignInRequestData data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = db.Users.Where(x => x.UserId == data.UserId).FirstOrDefault();

            //使用者編號檢查
            if (user == null)
            {
                return Content(HttpStatusCode.NotFound, "查無使用者資料");
            }

            //密碼檢查
            string Password = CryptoHelper.GenerateHash(data.Password, user.ApiKey);
            var checkUser = db.Users.Where(x => x.UserId == data.UserId && x.Password == Password).Any();
            if(!checkUser)
            {
                return BadRequest("登入失敗，帳號或密碼錯誤");
            }

            //產生 Token
            var token = bearerTokenManager.CreateToken(user);

            //更新使用者刷新令牌
            user.RefreshToken = token.RefreshToken;
            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(token);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="data">刷新請求資料</param>
        /// <returns>令牌資料</returns>
        [HttpPost]
        [Route("Refresh")]
        [ResponseType(typeof(TokenResponseData))]
        public IHttpActionResult Refresh([FromBody] RefreshRequestData data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = db.Users.Where(x => x.UserId == data.UserId).FirstOrDefault();

            //使用者編號檢查
            if (user == null)
            {
                return Content(HttpStatusCode.NotFound, "查無使用者資料");
            }

            //刷新令牌檢查
            var checkUser = db.Users.Where(x => x.UserId == data.UserId && x.RefreshToken == data.RefreshToken).Any();
            if (!checkUser)
            {
                return BadRequest("刷新令牌失效，請重新登入");
            }

            //產生新的 Token
            var token = bearerTokenManager.CreateToken(user);

            //更新使用者刷新令牌
            user.RefreshToken = token.RefreshToken;
            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(token);
        }
    }
}
