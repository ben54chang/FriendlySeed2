-- 簡化版本：修改 Role 表結構為 int 自動遞增

-- 1. 備份現有資料
SELECT * INTO Role_Backup FROM Role;

-- 2. 刪除現有的主鍵約束
ALTER TABLE Role DROP CONSTRAINT PK_Role;

-- 3. 刪除 RoleID 欄位
ALTER TABLE Role DROP COLUMN RoleID;

-- 4. 新增 RoleID 欄位為 int 並設定為自動遞增主鍵
ALTER TABLE Role ADD RoleID int IDENTITY(1,1) NOT NULL;

-- 5. 設定 RoleID 為主鍵
ALTER TABLE Role ADD CONSTRAINT PK_Role PRIMARY KEY (RoleID);

-- 6. 檢查結果
SELECT * FROM Role;

