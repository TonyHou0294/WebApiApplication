using Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApiApplication.Models;
using WebApiApplication.ViewModels;

namespace WebApiApplication.Controllers
{
    /// <summary>
    /// 使用者資料操作(REST)
    /// </summary>
    public class UserController : BaseApiController
    {
        /// <summary>
        /// 瀏覽全部使用者資料
        /// </summary>
        /// <returns>使用者清單</returns>
        public IQueryable<User> Get()
        {
            return db.Users;
        }

        /// <summary>
        /// 瀏覽特定使用者資料
        /// </summary>
        /// <param name="id">使用者ID</param>
        /// <returns>使用者資料</returns>
        [ResponseType(typeof(User))]
        public IHttpActionResult Get([FromUri] long id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return Content(HttpStatusCode.NotFound, "查無使用者資料");
            }

            return Ok(user);
        }

        /// <summary>
        /// 新增一筆使用者資料
        /// </summary>
        /// <param name="data">使用者請求資料</param>
        /// <returns>埦行結果</returns>
        [ResponseType(typeof(User))]
        public IHttpActionResult Post([FromBody] UserRequestData data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userData = db.Users.Where(x => x.UserId == data.UserId).FirstOrDefault();

            if (userData != null)
            {
                return BadRequest("使用者編號已存在");
            }

            string ApiKey = Guid.NewGuid().ToString().Replace("-", "");
            string Password = CryptoHelper.GenerateHash(data.Password, ApiKey);

            User newUserData = new User()
            {
                UserId = data.UserId,
                UserName = data.UserName,
                Password = Password,
                ApiKey = ApiKey
            };

            db.Users.Add(newUserData);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = newUserData.ID }, newUserData);
        }

        /// <summary>
        /// 修改特定使用者資料
        /// </summary>
        /// <param name="id">使用者ID</param>
        /// <param name="data">使用者請求資料</param>
        /// <returns>埦行結果</returns>
        [ResponseType(typeof(User))]
        public IHttpActionResult Put([FromUri] long id, [FromBody] UserRequestData data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = db.Users.Find(id);
            if (user == null)
            {
                return Content(HttpStatusCode.NotFound, "查無使用者資料");
            }

            var userData = db.Users.Where(x => x.ID != id && x.UserId == data.UserId).FirstOrDefault();

            if (userData != null)
            {
                return BadRequest("使用者編號已存在");
            }

            string Password = CryptoHelper.GenerateHash(data.Password, user.ApiKey);

            if (!string.IsNullOrWhiteSpace(data.UserName))
                user.UserName = data.UserName;

            user.UserId = data.UserId;
            user.Password = Password;

            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Content(HttpStatusCode.Accepted, user);
        }

        /// <summary>
        /// 刪除指定使用者資料
        /// </summary>
        /// <param name="id">使用者ID</param>
        /// <returns>埦行結果</returns>
        [ResponseType(typeof(User))]
        public IHttpActionResult Delete(long id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return Content(HttpStatusCode.NotFound, "查無使用者資料");
            }

            db.Users.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }
    }
}
