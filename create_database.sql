-- 建立資料庫
CREATE DATABASE FriendlySeed;
GO

USE FriendlySeed;
GO

-- 1. Menu 選單表 Menu Table
CREATE TABLE Menu (
    MenuID INT IDENTITY(1,1) PRIMARY KEY, -- 選單ID
    MenuName NVARCHAR(100) NOT NULL, -- 選單名稱
    ParentID INT NULL, -- 父選單ID
    SortOrder INT DEFAULT 0, -- 排序
    IsActive BIT DEFAULT 1, -- 狀態
    CreatedTime DATETIME2 DEFAULT GETDATE(), -- 建立時間
    UpdatedTime DATETIME2 DEFAULT GETDATE(), -- 更新時間
    FOREIGN KEY (ParentID) REFERENCES Menu(MenuID)
);
GO

-- 2. Role 角色表 Role Table
CREATE TABLE Role (
    RoleID INT IDENTITY(1,1) PRIMARY KEY, -- 角色ID
    RoleName NVARCHAR(50) NOT NULL, -- 角色名稱
    IsActive BIT DEFAULT 1, -- 狀態
    CreatedTime DATETIME2 DEFAULT GETDATE(), -- 建立時間
    UpdatedTime DATETIME2 DEFAULT GETDATE() -- 更新時間
);
GO

-- 3. MenuRole 選單角色關聯表 Menu Role Association Table
CREATE TABLE MenuRole (
    MenuID INT NOT NULL, -- 選單ID
    RoleID INT NOT NULL, -- 角色ID
    IsActive BIT DEFAULT 1, -- 狀態
    CreatedTime DATETIME2 DEFAULT GETDATE(), -- 建立時間
    UpdatedTime DATETIME2 DEFAULT GETDATE(), -- 更新時間
    PRIMARY KEY (MenuID, RoleID),
    FOREIGN KEY (MenuID) REFERENCES Menu(MenuID),
    FOREIGN KEY (RoleID) REFERENCES Role(RoleID)
);
GO

-- 4. User 使用者表 User Table
CREATE TABLE [User] (
    ID INT IDENTITY(1,1) PRIMARY KEY, -- ID
    Username NVARCHAR(50) NOT NULL UNIQUE, -- 使用者名稱
    DisplayName NVARCHAR(100) NOT NULL, -- 名稱
    Avatar NVARCHAR(500) NULL, -- 頭像
    Description NVARCHAR(500) NULL, -- 描述
    RoleID INT NOT NULL, -- 角色ID
    Email NVARCHAR(100) NOT NULL, -- 電子信箱
    Password NVARCHAR(255) NOT NULL, -- 密碼
    IsActive BIT DEFAULT 1, -- 狀態
    CreatedTime DATETIME2 DEFAULT GETDATE(), -- 建立時間
    UpdatedTime DATETIME2 DEFAULT GETDATE(), -- 更新時間
    FOREIGN KEY (RoleID) REFERENCES Role(RoleID)
);
GO

-- 5. Teacher 教師表 Teacher Table
CREATE TABLE Teacher (
    ID INT IDENTITY(1,1) PRIMARY KEY, -- ID
    Email NVARCHAR(100) NOT NULL UNIQUE, -- 電子信箱
    Name NVARCHAR(50) NOT NULL, -- 姓名
    BirthDate DATE NULL, -- 生日
    Gender NVARCHAR(10) NULL, -- 性別
    Organization NVARCHAR(100) NULL, -- 服務單位
    Position NVARCHAR(50) NULL, -- 職稱
    City NVARCHAR(20) NULL, -- 縣市
    District NVARCHAR(50) NULL, -- 鄉政市區
    Password NVARCHAR(255) NOT NULL, -- 密碼
    IsActive BIT DEFAULT 1, -- 狀態
    CreatedTime DATETIME2 DEFAULT GETDATE(), -- 建立時間
    UpdatedTime DATETIME2 DEFAULT GETDATE() -- 更新時間
);
GO

-- 6. TeacherRole 教師角色表 Teacher Role Table
CREATE TABLE TeacherRole (
    TeacherRoleID INT IDENTITY(1,1) PRIMARY KEY, -- 教師角色ID
    TeacherRoleName NVARCHAR(50) NOT NULL, -- 教師角色名稱
    IsActive BIT DEFAULT 1, -- 狀態
    CreatedTime DATETIME2 DEFAULT GETDATE(), -- 建立時間
    UpdatedTime DATETIME2 DEFAULT GETDATE() -- 更新時間
);
GO

