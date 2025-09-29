-- 清理 Menu 表中的無效資料
-- 1. 刪除自我引用的記錄
DELETE FROM Menu WHERE MenuID = ParentID;

-- 2. 刪除全是 NULL 的記錄
DELETE FROM Menu WHERE MenuID IS NULL OR MenuName IS NULL;

-- 3. 檢查清理後的資料
SELECT 
    MenuID,
    MenuName,
    ParentID,
    SortOrder,
    IsActive,
    CreatedTime,
    UpdatedTime,
    IconUrl,
    MenuUrl
FROM Menu
ORDER BY MenuID;
