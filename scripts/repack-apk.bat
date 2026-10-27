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

rem cleanup stale temp files from previous failed runs (apktool_temp / idsig) to avoid rename lock
if exist "%unsigned_apk%" del "%unsigned_apk%"
if exist "%unsigned_apk%.apktool_temp" del "%unsigned_apk%.apktool_temp"
if exist "%aligned_apk%" del "%aligned_apk%"
if exist "%aligned_apk%.idsig" del "%aligned_apk%.idsig"

rem apktool build; Windows Defender may briefly lock the temp APK during scan causing rename failure, so retry up to 3 times
set /a apktool_retry=0
:apktool_build
java.exe -jar "%build_tools_dir%\apktool.jar" b "%root_dir%\Solarmax3vt3_killer" -o "%unsigned_apk%"
if not errorlevel 1 goto apktool_build_ok
set /a apktool_retry+=1
if %apktool_retry% lss 4 (
   echo apktool build failed, retry %apktool_retry% (waiting 3 seconds)...
   timeout /t 3 /nobreak >nul
   goto apktool_build
)
echo Repack failed: apktool build error.
pause
exit /b 1
:apktool_build_ok
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
