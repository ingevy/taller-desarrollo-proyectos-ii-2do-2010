@ECHO OFF

SETLOCAL
%~d0
CD "%~dp0"

SET ServerAlias="%1%"
IF "%1%"=="" SET ServerAlias=.\SQLEXPRESS

ECHO.
ECHO ====================================
ECHO SelfManagement Tests Database Setup 
ECHO ====================================
ECHO.

ECHO Droping and creating the SelfManagement.Tests database...
@CALL SQLCMD -S %ServerAlias% -E -b -i ".\SelfManagementTestsDB_DropAndCreate.sql"
IF ERRORLEVEL 1 GOTO ERROR

ECHO Creating the schema of the SelfManagement.Tests database...
@CALL SQLCMD -S %ServerAlias% -E -b -d SelfManagement.Tests -i ".\SelfManagementDB_Schema.sql"
IF ERRORLEVEL 1 GOTO ERROR

ECHO.
ECHO Setting SelfManagement.Tests database permissions...
@CALL SQLCMD -S %ServerAlias% -E -b -d SelfManagement.Tests -i ".\SelfManagementDB_Permissions.sql"
IF ERRORLEVEL 1 GOTO ERROR

ECHO.
ECHO Populating SelfManagement.Tests database with sample data...
@CALL SQLCMD -S %ServerAlias% -E -b -d SelfManagement.Tests -i ".\SelfManagementTestsDB_Data.sql"
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