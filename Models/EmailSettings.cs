using System.ComponentModel.DataAnnotations;

namespace FriendlySeed.Models
{
    public class EmailSettings
    {
        [Display(Name = "ID")]
        public int ID { get; set; }
        
        [Required(ErrorMessage = "寄信方式為必填")]
        [StringLength(20, ErrorMessage = "寄信方式長度不能超過20個字元")]
        [Display(Name = "寄信方式")]
        public string SendMethod { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "寄件人信箱為必填")]
        [EmailAddress(ErrorMessage = "請輸入有效的電子信箱格式")]
        [StringLength(100, ErrorMessage = "寄件人信箱長度不能超過100個字元")]
        [Display(Name = "寄件人信箱")]
        public string SenderEmail { get; set; } = string.Empty;
        
        [StringLength(100, ErrorMessage = "寄件人名稱長度不能超過100個字元")]
        [Display(Name = "寄件人名稱")]
        public string? SenderName { get; set; }
        
        [StringLength(100, ErrorMessage = "SMTP主機長度不能超過100個字元")]
        [Display(Name = "SMTP主機")]
        public string? SmtpHost { get; set; }
        
        [Display(Name = "SMTP連接埠")]
        public int? SmtpPort { get; set; }
        
        [StringLength(20, ErrorMessage = "加密方式長度不能超過20個字元")]
        [Display(Name = "加密方式")]
        public string? EncryptionType { get; set; }
        
        [StringLength(100, ErrorMessage = "SMTP帳號長度不能超過100個字元")]
        [Display(Name = "SMTP帳號")]
        public string? SmtpUsername { get; set; }
        
        [StringLength(255, ErrorMessage = "SMTP密碼長度不能超過255個字元")]
        [Display(Name = "SMTP密碼")]
        public string? SmtpPassword { get; set; }
        
        [StringLength(500, ErrorMessage = "Sendmail設定長度不能超過500個字元")]
        [Display(Name = "Sendmail設定")]
        public string? SendmailConfig { get; set; }
        
        [StringLength(500, ErrorMessage = "Sendmail路徑長度不能超過500個字元")]
        [Display(Name = "Sendmail路徑")]
        public string? SendmailPath { get; set; }
        
        [Display(Name = "Gmail API")]
        public bool UseGmailAPI { get; set; } = false;
        
        [StringLength(100, ErrorMessage = "GmailProject ID長度不能超過100個字元")]
        [Display(Name = "GmailProject ID")]
        public string? GmailProjectID { get; set; }
        
        [StringLength(100, ErrorMessage = "GmailClient ID長度不能超過100個字元")]
        [Display(Name = "GmailClient ID")]
        public string? GmailClientID { get; set; }
        
        [StringLength(255, ErrorMessage = "GmailClient Secret長度不能超過255個字元")]
        [Display(Name = "GmailClient Secret")]
        public string? GmailClientSecret { get; set; }
        
        [StringLength(500, ErrorMessage = "GmailRedirect Url長度不能超過500個字元")]
        [Display(Name = "GmailRedirect Url")]
        public string? GmailRedirectUrl { get; set; }
        
        [StringLength(255, ErrorMessage = "API KEY長度不能超過255個字元")]
        [Display(Name = "API KEY")]
        public string? ApiKey { get; set; }
        
        [Display(Name = "啟用狀態")]
        public bool IsActive { get; set; } = true;
        
        [Display(Name = "建立時間")]
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        
        [Display(Name = "更新時間")]
        public DateTime UpdatedTime { get; set; } = DateTime.Now;
    }
}
