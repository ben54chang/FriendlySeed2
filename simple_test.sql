-- 簡單測試 SQL

-- 1. 新增角色
INSERT INTO Role (RoleName, IsActive, CreatedTime, UpdatedTime) 
VALUES ('TestRole', 1, GETDATE(), GETDATE());

-- 2. 取得新角色 ID
DECLARE @NewRoleID int = SCOPE_IDENTITY();
SELECT @NewRoleID as NewRoleID;

-- 3. 新增選單權限
INSERT INTO MenuRole (MenuID, RoleID, IsActive, CreatedTime, UpdatedTime)
VALUES (43, @NewRoleID, 1, GETDATE(), GETDATE());

-- 4. 檢查結果
SELECT 
    r.RoleID,
    r.RoleName,
    m.MenuID,
    m.MenuName
FROM Role r
LEFT JOIN MenuRole mr ON r.RoleID = mr.RoleID
LEFT JOIN Menu m ON mr.MenuID = m.MenuID
WHERE r.RoleID = @NewRoleID;

