USE [SelfManagement]
GO

CREATE USER [NT AUTHORITY\NETWORK SERVICE] FOR LOGIN [NT AUTHORITY\NETWORK SERVICE]
GO

CREATE USER [BUILTIN\IIS_IUSRS] FOR LOGIN [BUILTIN\IIS_IUSRS]
GO

EXEC sp_addrolemember 'db_owner', 'NT AUTHORITY\NETWORK SERVICE'
GO

EXEC sp_addrolemember 'db_owner', 'BUILTIN\IIS_IUSRS'
GO