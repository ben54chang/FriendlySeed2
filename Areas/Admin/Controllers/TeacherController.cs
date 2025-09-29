using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FriendlySeed.Models;
using FriendlySeed.Services;

namespace FriendlySeed.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class TeacherController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly ILogger<TeacherController> _logger;

        public TeacherController(ITeacherService teacherService, ILogger<TeacherController> logger)
        {
            _teacherService = teacherService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var teachers = await _teacherService.GetAllTeachersAsync();
                return View(teachers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading teachers");
                TempData["ErrorMessage"] = "載入教師資料時發生錯誤";
                return View(new List<Teacher>());
            }
        }

        public async Task<IActionResult> GetTeacher(int id)
        {
            try
            {
                var teacher = await _teacherService.GetTeacherByIdAsync(id);
                if (teacher == null)
                {
                    return Json(new { success = false, message = "教師不存在" });
                }

                return Json(new
                {
                    success = true,
                    id = teacher.ID,
                    email = teacher.Email,
                    name = teacher.Name,
                    birthDate = teacher.BirthDate?.ToString("yyyy-MM-dd"),
                    gender = teacher.Gender,
                    organization = teacher.Organization,
                    position = teacher.Position,
                    city = teacher.City,
                    district = teacher.District,
                    password = teacher.Password,
                    isActive = teacher.IsActive
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting teacher {Id}", id);
                return Json(new { success = false, message = "載入教師資料時發生錯誤" });
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teacher teacher)
        {
            try
            {
                // 新增時將 ID 設為 0
                teacher.ID = 0;
                _logger.LogInformation("Create method called with Name: {Name}, IsActive: {IsActive}", teacher.Name, teacher.IsActive);
                
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogWarning("ModelState is invalid: {Errors}", string.Join(", ", errors));
                    return Json(new { success = false, message = "請檢查輸入的資料: " + string.Join(", ", errors) });
                }

                _logger.LogInformation("Calling CreateTeacherAsync for Name: {Name}", teacher.Name);
                var result = await _teacherService.CreateTeacherAsync(teacher);
                
                if (result)
                {
                    _logger.LogInformation("Teacher created successfully: {Name}", teacher.Name);
                    return Json(new { success = true, message = "教師建立成功" });
                }
                else
                {
                    _logger.LogWarning("Teacher creation failed: {Name}", teacher.Name);
                    return Json(new { success = false, message = "教師建立失敗" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating teacher: {Name}", teacher.Name);
                return Json(new { success = false, message = "建立教師時發生錯誤: " + ex.Message });
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            return View(teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Teacher teacher)
        {
            try
            {
                _logger.LogInformation("Edit method called with ID: {ID}, Name: {Name}", teacher.ID, teacher.Name);
                
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogWarning("ModelState is invalid: {Errors}", string.Join(", ", errors));
                    return Json(new { success = false, message = "請檢查輸入的資料: " + string.Join(", ", errors) });
                }

                _logger.LogInformation("Calling UpdateTeacherAsync for ID: {ID}, Name: {Name}", teacher.ID, teacher.Name);
                var result = await _teacherService.UpdateTeacherAsync(teacher);
                
                if (result)
                {
                    _logger.LogInformation("Teacher updated successfully: ID {ID}, Name: {Name}", teacher.ID, teacher.Name);
                    return Json(new { success = true, message = "教師更新成功" });
                }
                else
                {
                    _logger.LogWarning("Teacher update failed: ID {ID}, Name: {Name}", teacher.ID, teacher.Name);
                    return Json(new { success = false, message = "教師更新失敗" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating teacher: ID {ID}, Name: {Name}", teacher.ID, teacher.Name);
                return Json(new { success = false, message = "更新教師時發生錯誤: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var result = await _teacherService.ToggleTeacherStatusAsync(id);
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
                _logger.LogError(ex, "Error toggling teacher status");
                return Json(new { success = false, message = "更新狀態時發生錯誤" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _teacherService.DeleteTeacherAsync(id);
                if (success)
                {
                    return Json(new { success = true, message = "教師刪除成功" });
                }
                else
                {
                    return Json(new { success = false, message = "刪除教師時發生錯誤" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting teacher");
                return Json(new { success = false, message = "刪除教師時發生錯誤" });
            }
        }
    }
}
