using System.ComponentModel.DataAnnotations;

namespace FriendlySeed.Models
{
    public class TeacherRole
    {
        [Display(Name = "教師角色ID")]
        public int TeacherRoleID { get; set; }
        
        [Display(Name = "教師角色名稱")]
        public string TeacherRoleName { get; set; } = string.Empty;
        
        [Display(Name = "啟用狀態")]
        public bool IsActive { get; set; } = true;
        
        [Display(Name = "建立時間")]
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        
        [Display(Name = "更新時間")]
        public DateTime UpdatedTime { get; set; } = DateTime.Now;
    }
}
