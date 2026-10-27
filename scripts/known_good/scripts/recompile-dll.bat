@echo off
setlocal
for %%d in (%~dp0..) do set root_dir=%%~fd
for %%d in (%~dp0..\scripts) do set scripts_dir=%%~fd
for %%d in (%~dp0..\build-tools) do set build_tools_dir=%%~fd
for %%d in (%~dp0..\build) do set build_dir=%%~fd

cd %root_dir%

rem check build tools
if not exist %build_tools_dir% (
   echo You should download and unzip the build tools first.
   pause
   exit
)

if not exist %build_dir% (
   mkdir %build_dir%
   echo "create build directory."
)

rem build Assembly-CSharp.dll
rem "C:\Program Files\Unity\Hub\Editor\2017.4.30f1\Editor\Data\MonoBleedingEdge\lib\mono\4.5\mcs.exe" -sdk:2 -target:library -out:Assembly-CSharp.dll "@dep_mcs.txt" "@files.txt"
%build_tools_dir%\mcs.exe -sdk:2 -target:library -out:%build_dir%\Assembly-CSharp.dll "@%root_dir%\dep_mcs.txt" "@%root_dir%\files.txt"
if errorlevel 1 (
   echo Recompile failed: mcs returned error.
   pause
   exit /b 1
)
%build_tools_dir%\rz\bin\rizin -w -c "wx 35 @ 0x400000" -qq %build_dir%\Assembly-CSharp.dll
if errorlevel 1 (
   echo Recompile failed: rizin patch returned error.
   pause
   exit /b 1
)

rem directly install to game_ass.dll
if not "%~1"=="" (
   adb push %build_dir%\Assembly-CSharp.dll /data/local/tmp
   adb shell su -c cp /data/local/tmp/Assembly-CSharp.dll /data/data/com.solarmax3.guanwangtlm/files/game_ass.dll
)

echo "Recompile dll completed!!!"

endlocal
