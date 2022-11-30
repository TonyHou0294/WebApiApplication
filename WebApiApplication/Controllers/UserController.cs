using Common;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using WebApiApplication.Models;
using WebApiApplication.Services;
using WebApiApplication.ViewModels;
using WebApiApplication.ViewModels.Request;
using WebApiApplication.ViewModels.Response;
using static WebApiApplication.Helpers.AuthorizationHelper.BasicAuthManager;

namespace WebApiApplication.Controllers
{
    /// <summary>
    /// 使用者資料操作(REST + Basic Auth)
    /// </summary>
    //[BasicAuthAuthorizationFilterAttribute]
    public class UserController : BaseApiController
    {
        private UserService _UserService;

        public UserController(UserService UserService)
        {
            _UserService = UserService;
        }

        /// <summary>
        /// 瀏覽全部使用者資料
        /// </summary>
        /// <returns>使用者清單</returns>
        public async Task<IQueryable<User>> Get()
        {
            return await _UserService.Get();
        }

        /// <summary>
        /// 瀏覽特定使用者資料
        /// </summary>
        /// <param name="id">使用者ID</param>
        /// <returns>使用者資料</returns>
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> Get([FromUri] long id)
        {
            var Result = await _UserService.Get(id);
            return Json(Result);
        }

        /// <summary>
        /// 新增一筆使用者資料
        /// </summary>
        /// <param name="request">使用者請求資料</param>
        /// <returns>埦行結果</returns>
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> Post([FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Json(new ApiResponse<ModelStateDictionary>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Data = ModelState
                });                
            }
            else
            {
                var Result = await _UserService.Post(request);
                return Json(Result);
            }
        }

        /// <summary>
        /// 修改特定使用者資料
        /// </summary>
        /// <param name="id">使用者ID</param>
        /// <param name="request">使用者請求資料</param>
        /// <returns>埦行結果</returns>
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> Put([FromUri] long id, [FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Json(new ApiResponse<ModelStateDictionary>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Data = ModelState
                });
            }
            else
            {
                var Result = await _UserService.Put(id, request);
                return Json(Result);
            }
        }

        /// <summary>
        /// 刪除指定使用者資料
        /// </summary>
        /// <param name="id">使用者ID</param>
        /// <returns>埦行結果</returns>
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> Delete(long id)
        {
            var Result = await _UserService.Delete(id);
            return Json(Result);
        }
    }
}
