-- 建立 Store Procedure 來處理角色新增和選單權限設定

CREATE OR ALTER PROCEDURE sp_CreateRoleWithMenus
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

