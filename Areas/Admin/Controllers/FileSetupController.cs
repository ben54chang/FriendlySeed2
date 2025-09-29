using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Text;

namespace FriendlySeed.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class FileSetupController : Controller
    {
        private readonly string _basePath;
        private readonly ILogger<FileSetupController> _logger;

        public FileSetupController(ILogger<FileSetupController> logger)
        {
            _logger = logger;
            _basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "FileSetup");
            
            // 確保基礎目錄存在
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Tree(string id = "#")
        {
            try
            {
                var result = new List<object>();
                
                if (id == "#")
                {
                    // 根目錄
                    var rootDir = new DirectoryInfo(_basePath);
                    if (rootDir.Exists)
                    {
                        var subDirs = rootDir.GetDirectories()
                            .Where(d => !d.Name.StartsWith("."))
                            .OrderBy(d => d.Name)
                            .Select(d => new
                            {
                                id = d.Name,
                                text = d.Name,
                                children = d.GetDirectories().Length > 0
                            });
                        result.AddRange(subDirs);
                    }
                }
                else
                {
                    // 子目錄
                    var targetPath = Path.Combine(_basePath, id);
                    var dir = new DirectoryInfo(targetPath);
                    if (dir.Exists)
                    {
                        var subDirs = dir.GetDirectories()
                            .Where(d => !d.Name.StartsWith("."))
                            .OrderBy(d => d.Name)
                            .Select(d => new
                            {
                                id = Path.Combine(id, d.Name).Replace("\\", "/"),
                                text = d.Name,
                                children = d.GetDirectories().Length > 0
                            });
                        result.AddRange(subDirs);
                    }
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading tree for id: {Id}", id);
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public IActionResult List(string path = "")
        {
            try
            {
                var targetPath = string.IsNullOrEmpty(path) ? _basePath : Path.Combine(_basePath, path);
                var dir = new DirectoryInfo(targetPath);
                
                if (!dir.Exists)
                {
                    return Json(new { success = false, message = "目錄不存在" });
                }

                var directories = dir.GetDirectories()
                    .Where(d => !d.Name.StartsWith("."))
                    .OrderBy(d => d.Name)
                    .Select(d => new
                    {
                        name = d.Name,
                        path = Path.Combine(path, d.Name).Replace("\\", "/"),
                        modified = d.LastWriteTime
                    });

                var files = dir.GetFiles()
                    .Where(f => !f.Name.StartsWith("."))
                    .OrderBy(f => f.Name)
                    .Select(f => new
                    {
                        name = f.Name,
                        path = Path.Combine(path, f.Name).Replace("\\", "/"),
                        size = f.Length,
                        modified = f.LastWriteTime
                    });

                return Json(new
                {
                    success = true,
                    directories = directories,
                    files = files
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing directory: {Path}", path);
                return Json(new { success = false, message = "讀取目錄失敗" });
            }
        }

        [HttpPost]
        public IActionResult CreateFolder(string path, string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return Json(new { success = false, message = "資料夾名稱不能為空" });
                }

                var targetPath = string.IsNullOrEmpty(path) ? _basePath : Path.Combine(_basePath, path);
                var newFolderPath = Path.Combine(targetPath, name);

                if (Directory.Exists(newFolderPath))
                {
                    return Json(new { success = false, message = "資料夾已存在" });
                }

                Directory.CreateDirectory(newFolderPath);
                _logger.LogInformation("Created folder: {Path}", newFolderPath);

                return Json(new { success = true, message = "資料夾建立成功" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating folder: {Path}/{Name}", path, name);
                return Json(new { success = false, message = "建立資料夾失敗" });
            }
        }

        [HttpPost]
        public IActionResult CreateFile(string path, string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return Json(new { success = false, message = "檔案名稱不能為空" });
                }

                var targetPath = string.IsNullOrEmpty(path) ? _basePath : Path.Combine(_basePath, path);
                var newFilePath = Path.Combine(targetPath, name);

                if (System.IO.File.Exists(newFilePath))
                {
                    return Json(new { success = false, message = "檔案已存在" });
                }

                System.IO.File.WriteAllText(newFilePath, "");
                _logger.LogInformation("Created file: {Path}", newFilePath);

                return Json(new { success = true, message = "檔案建立成功" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating file: {Path}/{Name}", path, name);
                return Json(new { success = false, message = "建立檔案失敗" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upload(string path, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return Json(new { success = false, message = "請選擇要上傳的檔案" });
                }

                // 檢查檔案大小 (50MB)
                if (file.Length > 50 * 1024 * 1024)
                {
                    return Json(new { success = false, message = "檔案大小不能超過 50MB" });
                }

                var targetPath = string.IsNullOrEmpty(path) ? _basePath : Path.Combine(_basePath, path);
                var filePath = Path.Combine(targetPath, file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("Uploaded file: {Path}", filePath);
                return Json(new { success = true, message = "檔案上傳成功" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file: {FileName}", file?.FileName);
                return Json(new { success = false, message = "檔案上傳失敗" });
            }
        }

        [HttpPost]
        public IActionResult Delete(string path, bool isDirectory)
        {
            try
            {
                var targetPath = Path.Combine(_basePath, path);

                if (isDirectory)
                {
                    if (Directory.Exists(targetPath))
                    {
                        Directory.Delete(targetPath, true);
                        _logger.LogInformation("Deleted directory: {Path}", targetPath);
                    }
                }
                else
                {
                    if (System.IO.File.Exists(targetPath))
                    {
                        System.IO.File.Delete(targetPath);
                        _logger.LogInformation("Deleted file: {Path}", targetPath);
                    }
                }

                return Json(new { success = true, message = "刪除成功" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting: {Path}", path);
                return Json(new { success = false, message = "刪除失敗" });
            }
        }

        [HttpGet]
        public IActionResult Download(string path)
        {
            try
            {
                var filePath = Path.Combine(_basePath, path);
                
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound();
                }

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var fileName = Path.GetFileName(filePath);
                
                return File(fileBytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file: {Path}", path);
                return NotFound();
            }
        }

        [HttpGet]
        public IActionResult Preview(string path)
        {
            try
            {
                var filePath = Path.Combine(_basePath, path);
                
                if (!System.IO.File.Exists(filePath))
                {
                    return Json(new { success = false, message = "檔案不存在" });
                }

                var extension = Path.GetExtension(filePath).ToLowerInvariant();
                var contentType = GetContentType(extension);

                // 圖片檔案直接返回
                if (contentType.StartsWith("image/"))
                {
                    var fileBytes = System.IO.File.ReadAllBytes(filePath);
                    return File(fileBytes, contentType);
                }

                // 文字檔案返回內容
                if (IsTextFile(extension))
                {
                    var content = System.IO.File.ReadAllText(filePath, Encoding.UTF8);
                    return Json(new
                    {
                        success = true,
                        kind = "text",
                        content = content
                    });
                }

                return Json(new { success = false, message = "不支援預覽此檔案類型" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing file: {Path}", path);
                return Json(new { success = false, message = "預覽失敗" });
            }
        }

        [HttpPost]
        public IActionResult Rename(string path, string newName)
        {
            try
            {
                if (string.IsNullOrEmpty(newName))
                {
                    return Json(new { success = false, message = "新名稱不能為空" });
                }

                var oldPath = Path.Combine(_basePath, path);
                var newPath = Path.Combine(Path.GetDirectoryName(oldPath)!, newName);

                if (Directory.Exists(oldPath))
                {
                    Directory.Move(oldPath, newPath);
                }
                else if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Move(oldPath, newPath);
                }
                else
                {
                    return Json(new { success = false, message = "檔案或資料夾不存在" });
                }

                _logger.LogInformation("Renamed: {OldPath} -> {NewPath}", oldPath, newPath);
                return Json(new { success = true, message = "重新命名成功" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error renaming: {Path} -> {NewName}", path, newName);
                return Json(new { success = false, message = "重新命名失敗" });
            }
        }

        [HttpPost]
        public IActionResult Move(string sourcePath, string targetDir)
        {
            try
            {
                var sourceFullPath = Path.Combine(_basePath, sourcePath);
                var targetFullPath = Path.Combine(_basePath, targetDir);

                if (!Directory.Exists(targetFullPath))
                {
                    return Json(new { success = false, message = "目的目錄不存在" });
                }

                var fileName = Path.GetFileName(sourceFullPath);
                var newPath = Path.Combine(targetFullPath, fileName);

                if (Directory.Exists(sourceFullPath))
                {
                    Directory.Move(sourceFullPath, newPath);
                }
                else if (System.IO.File.Exists(sourceFullPath))
                {
                    System.IO.File.Move(sourceFullPath, newPath);
                }
                else
                {
                    return Json(new { success = false, message = "來源檔案或資料夾不存在" });
                }

                _logger.LogInformation("Moved: {SourcePath} -> {NewPath}", sourceFullPath, newPath);
                return Json(new { success = true, message = "搬移成功" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moving: {SourcePath} -> {TargetDir}", sourcePath, targetDir);
                return Json(new { success = false, message = "搬移失敗" });
            }
        }

        [HttpPost]
        public IActionResult Copy(string sourcePath, string targetDir)
        {
            try
            {
                var sourceFullPath = Path.Combine(_basePath, sourcePath);
                var targetFullPath = Path.Combine(_basePath, targetDir);

                if (!Directory.Exists(targetFullPath))
                {
                    return Json(new { success = false, message = "目的目錄不存在" });
                }

                var fileName = Path.GetFileName(sourceFullPath);
                var newPath = Path.Combine(targetFullPath, fileName);

                if (Directory.Exists(sourceFullPath))
                {
                    CopyDirectory(sourceFullPath, newPath);
                }
                else if (System.IO.File.Exists(sourceFullPath))
                {
                    System.IO.File.Copy(sourceFullPath, newPath, true);
                }
                else
                {
                    return Json(new { success = false, message = "來源檔案或資料夾不存在" });
                }

                _logger.LogInformation("Copied: {SourcePath} -> {NewPath}", sourceFullPath, newPath);
                return Json(new { success = true, message = "複製成功" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error copying: {SourcePath} -> {TargetDir}", sourcePath, targetDir);
                return Json(new { success = false, message = "複製失敗" });
            }
        }

        private string GetContentType(string extension)
        {
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".txt" => "text/plain",
                ".html" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".json" => "application/json",
                ".xml" => "application/xml",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };
        }

        private bool IsTextFile(string extension)
        {
            var textExtensions = new[] { ".txt", ".html", ".css", ".js", ".json", ".xml", ".md", ".csv", ".log" };
            return textExtensions.Contains(extension);
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            var dir = new DirectoryInfo(sourceDir);
            var dirs = dir.GetDirectories();

            Directory.CreateDirectory(targetDir);

            foreach (var file in dir.GetFiles())
            {
                var targetFilePath = Path.Combine(targetDir, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            foreach (var subDir in dirs)
            {
                var newTargetDir = Path.Combine(targetDir, subDir.Name);
                CopyDirectory(subDir.FullName, newTargetDir);
            }
        }
    }
}
