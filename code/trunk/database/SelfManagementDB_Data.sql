USE [SelfManagement]
GO

/****** Object:  Table [dbo].[Customers]    Script Date: 09/26/2010 18:25:43 ******/
SET IDENTITY_INSERT [dbo].[Customers] ON
INSERT [dbo].[Customers] ([Id], [Name]) VALUES (1, N'Banco Hipotecario')
INSERT [dbo].[Customers] ([Id], [Name]) VALUES (2, N'Banco Nación')
INSERT [dbo].[Customers] ([Id], [Name]) VALUES (3, N'Tarjeta Naranja')
INSERT [dbo].[Customers] ([Id], [Name]) VALUES (4, N'La Nueva Seguros')
SET IDENTITY_INSERT [dbo].[Customers] OFF

/****** Object:  Table [dbo].[aspnet_Applications]    Script Date: 09/26/2010 18:25:43 ******/
INSERT [dbo].[aspnet_Applications] ([ApplicationName], [LoweredApplicationName], [ApplicationId], [Description]) VALUES (N'/', N'/', N'edfa09ce-920b-4d73-8421-45569162b63f', NULL)

/****** Object:  Table [dbo].[aspnet_SchemaVersions]    Script Date: 09/26/2010 18:25:44 ******/
INSERT [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'common', N'1', 1)
INSERT [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'health monitoring', N'1', 1)
INSERT [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'membership', N'1', 1)
INSERT [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'personalization', N'1', 1)
INSERT [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'profile', N'1', 1)
INSERT [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'role manager', N'1', 1)

/****** Object:  Table [dbo].[aspnet_Users]    Script Date: 09/26/2010 18:25:44 ******/
SET IDENTITY_INSERT [dbo].[aspnet_Users] ON
INSERT [dbo].[aspnet_Users] ([ApplicationId], [UserId], [InnerUserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'e3c0a5be-e7fc-4197-9827-4ba75bf8a9a3', 1, N'administrator', N'administrator', NULL, 0, CAST(0x00009DFD01593C23 AS DateTime))
INSERT [dbo].[aspnet_Users] ([ApplicationId], [UserId], [InnerUserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'24b12ff3-d7ed-45e4-ba56-1e0a1ec4adc8', 2, N'john.doe', N'john.doe', NULL, 0, CAST(0x00009DFD015971D2 AS DateTime))
SET IDENTITY_INSERT [dbo].[aspnet_Users] OFF

/****** Object:  Table [dbo].[aspnet_Roles]    Script Date: 09/26/2010 18:25:44 ******/
INSERT [dbo].[aspnet_Roles] ([ApplicationId], [RoleId], [RoleName], [LoweredRoleName], [Description]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'8afe09e1-2767-4d10-accf-812c70bbf224', N'AccountManager', N'accountmanager', NULL)
INSERT [dbo].[aspnet_Roles] ([ApplicationId], [RoleId], [RoleName], [LoweredRoleName], [Description]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'71484297-a405-476c-a3c3-f903255eedfc', N'Agent', N'agent', NULL)
INSERT [dbo].[aspnet_Roles] ([ApplicationId], [RoleId], [RoleName], [LoweredRoleName], [Description]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'797bd93e-ec2c-4992-a989-12fe81af0a4c', N'ITManager', N'itmanager', NULL)
INSERT [dbo].[aspnet_Roles] ([ApplicationId], [RoleId], [RoleName], [LoweredRoleName], [Description]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'd7b9d5b7-17a1-4b15-b6d7-c67a6da57c00', N'Supervisor', N'supervisor', NULL)

/****** Object:  Table [dbo].[aspnet_UsersInRoles]    Script Date: 09/26/2010 18:25:45 ******/
INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'e3c0a5be-e7fc-4197-9827-4ba75bf8a9a3', N'797bd93e-ec2c-4992-a989-12fe81af0a4c')
INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'24b12ff3-d7ed-45e4-ba56-1e0a1ec4adc8', N'8afe09e1-2767-4d10-accf-812c70bbf224')

/****** Object:  Table [dbo].[aspnet_Membership]    Script Date: 09/26/2010 18:25:45 ******/
INSERT [dbo].[aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'e3c0a5be-e7fc-4197-9827-4ba75bf8a9a3', N'6Njljdbm6qQGxNj1u/jg40vE2/4=', 1, N'ic7GW1lX408qKLMSloGcKA==', NULL, N'administrator@callcenter.com', N'administrator@callcenter.com', NULL, NULL, 1, 0, CAST(0x00009DFD01593B4C AS DateTime), CAST(0x00009DFD01593C23 AS DateTime), CAST(0x00009DFD01593B4C AS DateTime), CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), NULL)
INSERT [dbo].[aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'24b12ff3-d7ed-45e4-ba56-1e0a1ec4adc8', N'epZFyGLMtJylFXZ0SmlAU99zxPw=', 1, N'G0ZWLLgnIr0tujllfDyfEQ==', NULL, N'john.doe@callcenter.com', N'john.doe@callcenter.com', NULL, NULL, 1, 0, CAST(0x00009DFD01597134 AS DateTime), CAST(0x00009DFD015971D2 AS DateTime), CAST(0x00009DFD01597134 AS DateTime), CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), NULL)