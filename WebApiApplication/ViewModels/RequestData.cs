﻿using System.ComponentModel.DataAnnotations;

namespace WebApiApplication.ViewModels
{
    /// <summary>
    /// 使用者請求資料
    /// </summary>
    public class UserRequestData
    {
        /// <summary>
        /// 使用者編號
        /// </summary>
        [Required]
        [StringLength(50)]
        public string UserId { get; set; }

        /// <summary>
        /// 使用者名稱
        /// </summary>
        [StringLength(50)]
        public string UserName { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Password { get; set; }
    }

    /// <summary>
    /// 登入請求資料
    /// </summary>
    public class SignInRequestData
    {
        /// <summary>
        /// 使用者編號
        /// </summary>
        [Required]
        [StringLength(50)]
        public string UserId { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Password { get; set; }
    }

    /// <summary>
    /// 刷新請求資料
    /// </summary>
    public class RefreshRequestData
    {
        /// <summary>
        /// 使用者編號
        /// </summary>
        [Required]
        [StringLength(50)]
        public string UserId { get; set; }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        [Required]
        [StringLength(50)]
        public string RefreshToken { get; set; }
    }
}