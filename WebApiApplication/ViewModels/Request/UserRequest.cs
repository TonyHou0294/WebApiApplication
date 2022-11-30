using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApiApplication.ViewModels.Request
{
    /// <summary>
    /// 使用者請求資料
    /// </summary>
    public class UserRequest
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
}