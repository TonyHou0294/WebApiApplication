using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplication.Models
{
    /// <summary>
    /// ECPay會員資料
    /// </summary>
    public class ECPay
    {
        /// <summary>
        /// ID
        /// </summary>
        [DisplayName("ID")]
        [Key]
        [Required]
        public long ID { get; set; }

        /// <summary>
        /// 會員編號
        /// </summary>
        [DisplayName("會員編號")]
        [Required]
        [StringLength(50)]
        public string MerchantID { get; set; }

        /// <summary>
        /// 秘密金鑰
        /// </summary>
        [DisplayName("秘密金鑰")]
        [Required]
        [StringLength(50)]
        public string HashKey { get; set; }

        /// <summary>
        /// 初始化向量
        /// </summary>
        [DisplayName("初始化向量")]
        [Required]
        [StringLength(50)]
        public string HashIV { get; set; }

        /// <summary>
        /// 是否測試
        /// </summary>
        [DisplayName("是否測試")]
        [Required]
        public bool IsStage { get; set; }

    }
}