@echo off

setlocal 
%~d0
cd "%~dp0"

@call .\database\SetupDatabase.cmd

@call .\database\SetupTestsDatabase.cmd

@call .\iis-setup\SetupWebSiteInIIS.cmd

PAUSE