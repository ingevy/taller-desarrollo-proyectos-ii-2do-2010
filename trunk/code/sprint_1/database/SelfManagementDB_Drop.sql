USE [master]
GO

/****** Object:  Database [FedExMock]    Script Date: 08/03/2010 12:32:10 ******/
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'SelfManagement')
DROP DATABASE [SelfManagement]
GO