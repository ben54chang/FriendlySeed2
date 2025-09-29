using System.ComponentModel.DataAnnotations;

namespace FriendlySeed.Models
{
    public class MenuRole
    {
        [Display(Name = "選單ID")]
        public int MenuID { get; set; }
        
        [Display(Name = "角色ID")]
        public int RoleID { get; set; }
        
        [Display(Name = "啟用狀態")]
        public bool IsActive { get; set; } = true;
        
        [Display(Name = "建立時間")]
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        
        [Display(Name = "更新時間")]
        public DateTime UpdatedTime { get; set; } = DateTime.Now;
        
        // Navigation properties
        public Menu? Menu { get; set; }
        public Role? Role { get; set; }
    }
}