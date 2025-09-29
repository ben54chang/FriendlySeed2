using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace FriendlySeed.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FileManagerController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _uploadPath;

        public FileManagerController(IWebHostEnvironment environment)
        {
            _environment = environment;
            _uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
        }

        public IActionResult Index(string path = "")
        {
            var currentPath = string.IsNullOrEmpty(path) ? _uploadPath : Path.Combine(_uploadPath, path);
            
            if (!Directory.Exists(currentPath))
            {
                Directory.CreateDirectory(currentPath);
            }

            var files = Directory.GetFiles(currentPath).Select(f => new FileInfo(f)).ToList();
            var directories = Directory.GetDirectories(currentPath).Select(d => new DirectoryInfo(d)).ToList();

            var model = new FileManagerViewModel
            {
                CurrentPath = path,
                Files = files,
                Directories = directories,
                ParentPath = GetParentPath(path)
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult CreateFolder(string folderName, string currentPath)
        {
            try
            {
                var targetPath = string.IsNullOrEmpty(currentPath) 
                    ? Path.Combine(_uploadPath, folderName)
                    : Path.Combine(_uploadPath, currentPath, folderName);

                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                    TempData["SuccessMessage"] = "資料夾建立成功";
                }
                else
                {
                    TempData["ErrorMessage"] = "資料夾已存在";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "建立資料夾時發生錯誤：" + ex.Message;
            }

            return RedirectToAction("Index", new { path = currentPath });
        }

        [HttpPost]
        public IActionResult UploadFile(IFormFile file, string currentPath)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    var targetPath = string.IsNullOrEmpty(currentPath) 
                        ? _uploadPath 
                        : Path.Combine(_uploadPath, currentPath);

                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(targetPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    TempData["SuccessMessage"] = "檔案上傳成功";
                }
                else
                {
                    TempData["ErrorMessage"] = "請選擇要上傳的檔案";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "上傳檔案時發生錯誤：" + ex.Message;
            }

            return RedirectToAction("Index", new { path = currentPath });
        }

        [HttpPost]
        public IActionResult DeleteFile(string fileName, string currentPath)
        {
            try
            {
                var filePath = string.IsNullOrEmpty(currentPath) 
                    ? Path.Combine(_uploadPath, fileName)
                    : Path.Combine(_uploadPath, currentPath, fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    TempData["SuccessMessage"] = "檔案刪除成功";
                }
                else
                {
                    TempData["ErrorMessage"] = "檔案不存在";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "刪除檔案時發生錯誤：" + ex.Message;
            }

            return RedirectToAction("Index", new { path = currentPath });
        }

        [HttpPost]
        public IActionResult DeleteFolder(string folderName, string currentPath)
        {
            try
            {
                var folderPath = string.IsNullOrEmpty(currentPath) 
                    ? Path.Combine(_uploadPath, folderName)
                    : Path.Combine(_uploadPath, currentPath, folderName);

                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                    TempData["SuccessMessage"] = "資料夾刪除成功";
                }
                else
                {
                    TempData["ErrorMessage"] = "資料夾不存在";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "刪除資料夾時發生錯誤：" + ex.Message;
            }

            return RedirectToAction("Index", new { path = currentPath });
        }

        private string GetParentPath(string currentPath)
        {
            if (string.IsNullOrEmpty(currentPath))
                return null;

            var parts = currentPath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length <= 1)
                return "";

            return string.Join(Path.DirectorySeparatorChar, parts.Take(parts.Length - 1));
        }
    }

    public class FileManagerViewModel
    {
        public string CurrentPath { get; set; } = string.Empty;
        public string? ParentPath { get; set; }
        public List<FileInfo> Files { get; set; } = new();
        public List<DirectoryInfo> Directories { get; set; } = new();
    }
}
