-- 建立 FriendlySeed 資料庫和最高權限管理員帳號
-- 請在 SQL Server Management Studio 或 sqlcmd 中執行此腳本

-- 1. 建立資料庫
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'FriendlySeed')
BEGIN
    CREATE DATABASE FriendlySeed;
    PRINT '資料庫 FriendlySeed 建立成功';
END
ELSE
BEGIN
    PRINT '資料庫 FriendlySeed 已存在';
END

-- 2. 使用資料庫
USE FriendlySeed;

-- 3. 建立所有必要的資料表
-- Menu 選單表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Menu' AND xtype='U')
BEGIN
    CREATE TABLE Menu (
        MenuID INT IDENTITY(1,1) PRIMARY KEY,
        MenuName NVARCHAR(100) NOT NULL,
        ParentID INT NULL,
        SortOrder INT DEFAULT 0,
        IsActive BIT DEFAULT 1,
        CreatedTime DATETIME2 DEFAULT GETDATE(),
        UpdatedTime DATETIME2 DEFAULT GETDATE(),
        FOREIGN KEY (ParentID) REFERENCES Menu(MenuID)
    );
    PRINT 'Menu 資料表建立成功';
END

-- Role 角色表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Role' AND xtype='U')
BEGIN
    CREATE TABLE Role (
        RoleID INT IDENTITY(1,1) PRIMARY KEY,
        RoleName NVARCHAR(50) NOT NULL,
        IsActive BIT DEFAULT 1,
        CreatedTime DATETIME2 DEFAULT GETDATE(),
        UpdatedTime DATETIME2 DEFAULT GETDATE()
    );
    PRINT 'Role 資料表建立成功';
END

-- User 使用者表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='User' AND xtype='U')
BEGIN
    CREATE TABLE [User] (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL UNIQUE,
        DisplayName NVARCHAR(100) NOT NULL,
        Avatar NVARCHAR(500) NULL,
        Description NVARCHAR(500) NULL,
        RoleID INT NOT NULL,
        Email NVARCHAR(100) NOT NULL,
        Password NVARCHAR(255) NOT NULL,
        IsActive BIT DEFAULT 1,
        CreatedTime DATETIME2 DEFAULT GETDATE(),
        UpdatedTime DATETIME2 DEFAULT GETDATE(),
        FOREIGN KEY (RoleID) REFERENCES Role(RoleID)
    );
    PRINT 'User 資料表建立成功';
END

-- 4. 插入預設角色
IF NOT EXISTS (SELECT 1 FROM Role WHERE RoleName = '系統管理員')
BEGIN
    INSERT INTO Role (RoleName, IsActive) VALUES ('系統管理員', 1);
    PRINT '系統管理員角色建立成功';
END

-- 5. 建立最高權限管理員帳號
IF NOT EXISTS (SELECT 1 FROM [User] WHERE Username = 'admin')
BEGIN
    INSERT INTO [User] (
        Username, 
        DisplayName, 
        RoleID, 
        Email, 
        Password, 
        IsActive
    ) VALUES (
        'admin',                                    -- 使用者名稱
        '系統管理員',                               -- 顯示名稱
        1,                                          -- 角色ID (系統管理員)
        'admin@friendlyseed.com',                   -- 電子信箱
        '$2a$11$rQZ8K9vX8K9vX8K9vX8K9e',          -- 密碼: admin123 (BCrypt加密)
        1                                           -- 啟用狀態
    );
    PRINT '管理員帳號建立成功！';
END
ELSE
BEGIN
    PRINT '管理員帳號已存在！';
END

-- 6. 顯示管理員帳號資訊
PRINT '';
PRINT '=== 管理員帳號資訊 ===';
PRINT '使用者名稱: admin';
PRINT '密碼: admin123';
PRINT '電子信箱: admin@friendlyseed.com';
PRINT '角色: 系統管理員';
PRINT '';

-- 7. 顯示所有使用者
SELECT 
    ID as 'ID',
    Username as '使用者名稱',
    DisplayName as '顯示名稱',
    Email as '電子信箱',
    r.RoleName as '角色',
    IsActive as '啟用狀態',
    CreatedTime as '建立時間'
FROM [User] u
LEFT JOIN Role r ON u.RoleID = r.RoleID
ORDER BY u.ID;

PRINT '';
PRINT '資料庫設定完成！請使用 admin/admin123 登入後台系統。';
