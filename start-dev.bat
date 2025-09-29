@echo off
echo 啟動 FriendlySeed 開發環境 (Port 8054)...

cd /d "%~dp0"

if not exist "publish\FriendlySeed.dll" (
    echo 發佈檔案不存在，請先執行 publish.bat
    pause
    exit /b 1
)

echo 啟動應用程式...
dotnet run --project publish\FriendlySeed.dll --urls "http://localhost:8054"

pause

