del ..\bin\debug\SpiderCommon.dll
del ..\bin\release\SpiderCommon.dll
del ..\bin\debug\Spider.exe.config
del ..\bin\release\Spider.exe.config

copy SpiderCommon.dll ..\bin\debug\*.*
copy SpiderCommon.dll ..\bin\release\*.*
copy Spider.exe.config ..\bin\debug\*.*
copy Spider.exe.config ..\bin\release\*.*
