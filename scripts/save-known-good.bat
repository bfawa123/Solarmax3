@echo off
setlocal

for %%d in (%~dp0..) do set root_dir=%%~fd
set backup_dir=%root_dir%\scripts\known_good

if exist "%backup_dir%" rmdir /s /q "%backup_dir%"

mkdir "%backup_dir%"
mkdir "%backup_dir%\Assembly-CSharp"
mkdir "%backup_dir%\Assembly-CSharp\Solarmax"
mkdir "%backup_dir%\Solarmax3vt3_killer\assets\res\data"
mkdir "%backup_dir%\Solarmax3vt3_killer\assets\bin\Data\Managed"
mkdir "%backup_dir%\scripts"
mkdir "%backup_dir%\build"

copy /y "%root_dir%\Assembly-CSharp\BGGroup.cs" "%backup_dir%\Assembly-CSharp\BGGroup.cs" >nul
copy /y "%root_dir%\Assembly-CSharp\BGManager.cs" "%backup_dir%\Assembly-CSharp\BGManager.cs" >nul
copy /y "%root_dir%\Assembly-CSharp\CommonDialogWindow.cs" "%backup_dir%\Assembly-CSharp\CommonDialogWindow.cs" >nul
copy /y "%root_dir%\Assembly-CSharp\CooperationRoomWindow.cs" "%backup_dir%\Assembly-CSharp\CooperationRoomWindow.cs" >nul
copy /y "%root_dir%\Assembly-CSharp\FriendRankingCell.cs" "%backup_dir%\Assembly-CSharp\FriendRankingCell.cs" >nul
copy /y "%root_dir%\Assembly-CSharp\HallSelectionWindow.cs" "%backup_dir%\Assembly-CSharp\HallSelectionWindow.cs" >nul
copy /y "%root_dir%\Assembly-CSharp\HomeWindow.cs" "%backup_dir%\Assembly-CSharp\HomeWindow.cs" >nul
copy /y "%root_dir%\Assembly-CSharp\LogoWindow.cs" "%backup_dir%\Assembly-CSharp\LogoWindow.cs" >nul
copy /y "%root_dir%\Assembly-CSharp\PlayerData.cs" "%backup_dir%\Assembly-CSharp\PlayerData.cs" >nul
copy /y "%root_dir%\Assembly-CSharp\PVEWaitWindow.cs" "%backup_dir%\Assembly-CSharp\PVEWaitWindow.cs" >nul
copy /y "%root_dir%\Assembly-CSharp\ResourcesUtil.cs" "%backup_dir%\Assembly-CSharp\ResourcesUtil.cs" >nul
copy /y "%root_dir%\Assembly-CSharp\RoomWaitWindow.cs" "%backup_dir%\Assembly-CSharp\RoomWaitWindow.cs" >nul
copy /y "%root_dir%\Assembly-CSharp\StartWindowPVP.cs" "%backup_dir%\Assembly-CSharp\StartWindowPVP.cs" >nul
copy /y "%root_dir%\Assembly-CSharp\Solarmax\ChapterAssistConfigProvider.cs" "%backup_dir%\Assembly-CSharp\Solarmax\ChapterAssistConfigProvider.cs" >nul

copy /y "%root_dir%\Solarmax3vt3_killer\assets\res\data\coochapter.xml" "%backup_dir%\Solarmax3vt3_killer\assets\res\data\coochapter.xml" >nul

if exist "%root_dir%\build\Assembly-CSharp.dll" copy /y "%root_dir%\build\Assembly-CSharp.dll" "%backup_dir%\build\Assembly-CSharp.dll" >nul
if exist "%root_dir%\Solarmax3vt3_killer\assets\bin\Data\Managed\Assembly-CSharp.dll" copy /y "%root_dir%\Solarmax3vt3_killer\assets\bin\Data\Managed\Assembly-CSharp.dll" "%backup_dir%\Solarmax3vt3_killer\assets\bin\Data\Managed\Assembly-CSharp.dll" >nul

copy /y "%root_dir%\scripts\recompile-dll.bat" "%backup_dir%\scripts\recompile-dll.bat" >nul
copy /y "%root_dir%\scripts\repack-apk.bat" "%backup_dir%\scripts\repack-apk.bat" >nul

echo Saved known-good snapshot to: %backup_dir%
endlocal
