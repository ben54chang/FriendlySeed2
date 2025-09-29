using System.ComponentModel.DataAnnotations;

namespace FriendlySeed.Models
{
    public class Article
    {
        [Display(Name = "ID")]
        public int ID { get; set; }
        
        [StringLength(50, ErrorMessage = "代稱長度不能超過50個字元")]
        [Display(Name = "代稱")]
        public string? Alias { get; set; }
        
        [Required(ErrorMessage = "分類為必填")]
        [Display(Name = "分類")]
        public int CategoryID { get; set; }
        
        [Required(ErrorMessage = "作者為必填")]
        [StringLength(100, ErrorMessage = "作者長度不能超過100個字元")]
        [Display(Name = "作者")]
        public string Author { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "標題為必填")]
        [StringLength(200, ErrorMessage = "標題長度不能超過200個字元")]
        [Display(Name = "標題")]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "關鍵字長度不能超過500個字元")]
        [Display(Name = "關鍵字")]
        public string? Keywords { get; set; }
        
        [Display(Name = "內容")]
        public string? Description { get; set; }
        
        [StringLength(500, ErrorMessage = "標籤長度不能超過500個字元")]
        [Display(Name = "標籤")]
        public string? Tags { get; set; }
        
        [StringLength(500, ErrorMessage = "檔案路徑長度不能超過500個字元")]
        [Display(Name = "檔案")]
        public string? FilePath { get; set; }
        
        [Display(Name = "排序")]
        public int SortOrder { get; set; } = 0;
        
        [Display(Name = "發布時間")]
        public DateTime? PublishTime { get; set; }
        
        [Display(Name = "置頂")]
        public bool IsTop { get; set; } = false;
        
        [Display(Name = "啟用狀態")]
        public bool IsActive { get; set; } = true;
        
        [Display(Name = "建立時間")]
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        
        [Display(Name = "更新時間")]
        public DateTime UpdatedTime { get; set; } = DateTime.Now;
        
        // Navigation properties
        public ArticleCategory? Category { get; set; }
    }
}
