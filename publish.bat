@echo off
echo 開始發佈 FriendlySeed 專案...

REM 清理舊的發佈目錄
if exist "publish" (
    echo 清理舊的發佈目錄...
    rmdir /s /q "publish"
)

REM 建立發佈目錄
mkdir publish

REM 發佈專案
echo 發佈專案到 publish 目錄...
dotnet publish -c Release -o publish --self-contained false

if %ERRORLEVEL% EQU 0 (
    echo 發佈成功！
    echo.
    echo 發佈檔案位置: %CD%\publish
    echo.
    echo 開發環境啟動命令:
    echo   dotnet run --project publish\FriendlySeed.dll --urls "http://localhost:8054"
    echo.
    echo 正式環境啟動命令:
    echo   dotnet run --project publish\FriendlySeed.dll --urls "http://localhost:80"
    echo.
    echo 或使用 IIS 部署到 port 80
) else (
    echo 發佈失敗！請檢查錯誤訊息。
)

pause

