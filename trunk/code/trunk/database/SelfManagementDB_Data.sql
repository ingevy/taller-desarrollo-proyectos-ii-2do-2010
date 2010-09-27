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

/****** Object:  Table [dbo].[Metrics]    Script Date: 09/26/2010 23:27:47 ******/
SET IDENTITY_INSERT [dbo].[Metrics] ON
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [CLRType]) VALUES (1, N'I2C_PCT', N'Interaction to Call Percent', N'Total interactions created divided by total calls handled.', 0, N'CallCenter.SelfManagement.Metric.InteractionToCallPercentageMetric, SelfManagement.Metric')
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [CLRType]) VALUES (3, N'ACW_PCT', N'After Call Work Percentage', N'After Call Work Percentage', 0, N'CallCenter.SelfManagement.Metric.AfterCallWorkTimePercentageMetric, SelfManagement.Metric')
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [CLRType]) VALUES (4, N'ACW_TM', N'After Call Work Time', N'The amount of time spent doing After Call Work. Hours/Min/Sec.', 1, N'CallCenter.SelfManagement.Metric.AfterCallWorkTimeNumberMetric, SelfManagement.Metric')
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [CLRType]) VALUES (5, N'AUX_TM', N'Time in AUX Status', N'The total time spent in an AUX status.', 1, N'CallCenter.SelfManagement.Metric.AuxStatusTimeNumberMetric, SelfManagement.Metric')
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [CLRType]) VALUES (6, N'AVAIL_PCT', N'Available Call Status Percentage', N'Percentage of time an agent spent in an available call status.', 0, N'CallCenter.SelfManagement.Metric.AvailableCallStatusTimePercentageMetric, SelfManagement.Metric')
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [CLRType]) VALUES (7, N'AVAIL_TM', N'Time Spent in Available Call Status', N'Amount of time an agent spent in an available call status.', 1, N'CallCenter.SelfManagement.Metric.AvailableCallStatusTimeNumberMetric, SelfManagement.Metric')
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [CLRType]) VALUES (8, N'AVG_ACW_TM', N'Average After Call Work', N'The average amount of time from when the agent releases the caller to the time the agent dispositions the call and returns to either a ready status or a not ready status.', 1, N'CallCenter.SelfManagement.Metric.AverageAfterCallWorkTimeNumberMetric, SelfManagement.Metric')
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [CLRType]) VALUES (9, N'AVG_HDL_TM', N'Average Handle Time', N'The average amount of time from when an agent answers the call to when the agent either goes back into queue or into an AUX status. Hrs/Mins/Secs.', 1, N'CallCenter.SelfManagement.Metric.AverageHandleTimeNumberMetric, SelfManagement.Metric')
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [CLRType]) VALUES (10, N'AVG_TALK_TM', N'Average Talk Time (seconds/call)', N'The average time the agent spent talking to a customer. Sec/call.', 1, N'CallCenter.SelfManagement.Metric.AverageTalkTimeNumberMetric, SelfManagement.Metric')
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [CLRType]) VALUES (11, N'AWCV', N'Average Weighted Call Value (seconds/call)', N'The average of the total time in seconds an agent spends on calls. This is the sum of talk time, hold time, and after call work time divided by the number of calls taken.', 1, N'CallCenter.SelfManagement.Metric.AverageWeightedCallTimeNumberMetric, SelfManagement.Metric')
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [CLRType]) VALUES (12, N'CPH', N'Number of Calls Per Hour', N'The average number of calls handled by an agent in one hour.', 1, N'CallCenter.SelfManagement.Metric.AverageCallsPerHourNumberMetric, SelfManagement.Metric')
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [CLRType]) VALUES (13, N'INCHAIR_OCC', N'Percentage of Time Spent in Billable Mode', N'The percentage of time an agent is working in a productive billable mode.', 0, N'CallCenter.SelfManagement.Metric.BillableTimeSpentPercentageMetric, SelfManagement.Metric')
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [CLRType]) VALUES (14, N'NHC', N'Number of Inbound Calls Handled', N'The number of inbound calls handled by the agent.', 1, N'CallCenter.SelfManagement.Metric.InboundCallsHandledNumberMetric, SelfManagement.Metric')
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [CLRType]) VALUES (22, N'SWITCH_TM', N'Total Switch Work Time', N'The number of hours for Total Switch Work.', 1, N'CallCenter.SelfManagement.Metric.TotalSwitchWorkTimeNumberMetric, SelfManagement.Metric')
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [CLRType]) VALUES (23, N'TRANSFER_PCT', N'Percentage of calls transferred or conferenced', N'The percentage of calls transferred.', 0, N'CallCenter.SelfManagement.Metric.TransferredOrConferencedCallsPercentageMetric, SelfManagement.Metric')
SET IDENTITY_INSERT [dbo].[Metrics] OFF