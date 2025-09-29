using System.ComponentModel.DataAnnotations;

namespace FriendlySeed.Models
{
    public class Role
    {
        [Display(Name = "角色ID")]
        public int RoleID { get; set; }
        
        [Display(Name = "角色名稱")]
        public string RoleName { get; set; } = string.Empty;
        
        [Display(Name = "啟用狀態")]
        public bool IsActive { get; set; } = true;
        
        [Display(Name = "建立時間")]
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        
        [Display(Name = "更新時間")]
        public DateTime UpdatedTime { get; set; } = DateTime.Now;
        
        // Navigation properties
        public List<User>? Users { get; set; }
        public List<MenuRole>? MenuRoles { get; set; }
        
        // 用於編輯時的選單權限選擇
        public List<int>? SelectedMenuIds { get; set; } = new List<int>();
        public List<Menu>? AvailableMenus { get; set; } = new List<Menu>();
    }
}
