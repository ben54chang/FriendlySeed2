-- 修正 sp_UpdateRoleWithMenus 存儲過程的亂碼問題
-- 確保中文字符正確顯示

-- 先刪除現有的存儲過程
IF OBJECT_ID('sp_UpdateRoleWithMenus', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateRoleWithMenus;
GO

-- 建立修正後的存儲過程
CREATE PROCEDURE sp_UpdateRoleWithMenus
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

-- 測試存儲過程
PRINT 'sp_UpdateRoleWithMenus 存儲過程已成功建立';

