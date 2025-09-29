using System.ComponentModel.DataAnnotations;

namespace FriendlySeed.Models
{
    public class User
    {
        [Display(Name = "ID")]
        public int ID { get; set; }
        
        [StringLength(50, ErrorMessage = "使用者名稱長度不能超過50個字元")]
        [Display(Name = "使用者名稱")]
        public string Username { get; set; } = string.Empty;
        
        [StringLength(100, ErrorMessage = "名稱長度不能超過100個字元")]
        [Display(Name = "顯示名稱")]
        public string DisplayName { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "頭像路徑長度不能超過500個字元")]
        [Display(Name = "頭像")]
        public string? Avatar { get; set; }
        
        [StringLength(500, ErrorMessage = "描述長度不能超過500個字元")]
        [Display(Name = "描述")]
        public string? Description { get; set; }
        
        [Display(Name = "角色")]
        public string RoleID { get; set; } = string.Empty;
        
        [EmailAddress(ErrorMessage = "請輸入有效的電子信箱格式")]
        [StringLength(100, ErrorMessage = "電子信箱長度不能超過100個字元")]
        [Display(Name = "電子信箱")]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(255, ErrorMessage = "密碼長度不能超過255個字元")]
        [Display(Name = "密碼")]
        public string Password { get; set; } = string.Empty;
        
        [Display(Name = "確認密碼")]
        public string? ConfirmPassword { get; set; }
        
        [Display(Name = "啟用狀態")]
        public bool IsActive { get; set; } = true;
        
        [Display(Name = "建立時間")]
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        
        [Display(Name = "更新時間")]
        public DateTime UpdatedTime { get; set; } = DateTime.Now;
        
        // Navigation properties
        public Role? Role { get; set; }
        
        // 用於顯示多選角色名稱
        public string? RoleNames { get; set; }
    }
}
