using System.ComponentModel.DataAnnotations;

namespace FriendlySeed.Models
{
    public class WebsiteSettings
    {
        [Display(Name = "ID")]
        public int ID { get; set; }
        
        [Required(ErrorMessage = "網站名稱為必填")]
        [StringLength(100, ErrorMessage = "網站名稱長度不能超過100個字元")]
        [Display(Name = "網站名稱")]
        public string WebsiteName { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "版權聲明長度不能超過500個字元")]
        [Display(Name = "版權聲明")]
        public string? CopyrightText { get; set; }
        
        [EmailAddress(ErrorMessage = "請輸入有效的電子信箱格式")]
        [StringLength(100, ErrorMessage = "收件人信箱長度不能超過100個字元")]
        [Display(Name = "收件人")]
        public string? ContactEmail { get; set; }
        
        [StringLength(500, ErrorMessage = "網站小圖示路徑長度不能超過500個字元")]
        [Display(Name = "網站小圖示")]
        public string? Favicon { get; set; }
        
        [StringLength(500, ErrorMessage = "網站LOGO路徑長度不能超過500個字元")]
        [Display(Name = "網站LOGO")]
        public string? Logo { get; set; }
        
        [StringLength(500, ErrorMessage = "網站OG圖片路徑長度不能超過500個字元")]
        [Display(Name = "網站OG圖片")]
        public string? OgImage { get; set; }
        
        [Display(Name = "Content Security Policy")]
        public string? ContentSecurityPolicy { get; set; }
        
        [StringLength(200, ErrorMessage = "網站標題長度不能超過200個字元")]
        [Display(Name = "網站標題")]
        public string? PageTitle { get; set; }
        
        [StringLength(500, ErrorMessage = "網站關鍵字長度不能超過500個字元")]
        [Display(Name = "網站關鍵字")]
        public string? MetaKeywords { get; set; }
        
        [StringLength(1000, ErrorMessage = "網站描述長度不能超過1000個字元")]
        [Display(Name = "網站描述")]
        public string? MetaDescription { get; set; }
        
        [Display(Name = "其他Meta標籤")]
        public string? OtherMetaTags { get; set; }
        
        [StringLength(100, ErrorMessage = "Google Analytics長度不能超過100個字元")]
        [Display(Name = "Google Analytics")]
        public string? GoogleAnalytics { get; set; }
        
        [StringLength(100, ErrorMessage = "Google Tag Manager長度不能超過100個字元")]
        [Display(Name = "Google Tag Manager")]
        public string? GoogleTagManager { get; set; }
        
        [StringLength(100, ErrorMessage = "Facebook Pixel長度不能超過100個字元")]
        [Display(Name = "Facebook Pixel")]
        public string? FacebookPixel { get; set; }
        
        [Display(Name = "阻擋搜尋引擎")]
        public bool BlockSearchEngine { get; set; } = false;
        
        [Display(Name = "啟用狀態")]
        public bool IsActive { get; set; } = true;
        
        [Display(Name = "建立時間")]
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        
        [Display(Name = "更新時間")]
        public DateTime UpdatedTime { get; set; } = DateTime.Now;
    }
}
