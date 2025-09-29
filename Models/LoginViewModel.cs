using System.ComponentModel.DataAnnotations;

namespace FriendlySeed.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "使用者名稱為必填")]
        [Display(Name = "使用者名稱")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "密碼為必填")]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "記住我")]
        public bool RememberMe { get; set; }
    }
}
