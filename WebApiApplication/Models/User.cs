using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplication.Models
{
    /// <summary>
    /// 使用者資料
    /// </summary>
    public class User
    {
        /// <summary>
        /// ID
        /// </summary>
        [DisplayName("ID")]
        [Key]
        [Required]
        public long ID { get; set; }

        /// <summary>
        /// 使用者編號
        /// </summary>
        [DisplayName("使用者編號")]
        [Required]
        [StringLength(50)]
        public string UserId { get; set; }

        /// <summary>
        /// 使用者名稱
        /// </summary>
        [DisplayName("使用者名稱")]
        [StringLength(50)]
        public string UserName { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [DisplayName("密碼")]
        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        /// <summary>
        /// API金鑰
        /// </summary>
        [DisplayName("API金鑰")]
        [Required]
        [StringLength(50)]
        public string ApiKey { get; set; }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        [DisplayName("刷新令牌")]
        [StringLength(50)]
        public string RefreshToken { get; set; }
    }
}