-- 修改 Role 表結構，將 RoleID 從 nvarchar 改為 int 並設定自動遞增

-- 1. 先備份現有資料
SELECT * INTO Role_Backup FROM Role;

-- 2. 刪除現有的外鍵約束（如果有的話）
-- 檢查是否有外鍵約束
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
WHERE tr.name = 'Role';

-- 3. 刪除現有的主鍵約束
ALTER TABLE Role DROP CONSTRAINT PK_Role;

-- 4. 刪除 RoleID 欄位
ALTER TABLE Role DROP COLUMN RoleID;

-- 5. 新增 RoleID 欄位為 int 並設定為自動遞增主鍵
ALTER TABLE Role ADD RoleID int IDENTITY(1,1) NOT NULL;

-- 6. 設定 RoleID 為主鍵
ALTER TABLE Role ADD CONSTRAINT PK_Role PRIMARY KEY (RoleID);

-- 7. 更新 MenuRole 表的 RoleID 欄位類型
-- 先檢查 MenuRole 表是否存在
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'MenuRole')
BEGIN
    -- 刪除 MenuRole 表的外鍵約束（如果有的話）
    DECLARE @sql NVARCHAR(MAX) = '';
    SELECT @sql = @sql + 'ALTER TABLE MenuRole DROP CONSTRAINT ' + name + ';' + CHAR(13)
    FROM sys.foreign_keys 
    WHERE referenced_object_id = OBJECT_ID('Role');
    
    IF @sql != ''
        EXEC sp_executesql @sql;
    
    -- 修改 MenuRole 表的 RoleID 欄位類型
    ALTER TABLE MenuRole ALTER COLUMN RoleID int NOT NULL;
    
    -- 重新建立外鍵約束
    ALTER TABLE MenuRole 
    ADD CONSTRAINT FK_MenuRole_Role 
    FOREIGN KEY (RoleID) REFERENCES Role(RoleID);
END

-- 8. 更新 User 表的 RoleID 欄位類型（如果 User 表有 RoleID 欄位）
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('User') AND name = 'RoleID')
BEGIN
    -- 先處理 RoleID 欄位中的逗號分隔值
    -- 假設 RoleID 欄位包含逗號分隔的角色ID，我們需要處理這個情況
    -- 這裡先簡單地取第一個角色ID
    UPDATE [User] 
    SET RoleID = CASE 
        WHEN CHARINDEX(',', RoleID) > 0 
        THEN LEFT(RoleID, CHARINDEX(',', RoleID) - 1)
        ELSE RoleID
    END;
    
    -- 修改 User 表的 RoleID 欄位類型
    ALTER TABLE [User] ALTER COLUMN RoleID int;
END

-- 9. 驗證修改結果
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Role' 
ORDER BY ORDINAL_POSITION;

-- 10. 檢查資料是否正確
SELECT * FROM Role;

-- 11. 如果一切正常，可以刪除備份表
-- DROP TABLE Role_Backup;

