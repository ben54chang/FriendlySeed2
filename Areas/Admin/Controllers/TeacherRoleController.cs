using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FriendlySeed.Services;
using FriendlySeed.Models;

namespace FriendlySeed.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class TeacherRoleController : Controller
    {
        private readonly ITeacherRoleService _teacherRoleService;
        private readonly ILogger<TeacherRoleController> _logger;

        public TeacherRoleController(ITeacherRoleService teacherRoleService, ILogger<TeacherRoleController> logger)
        {
            _teacherRoleService = teacherRoleService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Starting to load teacher roles");
                var teacherRoles = await _teacherRoleService.GetAllTeacherRolesAsync();
                _logger.LogInformation("Successfully loaded {Count} teacher roles", teacherRoles.Count());
                return View(teacherRoles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading teacher roles: {Message}", ex.Message);
                _logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
                TempData["ErrorMessage"] = $"載入教師角色資料時發生錯誤: {ex.Message}";
                return View(new List<TeacherRole>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeacherRole model)
        {
            try
            {
                // 新增時將 TeacherRoleID 設為 0
                model.TeacherRoleID = 0;
                _logger.LogInformation("Create method called with TeacherRoleName: {TeacherRoleName}", model.TeacherRoleName);
                
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    var modelErrors = ModelState.Where(x => x.Value.Errors.Count > 0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
                    _logger.LogWarning("ModelState is invalid: {Errors}", string.Join(", ", errors));
                    _logger.LogWarning("ModelState details: {ModelErrors}", string.Join(", ", modelErrors.Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value)}")));
                    _logger.LogWarning("Model values: TeacherRoleName={TeacherRoleName}, IsActive={IsActive}", 
                        model.TeacherRoleName, model.IsActive);
                    TempData["ErrorMessage"] = "請檢查輸入的資料: " + string.Join(", ", errors);
                    return RedirectToAction("Index");
                }

                _logger.LogInformation("Calling CreateTeacherRoleAsync for TeacherRoleName: {TeacherRoleName}", model.TeacherRoleName);
                var result = await _teacherRoleService.CreateTeacherRoleAsync(model);
                
                if (result)
                {
                    _logger.LogInformation("TeacherRole created successfully: {TeacherRoleName}", model.TeacherRoleName);
                    TempData["SuccessMessage"] = "教師角色建立成功";
                }
                else
                {
                    _logger.LogWarning("TeacherRole creation failed: {TeacherRoleName}", model.TeacherRoleName);
                    TempData["ErrorMessage"] = "教師角色建立失敗";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating teacher role: {TeacherRoleName}", model.TeacherRoleName);
                TempData["ErrorMessage"] = "建立教師角色時發生錯誤: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TeacherRole model)
        {
            try
            {
                _logger.LogInformation("Edit method called with TeacherRoleID: {TeacherRoleID}, TeacherRoleName: {TeacherRoleName}", model.TeacherRoleID, model.TeacherRoleName);
                
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    var modelErrors = ModelState.Where(x => x.Value.Errors.Count > 0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
                    _logger.LogWarning("ModelState is invalid: {Errors}", string.Join(", ", errors));
                    _logger.LogWarning("ModelState details: {ModelErrors}", string.Join(", ", modelErrors.Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value)}")));
                    _logger.LogWarning("Model values: TeacherRoleName={TeacherRoleName}, IsActive={IsActive}", 
                        model.TeacherRoleName, model.IsActive);
                    TempData["ErrorMessage"] = "請檢查輸入的資料: " + string.Join(", ", errors);
                    return RedirectToAction("Index");
                }

                _logger.LogInformation("Calling UpdateTeacherRoleAsync for TeacherRoleID: {TeacherRoleID}, TeacherRoleName: {TeacherRoleName}", model.TeacherRoleID, model.TeacherRoleName);
                var result = await _teacherRoleService.UpdateTeacherRoleAsync(model);
                
                if (result)
                {
                    _logger.LogInformation("TeacherRole updated successfully: TeacherRoleID {TeacherRoleID}, TeacherRoleName: {TeacherRoleName}", model.TeacherRoleID, model.TeacherRoleName);
                    TempData["SuccessMessage"] = "教師角色更新成功";
                }
                else
                {
                    _logger.LogWarning("TeacherRole update failed: TeacherRoleID {TeacherRoleID}, TeacherRoleName: {TeacherRoleName}", model.TeacherRoleID, model.TeacherRoleName);
                    TempData["ErrorMessage"] = "教師角色更新失敗";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating teacher role: TeacherRoleID {TeacherRoleID}, TeacherRoleName: {TeacherRoleName}", model.TeacherRoleID, model.TeacherRoleName);
                TempData["ErrorMessage"] = "更新教師角色時發生錯誤: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var result = await _teacherRoleService.ToggleTeacherRoleStatusAsync(id);
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
                _logger.LogError(ex, "Error toggling teacher role status");
                return Json(new { success = false, message = "更新狀態時發生錯誤" });
            }
        }
    }
}

