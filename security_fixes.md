# 安全修復指南

## 立即修復項目

### 1. 移除敏感資訊記錄

**檔案**: `Services/RoleService.cs`

**修復前**:
```csharp
// 側錄 SQL 語法
Console.WriteLine("=== CreateRole SQL ===");
Console.WriteLine($"SQL: {sql}");
Console.WriteLine($"Parameters: RoleName={role.RoleName}, IsActive={role.IsActive}, CreatedTime={role.CreatedTime}, UpdatedTime={role.UpdatedTime}");
```

**修復後**:
```csharp
// 使用適當的日誌記錄
_logger.LogInformation("Creating role: {RoleName}", role.RoleName);
```

### 2. 改善 JWT 密鑰管理

**檔案**: `appsettings.json`

**修復前**:
```json
"JwtSettings": {
  "SecretKey": "FriendlySeed_SecretKey_2024_ThisIsAVeryLongSecretKeyForJWTTokenGeneration"
}
```

**修復後**:
```json
"JwtSettings": {
  "SecretKey": "${JWT_SECRET_KEY}"
}
```

**環境變數設定**:
```bash
# Windows
set JWT_SECRET_KEY=your-super-secure-random-key-here

# Linux/Mac
export JWT_SECRET_KEY=your-super-secure-random-key-here
```

### 3. 加入安全標頭

**檔案**: `Program.cs`

**在 `app.UseStaticFiles();` 後加入**:
```csharp
// 安全標頭
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});
```

### 4. 改善會話 Cookie 安全性

**檔案**: `Program.cs`

**修復前**:
```csharp
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
```

**修復後**:
```csharp
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Secure = true; // 僅在 HTTPS 環境下
    options.Cookie.SameSite = SameSiteMode.Strict;
});
```

### 5. 改善 Cookie 認證安全性

**檔案**: `Program.cs`

**修復前**:
```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Auth/Login";
        options.LogoutPath = "/Admin/Auth/Logout";
        options.AccessDeniedPath = "/Admin/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });
```

**修復後**:
```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Auth/Login";
        options.LogoutPath = "/Admin/Auth/Logout";
        options.AccessDeniedPath = "/Admin/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.Secure = true; // 僅在 HTTPS 環境下
        options.Cookie.SameSite = SameSiteMode.Strict;
    });
```

## 進階安全修復

### 6. 實作 Content Security Policy

**檔案**: `Program.cs`

**在安全標頭中間件中加入**:
```csharp
context.Response.Headers.Add("Content-Security-Policy", 
    "default-src 'self'; " +
    "script-src 'self' 'unsafe-inline' https://cdnjs.cloudflare.com; " +
    "style-src 'self' 'unsafe-inline' https://cdnjs.cloudflare.com; " +
    "img-src 'self' data:; " +
    "font-src 'self' https://cdnjs.cloudflare.com;");
```

### 7. 加入請求大小限制

**檔案**: `Program.cs`

**在 `builder.Services.AddControllersWithViews()` 後加入**:
```csharp
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 30 * 1024 * 1024; // 30MB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 30 * 1024 * 1024; // 30MB
});
```

### 8. 實作速率限制

**安裝套件**:
```bash
dotnet add package AspNetCoreRateLimit
```

**在 `Program.cs` 中加入**:
```csharp
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

// 在 app.UseRouting() 前加入
app.UseIpRateLimiting();
```

## 測試修復

### 1. 檢查安全標頭
```bash
curl -I http://localhost:8054
```

### 2. 檢查 JWT 密鑰
確認環境變數正確設定：
```bash
echo $JWT_SECRET_KEY
```

### 3. 檢查日誌
確認敏感資訊不再出現在日誌中。

## 注意事項

1. **HTTPS**: 在生產環境中必須使用 HTTPS
2. **環境變數**: 確保敏感資訊使用環境變數
3. **定期更新**: 定期更新套件和依賴項
4. **監控**: 實作安全監控和警報
5. **測試**: 定期進行安全測試和滲透測試

