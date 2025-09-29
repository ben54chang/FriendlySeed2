# FriendlySeed - 友善種子教育平台

這是一個基於 .NET 8.0 MVC 架構的教育管理系統，使用 MSSQL Server 資料庫、Dapper ORM，以及 Bootstrap 5 前端框架。

## 技術棧

- **後端框架**: .NET 8.0 MVC
- **資料庫**: Microsoft SQL Server
- **ORM**: Dapper
- **前端**: Bootstrap 5, jQuery, DataTables.js
- **認證**: JWT Bearer Token
- **日誌**: Serilog
- **密碼加密**: BCrypt.Net

## 功能特色

### 後台管理系統 (localhost:8054/Admin)
- 使用者管理
- 教師管理
- 文章管理
- 角色權限管理
- 選單管理
- 網站設定
- 郵件設定

### 前台展示
- 響應式設計
- 文章展示
- 教師介紹
- 關於我們

## 安裝與設定

### 1. 環境需求
- .NET 8.0 SDK
- Microsoft SQL Server 2019 或更新版本
- Visual Studio 2022 或 VS Code

### 2. 資料庫設定
1. 執行 `sql.md` 中的 TSQL 指令建立資料庫和資料表
2. 修改 `appsettings.json` 中的連線字串

### 3. 執行專案
```bash
# 還原套件
dotnet restore

# 建置專案
dotnet build

# 執行專案
dotnet run
```

### 4. 存取網址
- 前台首頁: http://localhost:8054
- 後台管理: http://localhost:8054/Admin

## 專案結構

```
FriendlySeed/
├── Areas/
│   └── Admin/                 # 後台管理區域
│       ├── Controllers/       # 後台控制器
│       └── Views/            # 後台視圖
├── Controllers/              # 前台控制器
├── Models/                   # 資料模型
├── Services/                 # 業務邏輯服務
├── Data/                     # 資料存取層
├── Views/                    # 前台視圖
├── wwwroot/                  # 靜態檔案
│   ├── css/                  # 樣式表
│   ├── js/                   # JavaScript
│   └── lib/                  # 第三方函式庫
├── appsettings.json          # 應用程式設定
└── Program.cs                # 應用程式入口點
```

## 安全性考量

- 密碼使用 BCrypt 加密
- JWT Token 認證
- 防 CSRF 攻擊
- 輸入驗證
- SQL 注入防護 (使用 Dapper 參數化查詢)

## 日誌系統

使用 Serilog 記錄應用程式日誌：
- 控制台輸出
- 檔案記錄 (Logs/log-{date}.txt)
- 保留 30 天日誌

## 開發注意事項

1. 所有資料庫欄位使用英文命名
2. 中文內容放在註解中
3. 使用繁體中文介面
4. 遵循 RESTful API 設計原則
5. 響應式設計支援各種裝置

## 授權

© 2024 FriendlySeed. All rights reserved.