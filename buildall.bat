setlocal
@echo off

if "%1" == "release" goto buildit
set BDEBUG=/d:DEBUG /debug
:buildit
set BDEBUG=

echo cleaning up

cd build\server
del /q /s *.*
cd ..\client
del /q /s *.*
cd ..\..

echo server build
cd src\server
csc /t:library SpiderCommon.cs %BDEBUG%
csc /t:library SpiderObj.cs /r:SpiderCommon.dll %BDEBUG%
csc /t:exe SpiderServer.cs

del ..\client\SpiderCommon.dll
copy SpiderCommon.dll ..\client\*.*
copy SpiderCommon.dll ..\..\build\guiclient\*.*
del ..\guiclient\spider\refs\SpiderCommon.dll
copy SpiderCommon.dll ..\guiclient\spider\refs\*.*

copy SpiderServer.exe.config ..\..\build\server
copy *.dll ..\..\build\server
copy *.exe ..\..\build\server
if not "" == "%BDEBUG%" (
	copy *.pdb ..\..\build\server
)
del *.dll
del *.exe
if not "" == "%BDEBUG%" (
	del *.pdb
)
echo client build

cd ..\client

rem csc /t:library SpiderCommon.cs 
csc /t:library SpiderClient.cs /r:SpiderCommon.dll %BDEBUG%
csc /t:exe Spider.cs /r:SpiderCommon.dll,SpiderClient.dll %BDEBUG%

copy Spider.exe.config ..\..\build\client
copy Spider.exe.config ..\..\build\guiclient
copy Spider.exe.config ..\guiclient\spider\refs\*.*

copy *.dll ..\..\build\client
copy *.exe ..\..\build\client
if not "" == "%BDEBUG%" (
	copy *.pdb ..\..\build\client
)

del *.dll
del *.exe
if not "" == "%BDEBUG%" (
	del *.pdb
)

echo guiclient copy
cd ..\guiclient\spider\refs
call run-me.bat
cd ..
cd bin
copy debug\spider.exe ..\..\..\..\build\guiclient\*.*
copy /y release\spider.exe ..\..\..\..\build\guiclient\*.*

cd ..\..\..\..\

echo done
endlocal