-- 修正 sp_CreateRoleWithMenus 存儲過程的亂碼問題
-- 確保中文字符正確顯示

-- 先刪除現有的存儲過程
IF OBJECT_ID('sp_CreateRoleWithMenus', 'P') IS NOT NULL
    DROP PROCEDURE sp_CreateRoleWithMenus;
GO

-- 建立修正後的存儲過程
CREATE PROCEDURE sp_CreateRoleWithMenus
    @RoleName NVARCHAR(50),
    @IsActive BIT,
    @MenuIds NVARCHAR(MAX) = NULL, -- 逗號分隔的選單ID字串，如 "45,50,48,49"
    @NewRoleID INT OUTPUT,
    @Message NVARCHAR(255) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Message = 'SUCCESS';

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. 新增角色
        INSERT INTO Role (RoleName, IsActive, CreatedTime, UpdatedTime)
        VALUES (@RoleName, @IsActive, GETDATE(), GETDATE());

        -- 2. 取得新角色的 ID
        SET @NewRoleID = SCOPE_IDENTITY();

        -- 3. 檢查是否有選單權限要設定
        IF @MenuIds IS NOT NULL AND @MenuIds <> ''
        BEGIN
            -- 4. 插入新的選單權限
            -- 使用 STRING_SPLIT 來分割選單ID字串
            INSERT INTO MenuRole (MenuID, RoleID, IsActive, CreatedTime, UpdatedTime)
            SELECT 
                CAST(value AS INT) as MenuID,
                @NewRoleID as RoleID,
                1 as IsActive,
                GETDATE() as CreatedTime,
                GETDATE() as UpdatedTime
            FROM STRING_SPLIT(@MenuIds, ',')
            WHERE TRY_CAST(value AS INT) IS NOT NULL;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @NewRoleID = 0;
        SET @Message = ERROR_MESSAGE();
    END CATCH
END;
GO

-- 測試存儲過程
PRINT 'sp_CreateRoleWithMenus 存儲過程已成功建立';

