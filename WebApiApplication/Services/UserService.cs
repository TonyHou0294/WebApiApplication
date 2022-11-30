using Common;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using WebApiApplication.DataAccess.Interfaces;
using WebApiApplication.Models;
using WebApiApplication.ViewModels.Request;
using WebApiApplication.ViewModels.Response;

namespace WebApiApplication.Services
{
    public class UserService
    {
        private readonly IRepository<User> _UserRepository;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public UserService(IRepository<User> UserRepository)
        {
            _UserRepository = UserRepository;
        }

        public async Task<IQueryable<User>> Get()
        {
            _logger.Error($"UserService-Get");
            return _UserRepository.GetAll();
        }

        public async Task<ApiResponse<User>> Get(long id)
        {
            _logger.Error($"UserService-Get Data:{id}");
            User user = _UserRepository.Find(id);
            ApiResponse<User> returnData = new ApiResponse<User>();

            if (user == null)
            {
                returnData.StatusCode = HttpStatusCode.NotFound;
                returnData.Message = "查無使用者資料";
            }
            else
            {
                returnData.StatusCode = HttpStatusCode.OK;
                returnData.Data = user;
            }

            return returnData;
        }

        public async Task<ApiResponse<User>> Post(UserRequest request)
        {
            ApiResponse<User> returnData = new ApiResponse<User>();

            var userData = _UserRepository.SearchFor(x => x.UserId == request.UserId).FirstOrDefault();

            if (userData != null)
            {
                returnData.StatusCode = HttpStatusCode.BadRequest;
                returnData.Message = "使用者編號已存在";
            }
            else
            {
                string ApiKey = Guid.NewGuid().ToString().Replace("-", "");
                string Password = CryptoHelper.GenerateHash(request.Password, ApiKey);

                User newUserData = new User()
                {
                    UserId = request.UserId,
                    UserName = request.UserName,
                    Password = Password,
                    ApiKey = ApiKey
                };

                _UserRepository.Insert(newUserData);
                _UserRepository.SaveChanges();

                returnData.StatusCode = HttpStatusCode.Created;
                returnData.Data = newUserData;
            }

            return returnData;
        }

        public async Task<ApiResponse<User>> Put(long id, UserRequest request)
        {
            User user = _UserRepository.Find(id);
            ApiResponse<User> returnData = new ApiResponse<User>();

            if (user == null)
            {
                returnData.StatusCode = HttpStatusCode.NotFound;
                returnData.Message = "查無使用者資料";
            }
            else
            {
                var userData = _UserRepository.SearchFor(x => x.ID != id && x.UserId == request.UserId).FirstOrDefault();

                if(userData != null)
                {
                    returnData.StatusCode = HttpStatusCode.BadRequest;
                    returnData.Message = "使用者編號已存在";
                }
                else
                {
                    string Password = CryptoHelper.GenerateHash(request.Password, user.ApiKey);

                    if (!string.IsNullOrWhiteSpace(request.UserName))
                        user.UserName = request.UserName;

                    user.UserId = request.UserId;
                    user.Password = Password;

                    _UserRepository.Update(user);
                    _UserRepository.SaveChanges();

                    returnData.StatusCode = HttpStatusCode.Accepted;
                    returnData.Data = user;
                }
            }

            return returnData;
        }

        public async Task<ApiResponse<User>> Delete(long id)
        {
            User user = _UserRepository.Find(id);
            ApiResponse<User> returnData = new ApiResponse<User>();

            if (user == null)
            {
                returnData.StatusCode = HttpStatusCode.NotFound;
                returnData.Message = "查無使用者資料";
            }
            else
            {
                _UserRepository.DeleteByKey(id);
                _UserRepository.SaveChanges();

                returnData.StatusCode = HttpStatusCode.OK;
                returnData.Data = user;
            }

            return returnData;
        }
    }
}