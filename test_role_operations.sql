-- 角色管理操作測試 SQL

-- 1. 測試新增角色
DECLARE @NewRoleID int;
INSERT INTO Role (RoleName, IsActive, CreatedTime, UpdatedTime) 
VALUES ('測試角色', 1, GETDATE(), GETDATE());
SET @NewRoleID = SCOPE_IDENTITY();
SELECT '新增角色成功，RoleID: ' + CAST(@NewRoleID AS VARCHAR(10)) as Result;

-- 2. 測試新增選單權限
INSERT INTO MenuRole (MenuID, RoleID, IsActive, CreatedTime, UpdatedTime)
VALUES (43, @NewRoleID, 1, GETDATE(), GETDATE());
SELECT '新增選單權限成功' as Result;

-- 3. 測試更新角色
UPDATE Role 
SET RoleName = '測試角色_更新', UpdatedTime = GETDATE()
WHERE RoleID = @NewRoleID;
SELECT '更新角色成功' as Result;

-- 4. 測試更新選單權限（先停用舊的，再新增新的）
UPDATE MenuRole 
SET IsActive = 0, UpdatedTime = GETDATE()
WHERE RoleID = @NewRoleID;

INSERT INTO MenuRole (MenuID, RoleID, IsActive, CreatedTime, UpdatedTime)
VALUES (44, @NewRoleID, 1, GETDATE(), GETDATE());
SELECT '更新選單權限成功' as Result;

-- 5. 檢查結果
SELECT 
    r.RoleID,
    r.RoleName,
    m.MenuID,
    m.MenuName,
    mr.IsActive as MenuRoleActive
FROM Role r
LEFT JOIN MenuRole mr ON r.RoleID = mr.RoleID AND mr.IsActive = 1
LEFT JOIN Menu m ON mr.MenuID = m.MenuID
WHERE r.RoleID = @NewRoleID;

-- 6. 清理測試資料
DELETE FROM MenuRole WHERE RoleID = @NewRoleID;
DELETE FROM Role WHERE RoleID = @NewRoleID;
SELECT '清理測試資料完成' as Result;

