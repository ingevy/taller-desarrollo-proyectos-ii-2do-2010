@ECHO OFF

SETLOCAL
%~d0
CD "%~dp0"

SET ServerAlias="%1%"
IF "%1%"=="" SET ServerAlias=.\SQLEXPRESS

ECHO.
ECHO ==================================
ECHO SelfManagement Database Setup 
ECHO ==================================
ECHO.

ECHO Droping the SelfManagement database...
@CALL SQLCMD -S %ServerAlias% -E -b -i ".\SelfManagementDB_Drop.sql"
IF ERRORLEVEL 1 GOTO ERROR

ECHO Creating the SelfManagement database...
@CALL SQLCMD -S %ServerAlias% -E -b -i ".\SelfManagementDB_Schema.sql"
IF ERRORLEVEL 1 GOTO ERROR

ECHO.
ECHO Setting SelfManagement database permissions...
@CALL SQLCMD -S %ServerAlias% -E -b -i ".\SelfManagementDB_Permissions.sql"
IF ERRORLEVEL 1 GOTO ERROR

ECHO.
ECHO Populating SelfManagement database with sample data...
@CALL SQLCMD -S %ServerAlias% -E -b -i ".\SelfManagementDB_Data.sql"
IF ERRORLEVEL 1 GOTO ERROR

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