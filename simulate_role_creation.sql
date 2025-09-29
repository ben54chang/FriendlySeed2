-- 模擬角色新增和選單權限設定

-- 1. 開始事務
BEGIN TRANSACTION;

-- 2. 新增角色
INSERT INTO Role (RoleName, IsActive, CreatedTime, UpdatedTime) 
VALUES ('測試角色_' + FORMAT(GETDATE(), 'yyyyMMddHHmmss'), 1, GETDATE(), GETDATE());

-- 3. 取得新角色的 ID
DECLARE @NewRoleID int = SCOPE_IDENTITY();
SELECT '新角色 ID: ' + CAST(@NewRoleID AS VARCHAR(10)) as NewRoleID;

-- 4. 設定選單權限（選擇子選單：文章分類、文章列表、使用者管理）
INSERT INTO MenuRole (MenuID, RoleID, IsActive, CreatedTime, UpdatedTime)
VALUES 
    (43, @NewRoleID, 1, GETDATE(), GETDATE()),  -- 文章分類
    (44, @NewRoleID, 1, GETDATE(), GETDATE()),  -- 文章列表
    (45, @NewRoleID, 1, GETDATE(), GETDATE());  -- 使用者管理

-- 5. 檢查結果
SELECT 
    r.RoleID,
    r.RoleName,
    m.MenuID,
    m.MenuName,
    mr.IsActive as MenuRoleActive,
    mr.CreatedTime
FROM Role r
LEFT JOIN MenuRole mr ON r.RoleID = mr.RoleID AND mr.IsActive = 1
LEFT JOIN Menu m ON mr.MenuID = m.MenuID
WHERE r.RoleID = @NewRoleID
ORDER BY m.MenuID;

-- 6. 提交事務
COMMIT TRANSACTION;

-- 7. 最終檢查
SELECT '角色新增和選單權限設定完成' as Result;

