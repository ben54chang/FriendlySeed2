using System.ComponentModel.DataAnnotations;

namespace FriendlySeed.Models
{
    public class ArticleCategoryTest
    {
        [Key]
        public int ArticleCategoryID { get; set; }
        
        [Required(ErrorMessage = "請輸入文章分類名稱")]
        [StringLength(100, ErrorMessage = "文章分類名稱長度不能超過100個字元")]
        public string ArticleCategoryName { get; set; } = string.Empty;
        
        public int? SortOrder { get; set; } = 10;
        
        public bool IsActive { get; set; } = true;
    }
}
