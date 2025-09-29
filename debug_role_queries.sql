-- 角色管理除錯 SQL 語法

-- 1. 檢查 Role 表結構
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Role' 
ORDER BY ORDINAL_POSITION;

-- 2. 檢查 Role 表的主鍵約束
SELECT 
    CONSTRAINT_NAME, 
    CONSTRAINT_TYPE 
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
WHERE TABLE_NAME = 'Role' AND CONSTRAINT_TYPE = 'PRIMARY KEY';

-- 3. 檢查 MenuRole 表結構
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'MenuRole' 
ORDER BY ORDINAL_POSITION;

-- 4. 檢查 MenuRole 表的主鍵約束
SELECT 
    CONSTRAINT_NAME, 
    CONSTRAINT_TYPE 
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
WHERE TABLE_NAME = 'MenuRole' AND CONSTRAINT_TYPE = 'PRIMARY KEY';

-- 5. 檢查 MenuRole 表的外鍵約束
SELECT 
    fk.name AS ForeignKeyName,
    tp.name AS ParentTable,
    cp.name AS ParentColumn,
    tr.name AS ReferencedTable,
    cr.name AS ReferencedColumn
FROM sys.foreign_keys fk
INNER JOIN sys.tables tp ON fk.parent_object_id = tp.object_id
INNER JOIN sys.tables tr ON fk.referenced_object_id = tr.object_id
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.columns cp ON fkc.parent_column_id = cp.column_id AND fkc.parent_object_id = cp.object_id
INNER JOIN sys.columns cr ON fkc.referenced_column_id = cr.column_id AND fkc.referenced_object_id = cr.object_id
WHERE tp.name = 'MenuRole';

-- 6. 檢查現有的 Role 資料
SELECT RoleID, RoleName, IsActive, CreatedTime, UpdatedTime FROM Role ORDER BY RoleID;

-- 7. 檢查現有的 Menu 資料（只顯示子選單）
SELECT MenuID, MenuName, ParentID, IsActive FROM Menu WHERE ParentID IS NOT NULL AND IsActive = 1 ORDER BY SortOrder;

-- 8. 檢查現有的 MenuRole 資料
SELECT MenuID, RoleID, IsActive, CreatedTime, UpdatedTime FROM MenuRole ORDER BY RoleID, MenuID;

-- 9. 測試插入新角色（手動測試）
-- INSERT INTO Role (RoleName, IsActive, CreatedTime, UpdatedTime) 
-- VALUES ('測試角色', 1, GETDATE(), GETDATE());

-- 10. 測試插入選單權限（手動測試）
-- INSERT INTO MenuRole (MenuID, RoleID, IsActive, CreatedTime, UpdatedTime)
-- VALUES (43, 1, 1, GETDATE(), GETDATE());

-- 11. 檢查角色和選單的關聯
SELECT 
    r.RoleID,
    r.RoleName,
    m.MenuID,
    m.MenuName,
    mr.IsActive as MenuRoleActive
FROM Role r
LEFT JOIN MenuRole mr ON r.RoleID = mr.RoleID
LEFT JOIN Menu m ON mr.MenuID = m.MenuID
WHERE r.IsActive = 1
ORDER BY r.RoleID, m.MenuID;

