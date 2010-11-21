@ECHO OFF

SETLOCAL
%~d0
CD "%~dp0"

REM --------- Variables ---------
Set APPCMD="%systemroot%\system32\inetsrv\APPCMD"
IF EXIST %WINDIR%\SysWow64 (
set powerShellDir=%WINDIR%\SysWow64\windowspowershell\v1.0
) ELSE (
set powerShellDir=%WINDIR%\system32\windowspowershell\v1.0
)
REM -----------------------------

ECHO.
ECHO ==================================
ECHO SelfManagement IIS Setup 
ECHO ==================================
ECHO.

PUSHD "%~dp0..\"

ECHO Building SelfManagement solution...
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MsBuild "%CD%\code\SelfManagement.sln" /p:Configuration=Release

ECHO.
ECHO Removing Default Web Site runnning in port 80...
%APPCMD% delete site /site.name:"Default Web Site"

ECHO.
ECHO Trying to remove 'callcenter.selfmanagement.com' site from IIS...
FOR /F %%s in ('%APPCMD% list sites callcenter.selfmanagement.com /text:ID') DO IF /I %%s NEQ "" %APPCMD% delete site /site.name:"callcenter.selfmanagement.com"

ECHO.
ECHO Setting up SelfManagement web site in IIS...
%APPCMD% add site /name:"callcenter.selfmanagement.com" /physicalPath:"%CD%\code\SelfManagement.Web" /bindings:"http/*:80:" /applicationDefaults.applicationPool:"DefaultAppPool"

ECHO.
SET AspnetRegiisPath="C:\Windows\Microsoft.NET\Framework\v4.0.30319"
IF EXIST "C:\WINDOWS\Microsoft.NET\Framework64\" SET AspnetRegiisPath="C:\Windows\Microsoft.NET\Framework64\v4.0.30319"
@CALL %AspnetRegiisPath%\aspnet_regiis.exe -i

ECHO.
ECHO Setting up permissions in SelfManagement local folder...
CACLS "%CD%\code" /E /G "IIS_IUSRS":F /T /C
ICACLS "%CD%\code" /grant :r "IIS_IUSRS":F /T /Q /C

POPD

IISRESET

ECHO.
ECHO Adding 'callcenter.selfmanagement.com' entry to HOSTS file...
%powerShellDir%\powershell.exe -NonInteractive -Command "Set-ExecutionPolicy unrestricted"
%powerShellDir%\powershell.exe -NonInteractive -command ".\AddHosts.ps1"

ECHO.
ECHO ==================================
ECHO Setup finished successfully!
ECHO ==================================

GOTO FINISH

:ERROR

ECHO.
ECHO ==================================
ECHO An error occured. 
ECHO Please review messages above.
ECHO ==================================

:FINISH