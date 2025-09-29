-- 測試 ArticleCategory 表是否存在
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ArticleCategory')
BEGIN
    PRINT 'ArticleCategory 表存在';
    
    -- 檢查表結構
    SELECT 
        COLUMN_NAME,
        DATA_TYPE,
        IS_NULLABLE,
        COLUMN_DEFAULT
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'ArticleCategory'
    ORDER BY ORDINAL_POSITION;
    
    -- 檢查資料
    SELECT COUNT(*) as RecordCount FROM ArticleCategory;
    
    -- 插入測試資料（如果沒有資料）
    IF NOT EXISTS (SELECT * FROM ArticleCategory)
    BEGIN
        INSERT INTO ArticleCategory (ArticleCategoryName, CategoryKeywords, CategoryDescription, SortOrder, IsActive)
        VALUES 
            ('技術文章', '程式設計,開發,技術', '關於程式設計和技術開發的文章', 1, 1),
            ('教學文章', '教學,學習,教育', '教學相關的文章和教程', 2, 1),
            ('新聞資訊', '新聞,資訊,更新', '最新的新聞和資訊', 3, 1);
        PRINT '測試資料插入成功';
    END
    ELSE
    BEGIN
        PRINT '資料已存在';
    END
END
ELSE
BEGIN
    PRINT 'ArticleCategory 表不存在，請先執行 create_article_category_table.sql';
END
