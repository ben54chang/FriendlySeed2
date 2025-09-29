using System.ComponentModel.DataAnnotations;

namespace FriendlySeed.Models
{
    public class Teacher
    {
        [Display(Name = "ID")]
        public int ID { get; set; }
        
        [Required(ErrorMessage = "電子信箱為必填")]
        [EmailAddress(ErrorMessage = "請輸入有效的電子信箱格式")]
        [StringLength(100, ErrorMessage = "電子信箱長度不能超過100個字元")]
        [Display(Name = "電子信箱")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "姓名為必填")]
        [StringLength(50, ErrorMessage = "姓名長度不能超過50個字元")]
        [Display(Name = "姓名")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "生日")]
        public DateTime? BirthDate { get; set; }
        
        [StringLength(10, ErrorMessage = "性別長度不能超過10個字元")]
        [Display(Name = "性別")]
        public string? Gender { get; set; }
        
        [StringLength(100, ErrorMessage = "服務單位長度不能超過100個字元")]
        [Display(Name = "服務單位")]
        public string? Organization { get; set; }
        
        [StringLength(50, ErrorMessage = "職稱長度不能超過50個字元")]
        [Display(Name = "職稱")]
        public string? Position { get; set; }
        
        [StringLength(20, ErrorMessage = "縣市長度不能超過20個字元")]
        [Display(Name = "縣市")]
        public string? City { get; set; }
        
        [StringLength(50, ErrorMessage = "鄉鎮市區長度不能超過50個字元")]
        [Display(Name = "鄉鎮市區")]
        public string? District { get; set; }
        
        [StringLength(255, ErrorMessage = "密碼長度不能超過255個字元")]
        [Display(Name = "密碼")]
        public string? Password { get; set; }
        
        [Display(Name = "建立時間")]
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        
        [Display(Name = "更新時間")]
        public DateTime UpdatedTime { get; set; } = DateTime.Now;
        
        [Display(Name = "啟用狀態")]
        public bool IsActive { get; set; } = true;
    }
}