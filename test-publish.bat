@echo off
echo 測試發佈版本...

cd /d "%~dp0\publish"

echo 啟動應用程式 (Port 8054)...
echo 請在瀏覽器中開啟 http://localhost:8054
echo 按 Ctrl+C 停止應用程式
echo.

dotnet FriendlySeed.dll --urls "http://localhost:8054"

