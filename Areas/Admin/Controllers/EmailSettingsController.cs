using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FriendlySeed.Services;
using FriendlySeed.Models;

namespace FriendlySeed.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class EmailSettingsController : Controller
    {
        private readonly IEmailSettingsService _emailSettingsService;
        private readonly ILogger<EmailSettingsController> _logger;

        public EmailSettingsController(IEmailSettingsService emailSettingsService, ILogger<EmailSettingsController> logger)
        {
            _emailSettingsService = emailSettingsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var settings = await _emailSettingsService.GetEmailSettingsAsync();
                if (settings == null)
                {
                    // 如果沒有設定，建立預設設定
                    settings = new EmailSettings
                    {
                        SendMethod = "SMTP",
                        SenderEmail = "noreply@friendlyseed.com",
                        SenderName = "FriendlySeed 系統",
                        SmtpHost = "smtp.gmail.com",
                        SmtpPort = 587,
                        EncryptionType = "TLS",
                        SmtpUsername = "your-email@gmail.com",
                        SmtpPassword = "your-app-password",
                        IsActive = true
                    };
                }
                return View(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading email settings");
                TempData["ErrorMessage"] = "載入郵件設定時發生錯誤";
                return View(new EmailSettings());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(EmailSettings model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _emailSettingsService.UpdateEmailSettingsAsync(model);
                if (result)
                {
                    TempData["SuccessMessage"] = "郵件設定已成功更新";
                }
                else
                {
                    TempData["ErrorMessage"] = "更新郵件設定失敗";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating email settings");
                TempData["ErrorMessage"] = "更新郵件設定時發生錯誤";
            }

            return View(model);
        }
    }
}
