using System.ComponentModel.DataAnnotations;

namespace FriendlySeed.Models
{
    public class ErrorViewModel
    {
        [Display(Name = "狀態碼")]
        public int? StatusCode { get; set; }
        
        [Display(Name = "錯誤訊息")]
        public string? Message { get; set; }
        
        [Display(Name = "請求ID")]
        public string? RequestId { get; set; }
        
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
