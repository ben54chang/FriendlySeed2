using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FriendlySeed.Services;
using FriendlySeed.Models;

namespace FriendlySeed.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class WebsiteSettingsController : Controller
    {
        private readonly IWebsiteSettingsService _websiteSettingsService;
        private readonly ILogger<WebsiteSettingsController> _logger;

        public WebsiteSettingsController(IWebsiteSettingsService websiteSettingsService, ILogger<WebsiteSettingsController> logger)
        {
            _websiteSettingsService = websiteSettingsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var settings = await _websiteSettingsService.GetWebsiteSettingsAsync();
                if (settings == null)
                {
                    // 如果沒有設定，建立預設設定
                    settings = new WebsiteSettings
                    {
                        WebsiteName = "FriendlySeed",
                        MetaDescription = "友善種子教育平台",
                        MetaKeywords = "教育,學習,友善,種子",
                        ContactEmail = "contact@friendlyseed.com",
                        CopyrightText = "© 2024 FriendlySeed. All rights reserved.",
                        IsActive = true
                    };
                }
                return View(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading website settings");
                TempData["ErrorMessage"] = "載入網站設定時發生錯誤";
                return View(new WebsiteSettings());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(WebsiteSettings model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _websiteSettingsService.UpdateWebsiteSettingsAsync(model);
                if (result)
                {
                    TempData["SuccessMessage"] = "網站設定已成功更新";
                }
                else
                {
                    TempData["ErrorMessage"] = "更新網站設定失敗";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating website settings");
                TempData["ErrorMessage"] = "更新網站設定時發生錯誤";
            }

            return View(model);
        }
    }
}
