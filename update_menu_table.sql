-- =============================================
-- 修改 Menu 表結構
-- 1. IsActive 設為必填 (NOT NULL)
-- 2. SortOrder 預設值改為 10
-- =============================================

USE [FriendlySeed]
GO

PRINT '開始修改 Menu 表結構...'

-- 1. 先檢查並刪除現有的 IsActive 預設值約束
DECLARE @constraint_name NVARCHAR(128);
SELECT @constraint_name = name 
FROM sys.default_constraints 
WHERE parent_object_id = OBJECT_ID('Menu') 
    AND parent_column_id = COLUMNPROPERTY(OBJECT_ID('Menu'), 'IsActive', 'ColumnId');

IF @constraint_name IS NOT NULL
BEGIN
    DECLARE @drop_sql NVARCHAR(MAX) = 'ALTER TABLE [Menu] DROP CONSTRAINT ' + @constraint_name;
    EXEC sp_executesql @drop_sql;
    PRINT '已刪除現有的 IsActive 預設值約束: ' + @constraint_name;
END
ELSE
BEGIN
    PRINT '未找到現有的 IsActive 預設值約束';
END

-- 2. 修改 IsActive 欄位為 NOT NULL
ALTER TABLE [Menu] 
ALTER COLUMN IsActive BIT NOT NULL;
GO

PRINT 'IsActive 欄位已設為 NOT NULL';

-- 3. 設定 IsActive 的新預設值約束
ALTER TABLE [Menu] 
ADD CONSTRAINT DF_Menu_IsActive DEFAULT (1) FOR IsActive;
GO

PRINT 'IsActive 預設值約束已設定為 1 (true)';

-- 4. 檢查並刪除現有的 SortOrder 預設值約束
DECLARE @sort_constraint_name NVARCHAR(128);
SELECT @sort_constraint_name = name 
FROM sys.default_constraints 
WHERE parent_object_id = OBJECT_ID('Menu') 
    AND parent_column_id = COLUMNPROPERTY(OBJECT_ID('Menu'), 'SortOrder', 'ColumnId');

IF @sort_constraint_name IS NOT NULL
BEGIN
    DECLARE @drop_sort_sql NVARCHAR(MAX) = 'ALTER TABLE [Menu] DROP CONSTRAINT ' + @sort_constraint_name;
    EXEC sp_executesql @drop_sort_sql;
    PRINT '已刪除現有的 SortOrder 預設值約束: ' + @sort_constraint_name;
END
ELSE
BEGIN
    PRINT '未找到現有的 SortOrder 預設值約束';
END

-- 5. 設定 SortOrder 的新預設值約束為 10
ALTER TABLE [Menu] 
ADD CONSTRAINT DF_Menu_SortOrder DEFAULT (10) FOR SortOrder;
GO

PRINT 'SortOrder 預設值約束已設定為 10';

-- 6. 更新現有資料的 SortOrder 為 10 (如果目前為 0 或 NULL)
UPDATE [Menu] 
SET SortOrder = 10 
WHERE SortOrder IS NULL OR SortOrder = 0;
GO

PRINT '已更新現有資料的 SortOrder 值';

-- 7. 驗證修改結果
PRINT '=== 驗證修改結果 ===';

SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Menu' 
    AND COLUMN_NAME IN ('IsActive', 'SortOrder')
ORDER BY ORDINAL_POSITION;

-- 8. 顯示約束資訊
PRINT '=== 約束資訊 ===';

SELECT 
    dc.name AS ConstraintName,
    dc.definition AS ConstraintDefinition,
    c.name AS ColumnName
FROM sys.default_constraints dc
INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
WHERE dc.parent_object_id = OBJECT_ID('Menu')
    AND c.name IN ('IsActive', 'SortOrder');

PRINT 'Menu 表結構修改完成！';
GO
