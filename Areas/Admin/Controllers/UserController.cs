using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FriendlySeed.Models;
using FriendlySeed.Services;

namespace FriendlySeed.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, IRoleService roleService, ILogger<UserController> logger)
        {
            _userService = userService;
            _roleService = roleService;
            _logger = logger;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users");
                TempData["ErrorMessage"] = "載入使用者資料時發生錯誤";
                return View(new List<User>());
            }
        }

        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _roleService.GetAllRolesAsync();
                return Json(new { success = true, roles = roles });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading roles");
                return Json(new { success = false, message = "載入角色資料時發生錯誤" });
            }
        }

        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "使用者不存在" });
                }

                return Json(new
                {
                    success = true,
                    id = user.ID,
                    username = user.Username,
                    displayName = user.DisplayName,
                    email = user.Email,
                    roleId = user.RoleID,
                    isActive = user.IsActive,
                    // 不返回密碼，因為是加密的
                    password = ""
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {Id}", id);
                return Json(new { success = false, message = "載入使用者資料時發生錯誤" });
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            try
            {
                // 新增時將 ID 設為 0
                user.ID = 0;
                _logger.LogInformation("Create method called with Username: {Username}, RoleID: {RoleID}, Email: {Email}, Password: {PasswordLength} chars", 
                    user.Username, user.RoleID, user.Email, user.Password?.Length ?? 0);
                
                // 手動驗證所有必填欄位
                var validationErrors = new List<string>();
                
                if (string.IsNullOrEmpty(user.Username) || user.Username.Trim() == "")
                {
                    validationErrors.Add("使用者名稱為必填");
                }
                
                if (string.IsNullOrEmpty(user.DisplayName) || user.DisplayName.Trim() == "")
                {
                    validationErrors.Add("名稱為必填");
                }
                
                if (string.IsNullOrEmpty(user.Email) || user.Email.Trim() == "")
                {
                    validationErrors.Add("電子信箱為必填");
                }
                else if (!IsValidEmail(user.Email))
                {
                    validationErrors.Add("請輸入有效的電子信箱格式");
                }
                
                if (string.IsNullOrEmpty(user.RoleID) || user.RoleID.Trim() == "")
                {
                    validationErrors.Add("請選擇角色");
                }
                
                if (string.IsNullOrEmpty(user.Password) || user.Password.Trim() == "")
                {
                    validationErrors.Add("密碼為必填");
                }
                
                // 確保 IsActive 有正確的值
                if (!user.IsActive && user.IsActive != false)
                {
                    user.IsActive = true; // 預設為 true
                }
                
                // 記錄所有欄位值
                _logger.LogInformation("User data: Username={Username}, DisplayName={DisplayName}, Email={Email}, RoleID={RoleID}, Password={PasswordLength}, IsActive={IsActive}", 
                    user.Username, user.DisplayName, user.Email, user.RoleID, user.Password?.Length ?? 0, user.IsActive);
                
                if (validationErrors.Any())
                {
                    _logger.LogWarning("Validation errors for user: {Username}, Errors: {Errors}", user.Username, string.Join(", ", validationErrors));
                    return Json(new { success = false, message = "請檢查輸入的資料: " + string.Join(", ", validationErrors) });
                }
                
                // 記錄 ModelState 狀態（僅用於除錯）
                _logger.LogInformation("ModelState.IsValid: {IsValid}", ModelState.IsValid);
                
                // 由於我們已經做了手動驗證，這裡只記錄 ModelState 錯誤（如果有）
                if (!ModelState.IsValid)
                {
                    var modelStateErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogWarning("ModelState errors (ignored): {Errors}", string.Join(", ", modelStateErrors));
                }

                _logger.LogInformation("Calling CreateUserAsync for Username: {Username}", user.Username);
                var result = await _userService.CreateUserAsync(user);
                
                if (result)
                {
                    _logger.LogInformation("User created successfully: {Username}", user.Username);
                    return Json(new { success = true, message = "使用者建立成功" });
                }
                else
                {
                    _logger.LogWarning("User creation failed: {Username}", user.Username);
                    return Json(new { success = false, message = "使用者建立失敗" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {Username}", user.Username);
                return Json(new { success = false, message = "建立使用者時發生錯誤: " + ex.Message });
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _roleService.GetAllRolesAsync();
            ViewBag.Roles = roles;
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User user)
        {
            try
            {
                _logger.LogInformation("Edit method called with ID: {ID}, Username: {Username}, IsActive: {IsActive}", user.ID, user.Username, user.IsActive);
                
                // 手動驗證所有必填欄位
                var validationErrors = new List<string>();
                
                if (string.IsNullOrEmpty(user.Username) || user.Username.Trim() == "")
                {
                    validationErrors.Add("使用者名稱為必填");
                }
                
                if (string.IsNullOrEmpty(user.DisplayName) || user.DisplayName.Trim() == "")
                {
                    validationErrors.Add("名稱為必填");
                }
                
                if (string.IsNullOrEmpty(user.Email) || user.Email.Trim() == "")
                {
                    validationErrors.Add("電子信箱為必填");
                }
                else if (!IsValidEmail(user.Email))
                {
                    validationErrors.Add("請輸入有效的電子信箱格式");
                }
                
                if (string.IsNullOrEmpty(user.RoleID) || user.RoleID.Trim() == "")
                {
                    validationErrors.Add("請選擇角色");
                }
                
                // 確保 IsActive 有正確的值
                if (!user.IsActive && user.IsActive != false)
                {
                    user.IsActive = true; // 預設為 true
                }
                
                if (validationErrors.Any())
                {
                    _logger.LogWarning("Validation errors for user edit: {Username}, Errors: {Errors}", user.Username, string.Join(", ", validationErrors));
                    return Json(new { success = false, message = "請檢查輸入的資料: " + string.Join(", ", validationErrors) });
                }

                _logger.LogInformation("Calling UpdateUserAsync for ID: {ID}, Username: {Username}", user.ID, user.Username);
                var result = await _userService.UpdateUserAsync(user);
                
                if (result)
                {
                    _logger.LogInformation("User updated successfully: ID {ID}, Username: {Username}", user.ID, user.Username);
                    return Json(new { success = true, message = "使用者更新成功" });
                }
                else
                {
                    _logger.LogWarning("User update failed: ID {ID}, Username: {Username}", user.ID, user.Username);
                    return Json(new { success = false, message = "使用者更新失敗" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: ID {ID}, Username: {Username}", user.ID, user.Username);
                return Json(new { success = false, message = "更新使用者時發生錯誤: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var result = await _userService.ToggleUserStatusAsync(id);
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
                _logger.LogError(ex, "Error toggling user status");
                return Json(new { success = false, message = "更新狀態時發生錯誤" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(int id)
        {
            try
            {
                // 產生 6 位數字亂數密碼
                var random = new Random();
                var newPassword = random.Next(100000, 999999).ToString();
                
                _logger.LogInformation("Generating new password for user {Id}: {Password}", id, newPassword);

                var result = await _userService.ChangePasswordAsync(id, newPassword);
                if (result)
                {
                    return Json(new { 
                        success = true, 
                        message = "密碼變更成功", 
                        newPassword = newPassword 
                    });
                }
                else
                {
                    return Json(new { success = false, message = "密碼變更失敗" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {Id}", id);
                return Json(new { success = false, message = "變更密碼時發生錯誤" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (result)
                {
                    return Json(new { success = true, message = "使用者刪除成功" });
                }
                else
                {
                    return Json(new { success = false, message = "使用者刪除失敗" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return Json(new { success = false, message = "刪除使用者時發生錯誤" });
            }
        }
    }
}


