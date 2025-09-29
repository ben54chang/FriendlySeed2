@echo off
echo 啟動 FriendlySeed 正式環境 (Port 80)...

cd /d "%~dp0"

if not exist "publish\FriendlySeed.dll" (
    echo 發佈檔案不存在，請先執行 publish.bat
    pause
    exit /b 1
)

echo 啟動應用程式...
dotnet run --project publish\FriendlySeed.dll --urls "http://localhost:80" --environment Production

pause

