using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FriendlySeed.Services;
using FriendlySeed.Models;

namespace FriendlySeed.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly ILogger<MenuController> _logger;

        public MenuController(IMenuService menuService, ILogger<MenuController> logger)
        {
            _menuService = menuService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var menus = await _menuService.GetAllMenusAsync();
                return View(menus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading menus");
                TempData["ErrorMessage"] = "載入選單資料時發生錯誤";
                return View(new List<Menu>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var parentMenus = await _menuService.GetAllMenusAsync();
                ViewBag.ParentMenus = parentMenus.Where(m => m.IsActive).ToList();
                return View(new Menu());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading parent menus for create");
                TempData["ErrorMessage"] = "載入父選單時發生錯誤";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult Test()
        {
            return View(new Menu());
        }

        [HttpGet]
        public IActionResult TestEdit()
        {
            return View(new Menu());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Menu model)
        {
            try
            {
                _logger.LogInformation("Create method called with MenuName: {MenuName}", model.MenuName);
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid: {Errors}", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                    TempData["ErrorMessage"] = "請檢查輸入的資料";
                    return RedirectToAction("Index");
                }

                _logger.LogInformation("Calling CreateMenuAsync for MenuName: {MenuName}", model.MenuName);
                var result = await _menuService.CreateMenuAsync(model);
                
                if (result)
                {
                    _logger.LogInformation("Menu created successfully: {MenuName}", model.MenuName);
                    TempData["SuccessMessage"] = "選單建立成功";
                }
                else
                {
                    _logger.LogWarning("Menu creation failed: {MenuName}", model.MenuName);
                    TempData["ErrorMessage"] = "選單建立失敗";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating menu: {MenuName}", model.MenuName);
                TempData["ErrorMessage"] = "建立選單時發生錯誤: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var menu = await _menuService.GetMenuByIdAsync(id);
                if (menu == null)
                {
                    TempData["ErrorMessage"] = "找不到指定的選單";
                    return RedirectToAction("Index");
                }

                var parentMenus = await _menuService.GetAllMenusAsync();
                ViewBag.ParentMenus = parentMenus.Where(m => m.IsActive && m.MenuID != id).ToList();
                return View(menu);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading menu for edit");
                TempData["ErrorMessage"] = "載入選單時發生錯誤";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Menu model)
        {
            try
            {
                _logger.LogInformation("Edit method called with MenuID: {MenuID}, MenuName: {MenuName}", model.MenuID, model.MenuName);
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid: {Errors}", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                    TempData["ErrorMessage"] = "請檢查輸入的資料";
                    return RedirectToAction("Index");
                }

                _logger.LogInformation("Calling UpdateMenuAsync for MenuID: {MenuID}, MenuName: {MenuName}", model.MenuID, model.MenuName);
                var result = await _menuService.UpdateMenuAsync(model);
                
                if (result)
                {
                    _logger.LogInformation("Menu updated successfully: MenuID {MenuID}, MenuName: {MenuName}", model.MenuID, model.MenuName);
                    TempData["SuccessMessage"] = "選單更新成功";
                }
                else
                {
                    _logger.LogWarning("Menu update failed: MenuID {MenuID}, MenuName: {MenuName}", model.MenuID, model.MenuName);
                    TempData["ErrorMessage"] = "選單更新失敗";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating menu: MenuID {MenuID}, MenuName: {MenuName}", model.MenuID, model.MenuName);
                TempData["ErrorMessage"] = "更新選單時發生錯誤: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _menuService.DeleteMenuAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "選單刪除成功";
                }
                else
                {
                    TempData["ErrorMessage"] = "選單刪除失敗";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting menu");
                TempData["ErrorMessage"] = "刪除選單時發生錯誤";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var result = await _menuService.ToggleMenuStatusAsync(id);
                if (result)
                {
                    return Json(new { success = true, message = "狀態更新成功" });
                }
                else
                {
                    return Json(new { success = false, message = "狀態更新失敗" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling menu status");
                return Json(new { success = false, message = "更新狀態時發生錯誤" });
            }
        }
    }
}
