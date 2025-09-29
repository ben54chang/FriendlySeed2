using System.ComponentModel.DataAnnotations;

namespace FriendlySeed.Models
{
    public class Menu
    {
        [Display(Name = "選單ID")]
        public int MenuID { get; set; }

        [Display(Name = "選單名稱")]
        public string MenuName { get; set; } = string.Empty;

        [Display(Name = "父選單ID")]
        public int? ParentID { get; set; }

        [Display(Name = "排序")]
        public int SortOrder { get; set; } = 10;

        [Display(Name = "是否啟用")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "圖示URL")]
        public string? IconUrl { get; set; }

        [Display(Name = "選單URL")]
        public string? MenuUrl { get; set; }

        [Display(Name = "建立時間")]
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        [Display(Name = "更新時間")]
        public DateTime UpdatedTime { get; set; } = DateTime.Now;

        // Navigation properties
        public Menu? Parent { get; set; }
        public List<Menu> Children { get; set; } = new List<Menu>();
    }
}