-- 7. ArticleCategory 文章分類表 Article Category Table
CREATE TABLE ArticleCategory (
    ArticleCategoryID INT IDENTITY(1,1) PRIMARY KEY, -- 文章分類ID
    ArticleCategoryName NVARCHAR(100) NOT NULL, -- 文章分類名稱
    CategoryKeywords NVARCHAR(200) NULL, -- 分類關鍵字
    CategoryDescription NVARCHAR(500) NULL, -- 分類描述
    SortOrder INT DEFAULT 0, -- 排序
    IsActive BIT DEFAULT 1, -- 狀態
    CreatedTime DATETIME2 DEFAULT GETDATE(), -- 建立時間
    UpdatedTime DATETIME2 DEFAULT GETDATE() -- 更新時間
);
GO

-- 8. Article 文章表 Article Table
CREATE TABLE Article (
    ID INT IDENTITY(1,1) PRIMARY KEY, -- ID
    Alias NVARCHAR(50) NULL, -- 代稱
    CategoryID INT NOT NULL, -- 分類
    Author NVARCHAR(100) NOT NULL, -- 作者
    Title NVARCHAR(200) NOT NULL, -- 標題
    Keywords NVARCHAR(500) NULL, -- 關鍵字
    Description NVARCHAR(1000) NULL, -- 描述
    FilePath NVARCHAR(500) NULL, -- 檔案
    SortOrder INT DEFAULT 0, -- 排序
    PublishTime DATETIME2 NULL, -- 發布時間
    IsTop BIT DEFAULT 0, -- 置頂
    Copyright NVARCHAR(200) NULL, -- 版權所有
    Address NVARCHAR(200) NULL, -- 地址
    Phone NVARCHAR(50) NULL, -- 電話
    IsActive BIT DEFAULT 1, -- 狀態
    CreatedTime DATETIME2 DEFAULT GETDATE(), -- 建立時間
    UpdatedTime DATETIME2 DEFAULT GETDATE(), -- 更新時間
    FOREIGN KEY (CategoryID) REFERENCES ArticleCategory(ArticleCategoryID)
);
GO

-- 9. 網站設定表 Website Settings Table
CREATE TABLE WebsiteSettings (
    ID INT IDENTITY(1,1) PRIMARY KEY, -- ID
    WebsiteName NVARCHAR(100) NOT NULL, -- 網站名稱
    CopyrightText NVARCHAR(500) NULL, -- 版權聲明
    ContactEmail NVARCHAR(100) NULL, -- 收件人
    Favicon NVARCHAR(500) NULL, -- 網站小圖示
    Logo NVARCHAR(500) NULL, -- 網站LOGO
    OgImage NVARCHAR(500) NULL, -- 網站OG圖片
    ContentSecurityPolicy NVARCHAR(MAX) NULL, -- Content Security Policy
    PageTitle NVARCHAR(200) NULL, -- 網站標題
    MetaKeywords NVARCHAR(500) NULL, -- 網站關鍵字
    MetaDescription NVARCHAR(1000) NULL, -- 網站描述
    OtherMetaTags NVARCHAR(MAX) NULL, -- 其他Meta標籤
    GoogleAnalytics NVARCHAR(100) NULL, -- Google Analytics
    GoogleTagManager NVARCHAR(100) NULL, -- Google Tag Manager
    FacebookPixel NVARCHAR(100) NULL, -- Facebook Pixel
    BlockSearchEngine BIT DEFAULT 0, -- 阻擋搜尋引擎
    IsActive BIT DEFAULT 1, -- 狀態
    CreatedTime DATETIME2 DEFAULT GETDATE(), -- 建立時間
    UpdatedTime DATETIME2 DEFAULT GETDATE() -- 更新時間
);
GO

