using System.ComponentModel.DataAnnotations;

namespace FriendlySeed.Models
{
    public class ArticleCategory
    {
        [Key]
        [Display(Name = "文章分類ID")]
        public int ArticleCategoryID { get; set; }
        
        [Required(ErrorMessage = "請輸入文章分類名稱")]
        [StringLength(100, ErrorMessage = "文章分類名稱長度不能超過100個字元")]
        [Display(Name = "文章分類名稱")]
        public string ArticleCategoryName { get; set; } = string.Empty;
        
        [StringLength(200, ErrorMessage = "分類關鍵字長度不能超過200個字元")]
        [Display(Name = "分類關鍵字")]
        public string? CategoryKeywords { get; set; }
        
        [StringLength(500, ErrorMessage = "分類描述長度不能超過500個字元")]
        [Display(Name = "分類描述")]
        public string? CategoryDescription { get; set; }
        
        [Display(Name = "排序")]
        public int SortOrder { get; set; } = 10;
        
        [Display(Name = "建立時間")]
        public DateTime? CreatedTime { get; set; }
        
        [Display(Name = "更新時間")]
        public DateTime? UpdatedTime { get; set; }
        
        [Display(Name = "啟用狀態")]
        public bool IsActive { get; set; } = true;
    }
}