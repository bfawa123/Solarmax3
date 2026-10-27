@echo off
setlocal
for %%d in (%~dp0..) do set "root_dir=%%~fd"
for %%d in (%~dp0..\scripts) do set "scripts_dir=%%~fd"
for %%d in (%~dp0..\build-tools) do set "build_tools_dir=%%~fd"
for %%d in (%~dp0..\build) do set "build_dir=%%~fd"

rem check build tools
if not exist "%build_tools_dir%" (
   echo You should download and unzip the build tools first.
   pause
   exit
)

rem check java environment
java -version >nul 2>&1
if errorlevel 1 (
   echo Check java environment failed. Please install java environment.
   pause
   exit
)

call %scripts_dir%\recompile-dll.bat
if errorlevel 1 (
   echo Repack failed: recompile-dll.bat returned error.
   pause
   exit /b 1
)

cd /d "%root_dir%"
copy /y "%build_dir%\Assembly-CSharp.dll" "%root_dir%\Solarmax3vt3_killer\assets\bin\Data\Managed\Assembly-CSharp.dll" >nul

set "unsigned_apk=%build_dir%\Solarmax3vt3_killer-unsigned.apk"
set "aligned_apk=%build_dir%\Solarmax3vt3_killersigned-aligned.apk"

if exist "%unsigned_apk%" del "%unsigned_apk%"
if exist "%aligned_apk%" del "%aligned_apk%"

java.exe -jar "%build_tools_dir%\apktool.jar" b "%root_dir%\Solarmax3vt3_killer" -o "%unsigned_apk%"
if errorlevel 1 (
   echo Repack failed: apktool build error.
   pause
   exit /b 1
)
if not exist "%unsigned_apk%" (
   echo Repack failed: unsigned apk not generated.
   pause
   exit /b 1
)

call "%build_tools_dir%\android-build-tools\zipalign.exe" -v 4 "%unsigned_apk%" "%aligned_apk%"
if errorlevel 1 (
   echo Repack failed: zipalign error.
   pause
   exit /b 1
)

call "%build_tools_dir%\android-build-tools\apksigner.bat" sign -ks "%root_dir%\candice.keystore" -ks-pass pass:candice "%aligned_apk%"
if errorlevel 1 (
   echo Repack failed: apksigner error.
   pause
   exit /b 1
)

rem directly install to game_ass.dll
if not "%~1"=="" (
   adb install "%aligned_apk%"
)

echo "Repack apk completed!!!"

endlocal
