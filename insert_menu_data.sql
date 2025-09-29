-- =============================================
-- 插入基本菜單資料
-- =============================================

USE [FriendlySeed]
GO

-- 清除現有菜單資料
DELETE FROM [Menu];
GO

-- 插入根菜單項目
INSERT INTO [Menu] (MenuName, ParentID, SortOrder, IsActive, IconUrl, MenuUrl) VALUES
('首頁', NULL, 1, 1, 'fas fa-home', '/Admin/Home'),
('文章管理', NULL, 2, 1, 'fas fa-newspaper', NULL),
('系統管理', NULL, 3, 1, 'fas fa-cogs', NULL),
('教師管理', NULL, 4, 1, 'fas fa-chalkboard-teacher', NULL),
('網站資料', NULL, 5, 1, 'fas fa-database', NULL);

-- 獲取剛插入的菜單ID
DECLARE @ArticleMenuID INT = (SELECT MenuID FROM [Menu] WHERE MenuName = '文章管理');
DECLARE @SystemMenuID INT = (SELECT MenuID FROM [Menu] WHERE MenuName = '系統管理');
DECLARE @TeacherMenuID INT = (SELECT MenuID FROM [Menu] WHERE MenuName = '教師管理');
DECLARE @WebsiteMenuID INT = (SELECT MenuID FROM [Menu] WHERE MenuName = '網站資料');

-- 插入文章管理的子菜單
INSERT INTO [Menu] (MenuName, ParentID, SortOrder, IsActive, IconUrl, MenuUrl) VALUES
('文章分類', @ArticleMenuID, 1, 1, 'far fa-circle', '/Admin/ArticleCategory'),
('文章列表', @ArticleMenuID, 2, 1, 'far fa-circle', '/Admin/Article');

-- 插入系統管理的子菜單
INSERT INTO [Menu] (MenuName, ParentID, SortOrder, IsActive, IconUrl, MenuUrl) VALUES
('網站設定', @SystemMenuID, 1, 1, 'far fa-circle', '/Admin/WebsiteSettings'),
('Email設定', @SystemMenuID, 2, 1, 'far fa-circle', '/Admin/EmailSettings'),
('選單管理', @SystemMenuID, 3, 1, 'far fa-circle', '/Admin/Menu'),
('角色管理', @SystemMenuID, 4, 1, 'far fa-circle', '/Admin/Role'),
('使用者管理', @SystemMenuID, 5, 1, 'far fa-circle', '/Admin/User');

-- 插入教師管理的子菜單
INSERT INTO [Menu] (MenuName, ParentID, SortOrder, IsActive, IconUrl, MenuUrl) VALUES
('教師帳號', @TeacherMenuID, 1, 1, 'far fa-circle', '/Admin/Teacher'),
('教師角色', @TeacherMenuID, 2, 1, 'far fa-circle', '/Admin/TeacherRole');

-- 插入網站資料的子菜單
INSERT INTO [Menu] (MenuName, ParentID, SortOrder, IsActive, IconUrl, MenuUrl) VALUES
('文章類別', @WebsiteMenuID, 1, 1, 'far fa-circle', '/Admin/ArticleCategory'),
('文章管理', @WebsiteMenuID, 2, 1, 'far fa-circle', '/Admin/Article'),
('檔案總管', @WebsiteMenuID, 3, 1, 'far fa-circle', '/Admin/FileManager');

-- 驗證插入結果
SELECT 
    m.MenuID,
    m.MenuName,
    p.MenuName as ParentName,
    m.SortOrder,
    m.IsActive,
    m.IconUrl,
    m.MenuUrl
FROM [Menu] m
LEFT JOIN [Menu] p ON m.ParentID = p.MenuID
ORDER BY m.SortOrder, m.MenuName;

PRINT '菜單資料插入完成！';
GO
