-- 快速除錯 SQL

-- 1. 檢查 Role 表是否有主鍵
SELECT COUNT(*) as PrimaryKeyCount 
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
WHERE TABLE_NAME = 'Role' AND CONSTRAINT_TYPE = 'PRIMARY KEY';

-- 2. 檢查 MenuRole 表是否有外鍵
SELECT COUNT(*) as ForeignKeyCount 
FROM sys.foreign_keys 
WHERE parent_object_id = OBJECT_ID('MenuRole');

-- 3. 檢查資料類型是否一致
SELECT 
    'Role.RoleID' as Table_Column,
    DATA_TYPE as DataType
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Role' AND COLUMN_NAME = 'RoleID'
UNION ALL
SELECT 
    'MenuRole.RoleID' as Table_Column,
    DATA_TYPE as DataType
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'MenuRole' AND COLUMN_NAME = 'RoleID';

-- 4. 檢查是否有重複的主鍵
SELECT RoleID, COUNT(*) as Count 
FROM Role 
GROUP BY RoleID 
HAVING COUNT(*) > 1;

-- 5. 檢查是否有重複的 MenuRole 組合
SELECT MenuID, RoleID, COUNT(*) as Count 
FROM MenuRole 
GROUP BY MenuID, RoleID 
HAVING COUNT(*) > 1;