-- 10. 郵件設定表 Email Settings Table
CREATE TABLE EmailSettings (
    ID INT IDENTITY(1,1) PRIMARY KEY, -- ID
    SendMethod NVARCHAR(20) NOT NULL, -- 寄信方式
    SenderEmail NVARCHAR(100) NOT NULL, -- 寄件人信箱
    SenderName NVARCHAR(100) NULL, -- 寄件人名稱
    SmtpHost NVARCHAR(100) NULL, -- SMTP主機
    SmtpPort INT NULL, -- SMTP連接埠
    EncryptionType NVARCHAR(20) NULL, -- 加密方式
    SmtpUsername NVARCHAR(100) NULL, -- SMTP帳號
    SmtpPassword NVARCHAR(255) NULL, -- SMTP密碼
    SendmailConfig NVARCHAR(500) NULL, -- Sendmail設定
    SendmailPath NVARCHAR(500) NULL, -- Sendmail路徑
    UseGmailAPI BIT DEFAULT 0, -- Gmail API
    GmailProjectID NVARCHAR(100) NULL, -- GmailProject ID
    GmailClientID NVARCHAR(100) NULL, -- GmailClient ID
    GmailClientSecret NVARCHAR(255) NULL, -- GmailClient Secret
    GmailRedirectUrl NVARCHAR(500) NULL, -- GmailRedirect Url
    ApiKey NVARCHAR(255) NULL, -- API KEY
    IsActive BIT DEFAULT 1, -- 狀態
    CreatedTime DATETIME2 DEFAULT GETDATE(), -- 建立時間
    UpdatedTime DATETIME2 DEFAULT GETDATE() -- 更新時間
);
GO

-- 建立索引以提升查詢效能 Create Indexes for Performance
CREATE INDEX IX_Menu_ParentID ON Menu(ParentID);
CREATE INDEX IX_User_Username ON [User](Username);
CREATE INDEX IX_User_Email ON [User](Email);
CREATE INDEX IX_Teacher_Email ON Teacher(Email);
CREATE INDEX IX_Article_CategoryID ON Article(CategoryID);
CREATE INDEX IX_Article_PublishTime ON Article(PublishTime);
CREATE INDEX IX_Article_IsTop ON Article(IsTop);
GO

-- 插入預設資料 Insert Default Data
-- 插入預設角色 Insert Default Roles
INSERT INTO Role (RoleName) VALUES 
('系統管理員'), -- System Administrator
('一般使用者'), -- General User
('教師'); -- Teacher

-- 插入預設選單 Insert Default Menus
INSERT INTO Menu (MenuName, ParentID, SortOrder) VALUES 
('首頁', NULL, 1), -- Home
('文章管理', NULL, 2), -- Article Management
('教師管理', NULL, 3), -- Teacher Management
('系統設定', NULL, 4), -- System Settings
('文章列表', 2, 1), -- Article List
('文章分類', 2, 2), -- Article Category
('教師列表', 3, 1), -- Teacher List
('教師角色', 3, 2), -- Teacher Role
('網站設定', 4, 1), -- Website Settings
('郵件設定', 4, 2); -- Email Settings

-- 插入預設文章分類 Insert Default Article Categories
INSERT INTO ArticleCategory (ArticleCategoryName, CategoryKeywords, CategoryDescription, SortOrder) VALUES 
('一般公告', '公告,通知', '系統一般性公告', 1), -- General Announcement
('教學資源', '教學,資源,教材', '教學相關資源', 2), -- Teaching Resources
('活動訊息', '活動,訊息,活動', '各類活動訊息', 3); -- Activity News

-- 插入預設網站設定 Insert Default Website Settings
INSERT INTO WebsiteSettings (WebsiteName, CopyrightText, PageTitle, MetaDescription) VALUES 
('FriendlySeed', '© 2024 FriendlySeed. All rights reserved.', 'FriendlySeed - 友善種子', '友善種子教育平台'); -- Friendly Seed Education Platform

-- 插入預設郵件設定 Insert Default Email Settings
INSERT INTO EmailSettings (SendMethod, SenderEmail, SenderName) VALUES 
('SMTP', 'noreply@friendlyseed.com', 'FriendlySeed 系統'); -- FriendlySeed System

-- 插入預設管理員帳號
INSERT INTO [User] (Username, DisplayName, RoleID, Email, Password) VALUES 
('admin', '系統管理員', 1, 'admin@friendlyseed.com', '$2a$11$rQZ8K9vX8K9vX8K9vX8K9e'); -- 密碼: admin123
GO
