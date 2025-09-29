-- 檢查 Menu 表的資料
SELECT 
    MenuID,
    MenuName,
    ParentID,
    SortOrder,
    IconUrl,
    MenuUrl,
    IsActive,
    CreatedTime,
    UpdatedTime
FROM Menu
ORDER BY MenuID;

-- 檢查特定 MenuID 是否存在
SELECT COUNT(*) as RecordCount
FROM Menu 
WHERE MenuID = 1; -- 請替換為您要測試的 MenuID
