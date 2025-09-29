-- 修正 User 資料表結構
USE FriendlySeed;
GO

-- 1. 修改 RoleID 欄位為 nvarchar(500) 以支援多選
-- 先檢查是否有外鍵約束
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE referenced_object_id = OBJECT_ID('Role') AND parent_object_id = OBJECT_ID('User'))
BEGIN
    -- 刪除外鍵約束
    DECLARE @sql NVARCHAR(MAX) = '';
    SELECT @sql = @sql + 'ALTER TABLE [User] DROP CONSTRAINT ' + name + ';' + CHAR(13)
    FROM sys.foreign_keys 
    WHERE referenced_object_id = OBJECT_ID('Role') AND parent_object_id = OBJECT_ID('User');
    
    EXEC sp_executesql @sql;
    PRINT '已刪除 RoleID 外鍵約束';
END

-- 修改 RoleID 欄位類型
ALTER TABLE [User] 
ALTER COLUMN RoleID NVARCHAR(500) NOT NULL;
GO

PRINT 'RoleID 欄位已修改為 NVARCHAR(500)';

-- 2. 修改 IsActive 欄位：不允許 NULL，預設值為 true
-- 先檢查並刪除現有的預設值約束
DECLARE @constraint_name NVARCHAR(128);
SELECT @constraint_name = name 
FROM sys.default_constraints 
WHERE parent_object_id = OBJECT_ID('User') 
    AND parent_column_id = COLUMNPROPERTY(OBJECT_ID('User'), 'IsActive', 'ColumnId');

IF @constraint_name IS NOT NULL
BEGIN
    DECLARE @drop_sql NVARCHAR(MAX) = 'ALTER TABLE [User] DROP CONSTRAINT ' + @constraint_name;
    EXEC sp_executesql @drop_sql;
    PRINT '已刪除現有的 IsActive 預設值約束: ' + @constraint_name;
END

-- 修改 IsActive 欄位為 NOT NULL
ALTER TABLE [User] 
ALTER COLUMN IsActive BIT NOT NULL;
GO

-- 設定新的預設值約束
ALTER TABLE [User] 
ADD CONSTRAINT DF_User_IsActive DEFAULT (1) FOR IsActive;
GO

PRINT 'IsActive 欄位已修改為 NOT NULL，預設值為 true';

-- 3. 顯示修改後的欄位結構
SELECT 
    COLUMN_NAME as '欄位名稱',
    DATA_TYPE as '資料類型',
    CHARACTER_MAXIMUM_LENGTH as '最大長度',
    IS_NULLABLE as '允許NULL',
    COLUMN_DEFAULT as '預設值'
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'User' 
    AND COLUMN_NAME IN ('RoleID', 'IsActive')
ORDER BY ORDINAL_POSITION;

PRINT 'User 資料表結構修改完成！';
PRINT '注意：RoleID 現在支援多選（以逗號分隔的角色ID，例如：1,2,3）';
PRINT 'IsActive 現在不允許 NULL，預設值為 true';
