-- 建立 TeacherRole 資料表
-- 教師角色管理

-- 檢查資料表是否存在，如果存在則刪除
IF OBJECT_ID('TeacherRole', 'U') IS NOT NULL
    DROP TABLE TeacherRole;
GO

-- 建立 TeacherRole 資料表
CREATE TABLE TeacherRole (
    TeacherRoleID INT IDENTITY(1,1) PRIMARY KEY,
    TeacherRoleName NVARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedTime DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedTime DATETIME NOT NULL DEFAULT GETDATE()
);

-- 建立索引
CREATE INDEX IX_TeacherRole_IsActive ON TeacherRole(IsActive);
CREATE INDEX IX_TeacherRole_CreatedTime ON TeacherRole(CreatedTime);

-- 插入預設資料
INSERT INTO TeacherRole (TeacherRoleName, IsActive) VALUES
('校長', 1),
('主任', 1),
('組長', 1),
('導師', 1),
('科任教師', 1),
('代課教師', 1),
('實習教師', 1);

-- 顯示建立結果
SELECT 'TeacherRole 資料表建立成功' AS Message;
SELECT COUNT(*) AS RecordCount FROM TeacherRole;

