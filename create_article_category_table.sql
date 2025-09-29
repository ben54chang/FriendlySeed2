-- 創建 ArticleCategory 表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ArticleCategory' AND xtype='U')
BEGIN
    CREATE TABLE ArticleCategory (
        ArticleCategoryID INT IDENTITY(1,1) PRIMARY KEY, -- 文章分類ID
        ArticleCategoryName NVARCHAR(100) NOT NULL, -- 文章分類名稱
        CategoryKeywords NVARCHAR(200) NULL, -- 分類關鍵字
        CategoryDescription NVARCHAR(500) NULL, -- 分類描述
        SortOrder INT DEFAULT 0, -- 排序
        CreatedTime DATETIME2 DEFAULT GETDATE(), -- 建立時間
        UpdatedTime DATETIME2 DEFAULT GETDATE(), -- 更新時間
        IsActive BIT DEFAULT 1 -- 狀態
    );
    PRINT 'ArticleCategory 表創建成功';
END
ELSE
BEGIN
    PRINT 'ArticleCategory 表已存在';
END
GO

-- 插入一些測試資料
IF NOT EXISTS (SELECT * FROM ArticleCategory WHERE ArticleCategoryName = '技術文章')
BEGIN
    INSERT INTO ArticleCategory (ArticleCategoryName, CategoryKeywords, CategoryDescription, SortOrder, IsActive)
    VALUES 
        ('技術文章', '程式設計,開發,技術', '關於程式設計和技術開發的文章', 1, 1),
        ('教學文章', '教學,學習,教育', '教學相關的文章和教程', 2, 1),
        ('新聞資訊', '新聞,資訊,更新', '最新的新聞和資訊', 3, 1),
        ('活動公告', '活動,公告,通知', '相關活動和公告', 4, 1);
    PRINT '測試資料插入成功';
END
ELSE
BEGIN
    PRINT '測試資料已存在';
END
GO
