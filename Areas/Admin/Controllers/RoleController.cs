using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FriendlySeed.Models;
using FriendlySeed.Services;

namespace FriendlySeed.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IMenuService _menuService;
        private readonly IMenuRoleService _menuRoleService;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IRoleService roleService, IMenuService menuService, IMenuRoleService menuRoleService, ILogger<RoleController> logger)
        {
            _roleService = roleService;
            _menuService = menuService;
            _menuRoleService = menuRoleService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var roles = await _roleService.GetAllRolesAsync();
                // 只取得子選單（ParentID IS NOT NULL）
                var allMenus = await _menuService.GetActiveMenusAsync();
                var childMenus = allMenus.Where(m => m.ParentID != null).OrderBy(m => m.SortOrder);
                ViewBag.Menus = childMenus;
                return View(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading roles");
                TempData["ErrorMessage"] = "載入角色資料時發生錯誤";
                return View(new List<Role>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        public async Task<IActionResult> GetRole(int id)
        {
            try
            {
                var role = await _roleService.GetRoleWithMenusAsync(id);
                if (role == null)
                {
                    return Json(new { success = false, message = "角色不存在" });
                }
                return Json(new { success = true, data = role });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role {RoleId}", id);
                return Json(new { success = false, message = "取得角色資料時發生錯誤" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Role role, List<int> selectedMenuIds)
        {
            try
            {
                Console.WriteLine("=== RoleController.Create ===");
                Console.WriteLine($"Role: RoleName={role.RoleName}, IsActive={role.IsActive}");
                Console.WriteLine($"SelectedMenuIds: {string.Join(",", selectedMenuIds ?? new List<int>())}");
                Console.WriteLine($"SelectedMenuIds Count: {selectedMenuIds?.Count ?? 0}");
                Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
                
                // 檢查 ModelState 的詳細錯誤
                if (!ModelState.IsValid)
                {
                    foreach (var key in ModelState.Keys)
                    {
                        var errors = ModelState[key].Errors;
                        if (errors.Any())
                        {
                            Console.WriteLine($"ModelState Error - {key}: {string.Join(", ", errors.Select(e => e.ErrorMessage))}");
                        }
                    }
                }
                
                if (ModelState.IsValid)
                {
                    role.CreatedTime = DateTime.Now;
                    role.UpdatedTime = DateTime.Now;
                    
                    // 使用 Store Procedure 一次性處理角色新增和選單權限設定
                    var result = await _roleService.CreateRoleWithMenusAsync(role, selectedMenuIds);
                    Console.WriteLine($"CreateRoleWithMenus result: {result}");
                    Console.WriteLine($"Role.RoleID after creation: {role.RoleID}");
                    
                    if (result)
                    {
                        Console.WriteLine("Role creation with menu permissions successful");
                        Console.WriteLine("=============================================");
                        return Json(new { success = true, message = "角色建立成功" });
                    }
                    else
                    {
                        Console.WriteLine("Role creation with menu permissions failed");
                        Console.WriteLine("==========================================");
                        return Json(new { success = false, message = "角色建立失敗" });
                    }
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    Console.WriteLine($"ModelState errors: {string.Join(", ", errors)}");
                    Console.WriteLine("========================");
                    return Json(new { success = false, message = "請檢查輸入的資料", errors = errors });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Create: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Console.WriteLine("========================");
                _logger.LogError(ex, "Error creating role");
                return Json(new { success = false, message = "建立角色時發生錯誤" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Role role, List<int> selectedMenuIds)
        {
            try
            {
                Console.WriteLine("=== RoleController.Edit ===");
                Console.WriteLine($"Role: RoleID={role.RoleID}, RoleName={role.RoleName}, IsActive={role.IsActive}");
                Console.WriteLine($"SelectedMenuIds: {string.Join(",", selectedMenuIds ?? new List<int>())}");
                Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
                
                if (ModelState.IsValid)
                {
                    role.UpdatedTime = DateTime.Now;
                    
                    var result = await _roleService.UpdateRoleWithMenusAsync(role, selectedMenuIds ?? new List<int>());
                    Console.WriteLine($"UpdateRoleWithMenus result: {result}");
                    
                    if (result)
                    {
                        Console.WriteLine("Role update successful");
                        Console.WriteLine("======================");
                        return Json(new { success = true, message = "角色更新成功" });
                    }
                    else
                    {
                        Console.WriteLine("Role update failed");
                        Console.WriteLine("==================");
                        return Json(new { success = false, message = "角色更新失敗" });
                    }
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    Console.WriteLine($"ModelState errors: {string.Join(", ", errors)}");
                    Console.WriteLine("======================");
                    return Json(new { success = false, message = "請檢查輸入的資料", errors = errors });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Edit: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Console.WriteLine("======================");
                _logger.LogError(ex, "Error updating role {RoleId}", role.RoleID);
                return Json(new { success = false, message = "更新角色時發生錯誤" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _roleService.DeleteRoleAsync(id);
                if (result)
                {
                    return Json(new { success = true, message = "角色刪除成功" });
                }
                else
                {
                    return Json(new { success = false, message = "角色刪除失敗" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {RoleId}", id);
                return Json(new { success = false, message = "刪除角色時發生錯誤" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var result = await _roleService.ToggleRoleStatusAsync(id);
                if (result)
                {
                    // 取得更新後的角色狀態
                    var role = await _roleService.GetRoleByIdAsync(id);
                    var statusText = role?.IsActive == true ? "啟用" : "停用";
                    return Json(new { success = true, message = $"角色{statusText}成功", isActive = role?.IsActive });
                }
                else
                {
                    return Json(new { success = false, message = "狀態更新失敗" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling role status {RoleId}", id);
                return Json(new { success = false, message = "更新狀態時發生錯誤" });
            }
        }
    }
}
