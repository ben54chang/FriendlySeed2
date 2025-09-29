-- 修正所有角色管理存儲過程的亂碼問題
-- 確保中文字符正確顯示

-- =============================================
-- 1. 修正 sp_CreateRoleWithMenus
-- =============================================
IF OBJECT_ID('sp_CreateRoleWithMenus', 'P') IS NOT NULL
    DROP PROCEDURE sp_CreateRoleWithMenus;
GO

CREATE PROCEDURE [dbo].[sp_CreateRoleWithMenus]
    @RoleName NVARCHAR(100),
    @IsActive BIT = 1,
    @MenuIds NVARCHAR(MAX), -- 逗號分隔的選單ID字串，如 "45,50,48,49"
    @NewRoleID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ErrorMessage NVARCHAR(4000) = '';
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- 1. 新增角色
        INSERT INTO Role (RoleName, IsActive, CreatedTime, UpdatedTime)
        VALUES (@RoleName, @IsActive, GETDATE(), GETDATE());
        
        -- 2. 取得新角色的 ID
        SET @NewRoleID = SCOPE_IDENTITY();
        
        -- 3. 檢查是否有選單權限要設定
        IF @MenuIds IS NOT NULL AND LEN(LTRIM(RTRIM(@MenuIds))) > 0
        BEGIN
            -- 4. 先刪除現有的選單權限（如果有的話）
            DELETE FROM MenuRole WHERE RoleID = @NewRoleID;
            
            -- 5. 插入新的選單權限
            -- 使用 STRING_SPLIT 來分割選單ID字串
            INSERT INTO MenuRole (MenuID, RoleID, IsActive, CreatedTime, UpdatedTime)
            SELECT 
                CAST(value AS INT) as MenuID,
                @NewRoleID as RoleID,
                1 as IsActive,
                GETDATE() as CreatedTime,
                GETDATE() as UpdatedTime
            FROM STRING_SPLIT(@MenuIds, ',')
            WHERE LTRIM(RTRIM(value)) != '';
        END
        
        COMMIT TRANSACTION;
        
        -- 6. 回傳成功訊息
        SELECT 'SUCCESS' as Result, @NewRoleID as NewRoleID;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        SELECT 
            ERROR_NUMBER() as ErrorNumber,
            ERROR_SEVERITY() as ErrorSeverity,
            ERROR_STATE() as ErrorState,
            ERROR_MESSAGE() as ErrorMessage;
            
        SET @NewRoleID = 0;
    END CATCH
END
GO

-- =============================================
-- 2. 修正 sp_UpdateRoleWithMenus
-- =============================================
IF OBJECT_ID('sp_UpdateRoleWithMenus', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateRoleWithMenus;
GO

CREATE PROCEDURE [dbo].[sp_UpdateRoleWithMenus]
    @RoleID INT,
    @RoleName NVARCHAR(50),
    @IsActive BIT,
    @MenuIds NVARCHAR(MAX) = NULL, -- 逗號分隔的選單ID字串，如 "45,50,48,49"
    @Message NVARCHAR(255) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Message = 'SUCCESS';

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. 更新角色資料
        UPDATE Role
        SET
            RoleName = @RoleName,
            IsActive = @IsActive,
            UpdatedTime = GETDATE()
        WHERE
            RoleID = @RoleID;

        -- 檢查角色是否存在
        IF @@ROWCOUNT = 0
        BEGIN
            SET @Message = 'Role not found or no changes made.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2. 停用現有的選單權限
        UPDATE MenuRole
        SET
            IsActive = 0,
            UpdatedTime = GETDATE()
        WHERE
            RoleID = @RoleID;

        -- 3. 插入或重新啟用新的選單權限
        IF @MenuIds IS NOT NULL AND @MenuIds <> ''
        BEGIN
            -- 使用 MERGE 來處理選單權限的插入或更新
            MERGE MenuRole AS target
            USING (
                SELECT
                    CAST(value AS INT) AS MenuID,
                    @RoleID AS RoleID,
                    1 AS IsActive,
                    GETDATE() AS CreatedTime,
                    GETDATE() AS UpdatedTime
                FROM
                    STRING_SPLIT(@MenuIds, ',')
                WHERE
                    TRY_CAST(value AS INT) IS NOT NULL
            ) AS source
            ON (target.MenuID = source.MenuID AND target.RoleID = source.RoleID)
            WHEN MATCHED THEN
                UPDATE SET
                    target.IsActive = 1,
                    target.UpdatedTime = source.UpdatedTime
            WHEN NOT MATCHED BY TARGET THEN
                INSERT (MenuID, RoleID, IsActive, CreatedTime, UpdatedTime)
                VALUES (source.MenuID, source.RoleID, source.IsActive, source.CreatedTime, source.UpdatedTime);
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @Message = ERROR_MESSAGE();
    END CATCH
END;
GO

-- =============================================
-- 3. 測試存儲過程
-- =============================================
PRINT '所有角色管理存儲過程已成功建立，亂碼問題已修正';
PRINT 'sp_CreateRoleWithMenus - 建立完成';
PRINT 'sp_UpdateRoleWithMenus - 建立完成';

-- 測試建立角色
DECLARE @TestRoleID INT;
DECLARE @TestMessage NVARCHAR(255);

EXEC sp_CreateRoleWithMenus 
    @RoleName = N'測試角色',
    @IsActive = 1,
    @MenuIds = N'45,50,48,49',
    @NewRoleID = @TestRoleID OUTPUT;

PRINT '測試結果:';
PRINT 'RoleID: ' + CAST(@TestRoleID AS NVARCHAR(10));

