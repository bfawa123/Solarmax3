@echo off
setlocal

for %%d in (%~dp0..) do set root_dir=%%~fd
set backup_dir=%root_dir%\scripts\known_good

if not exist "%backup_dir%\Assembly-CSharp\RoomWaitWindow.cs" (
  echo Known-good snapshot not found: %backup_dir%
  echo Run scripts\save-known-good.bat first.
  exit /b 1
)

copy /y "%backup_dir%\Assembly-CSharp\BGGroup.cs" "%root_dir%\Assembly-CSharp\BGGroup.cs" >nul
copy /y "%backup_dir%\Assembly-CSharp\BGManager.cs" "%root_dir%\Assembly-CSharp\BGManager.cs" >nul
copy /y "%backup_dir%\Assembly-CSharp\CommonDialogWindow.cs" "%root_dir%\Assembly-CSharp\CommonDialogWindow.cs" >nul
copy /y "%backup_dir%\Assembly-CSharp\CooperationRoomWindow.cs" "%root_dir%\Assembly-CSharp\CooperationRoomWindow.cs" >nul
copy /y "%backup_dir%\Assembly-CSharp\FriendRankingCell.cs" "%root_dir%\Assembly-CSharp\FriendRankingCell.cs" >nul
copy /y "%backup_dir%\Assembly-CSharp\HallSelectionWindow.cs" "%root_dir%\Assembly-CSharp\HallSelectionWindow.cs" >nul
copy /y "%backup_dir%\Assembly-CSharp\HomeWindow.cs" "%root_dir%\Assembly-CSharp\HomeWindow.cs" >nul
copy /y "%backup_dir%\Assembly-CSharp\LogoWindow.cs" "%root_dir%\Assembly-CSharp\LogoWindow.cs" >nul
copy /y "%backup_dir%\Assembly-CSharp\PlayerData.cs" "%root_dir%\Assembly-CSharp\PlayerData.cs" >nul
copy /y "%backup_dir%\Assembly-CSharp\PVEWaitWindow.cs" "%root_dir%\Assembly-CSharp\PVEWaitWindow.cs" >nul
copy /y "%backup_dir%\Assembly-CSharp\ResourcesUtil.cs" "%root_dir%\Assembly-CSharp\ResourcesUtil.cs" >nul
copy /y "%backup_dir%\Assembly-CSharp\RoomWaitWindow.cs" "%root_dir%\Assembly-CSharp\RoomWaitWindow.cs" >nul
copy /y "%backup_dir%\Assembly-CSharp\StartWindowPVP.cs" "%root_dir%\Assembly-CSharp\StartWindowPVP.cs" >nul
copy /y "%backup_dir%\Assembly-CSharp\Solarmax\ChapterAssistConfigProvider.cs" "%root_dir%\Assembly-CSharp\Solarmax\ChapterAssistConfigProvider.cs" >nul

copy /y "%backup_dir%\Solarmax3vt3_killer\assets\res\data\coochapter.xml" "%root_dir%\Solarmax3vt3_killer\assets\res\data\coochapter.xml" >nul

if exist "%backup_dir%\build\Assembly-CSharp.dll" copy /y "%backup_dir%\build\Assembly-CSharp.dll" "%root_dir%\build\Assembly-CSharp.dll" >nul
if exist "%backup_dir%\Solarmax3vt3_killer\assets\bin\Data\Managed\Assembly-CSharp.dll" copy /y "%backup_dir%\Solarmax3vt3_killer\assets\bin\Data\Managed\Assembly-CSharp.dll" "%root_dir%\Solarmax3vt3_killer\assets\bin\Data\Managed\Assembly-CSharp.dll" >nul

copy /y "%backup_dir%\scripts\recompile-dll.bat" "%root_dir%\scripts\recompile-dll.bat" >nul
copy /y "%backup_dir%\scripts\repack-apk.bat" "%root_dir%\scripts\repack-apk.bat" >nul

echo Restored known-good snapshot. Rebuild/repack to apply.
endlocal
