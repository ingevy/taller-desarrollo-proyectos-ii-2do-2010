USE [SelfManagement]
GO

/** begin and end dates for the sample campaing **/
DECLARE @beginDate datetime
SET @beginDate = '2010-10-13 00:00:00.000'

DECLARE @endDate datetime
SET @endDate = '2010-10-31 00:00:00.000'

DECLARE @beginDate2 datetime
SET @beginDate2 = '2010-11-01 00:00:00.000'

DECLARE @endDate2 datetime
SET @endDate2 = '2010-11-30 00:00:00.000'

DECLARE @beginDate3 datetime
SET @beginDate3 = '2010-10-01 00:00:00.000'

DECLARE @endDate3 datetime
SET @endDate3 = '2010-10-12 00:00:00.000'

/****** Object:  Table [dbo].[aspnet_Applications]    Script Date: 11/07/2010 19:47:32 ******/
INSERT [dbo].[aspnet_Applications] ([ApplicationName], [LoweredApplicationName], [ApplicationId], [Description]) VALUES (N'/', N'/', N'edfa09ce-920b-4d73-8421-45569162b63f', NULL)

/****** Object:  Table [dbo].[aspnet_SchemaVersions]    Script Date: 11/07/2010 19:47:43 ******/
INSERT [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'common', N'1', 1)
INSERT [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'health monitoring', N'1', 1)
INSERT [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'membership', N'1', 1)
INSERT [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'personalization', N'1', 1)
INSERT [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'profile', N'1', 1)
INSERT [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'role manager', N'1', 1)

/****** Object:  Table [dbo].[Metrics]    Script Date: 10/17/2010 18:46:51 ******/
SET IDENTITY_INSERT [dbo].[Metrics] ON
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [IsHighestToLowest], [CLRType]) VALUES (1, N'I2C_PCT', N'Interaction to Call Percent', N'Total interactions created divided by total calls handled.', 0, 1, N'CallCenter.SelfManagement.Metric.InteractionToCallPercentMetric,SelfManagement.Metric')
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [IsHighestToLowest], [CLRType]) VALUES (13, N'INCHAIR_OCC', N'Percentage of Time Spent in Billable Mode', N'The percentage of time an agent is working in a productive billable mode.', 0, 1, N'CallCenter.SelfManagement.Metric.PercentageOfTimeSpentInBillableModeMetric,SelfManagement.Metric')
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [IsHighestToLowest], [CLRType]) VALUES (14, N'NHC', N'Number of Inbound Calls Handled', N'The number of inbound calls handled by the agent.', 1, 1, N'CallCenter.SelfManagement.Metric.NumberOfInboundCallsHandledMetric,SelfManagement.Metric')

INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [IsHighestToLowest], [CLRType]) VALUES (5, N'AUX_TM', N'Time in AUX Status', N'The total time spent in an AUX status.', 1, 0, N'CallCenter.SelfManagement.Metric.TimeInAuxStatusMetric,SelfManagement.Metric')
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [IsHighestToLowest], [CLRType]) VALUES (6, N'AVAIL_PCT', N'Available Call Status Percentage', N'Percentage of time an agent spent in an available call status.', 0, 0, N'CallCenter.SelfManagement.Metric.AvailableCallStatusPercentageMetric,SelfManagement.Metric')
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [IsHighestToLowest], [CLRType]) VALUES (8, N'AVG_ACW_TM', N'Average After Call Work', N'The average amount of time from when the agent releases the caller to the time the agent dispositions the call and returns to either a ready status or a not ready status.', 2, 0, N'CallCenter.SelfManagement.Metric.AverageAfterCallWorkMetric,SelfManagement.Metric')

INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [IsHighestToLowest], [CLRType]) VALUES (12, N'CPH', N'Number of Calls Per Hour', N'The average number of calls handled by an agent in one hour.', 2, 1, N'CallCenter.SelfManagement.Metric.NumberOfCallsPerHourMetric,SelfManagement.Metric')
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [IsHighestToLowest], [CLRType]) VALUES (18, N'QA_SCORE', N'Overall Quality Score Percent', N'The average of the agents overall quality score as a percentage.', 0, 1, N'CallCenter.SelfManagement.Metric.OverallQualityScorePercentMetric,SelfManagement.Metric')
INSERT [dbo].[Metrics] ([Id], [MetricName], [ShortDescription], [LondDescription], [Format], [IsHighestToLowest], [CLRType]) VALUES (20, N'SCHED_ADG', N'Schedule Adherence', N'The comparison of scheduled time to actual time worked.', 2, 0, N'CallCenter.SelfManagement.Metric.ScheduleAdherenceMetric,SelfManagement.Metric')
SET IDENTITY_INSERT [dbo].[Metrics] OFF

/****** Object:  Table [dbo].[Customers]    Script Date: 10/17/2010 18:46:52 ******/
SET IDENTITY_INSERT [dbo].[Customers] ON
INSERT [dbo].[Customers] ([Id], [Name]) VALUES (1, N'Banco Hipotecario')
INSERT [dbo].[Customers] ([Id], [Name]) VALUES (2, N'Banco Nación')
INSERT [dbo].[Customers] ([Id], [Name]) VALUES (3, N'Banco Provincia')
INSERT [dbo].[Customers] ([Id], [Name]) VALUES (4, N'Tarjeta Naranja')
INSERT [dbo].[Customers] ([Id], [Name]) VALUES (5, N'La Nueva Seguros')
INSERT [dbo].[Customers] ([Id], [Name]) VALUES (6, N'La Caja Seguros')
INSERT [dbo].[Customers] ([Id], [Name]) VALUES (7, N'Provincia Seguros')
SET IDENTITY_INSERT [dbo].[Customers] OFF

/****** Object:  Table [dbo].[Campaings]    Script Date: 10/17/2010 18:46:55 ******/
SET IDENTITY_INSERT [dbo].[Campaings] ON
INSERT [dbo].[Campaings] ([Id], [CustomerId], [Name], [Description], [BeginDate], [EndDate], [OptimalHourlyValue], [ObjectiveHourlyValue], [MinimumHourlyValue], [CampaingType]) VALUES (1, 5, N'Promoción La Nueva Seguros', N'Campaña para promocionar un nuevo producto del cliente La Nueva Seguros', @beginDate, @endDate, 7.9100, 3.2400, 1.6600, 1)
INSERT [dbo].[Campaings] ([Id], [CustomerId], [Name], [Description], [BeginDate], [EndDate], [OptimalHourlyValue], [ObjectiveHourlyValue], [MinimumHourlyValue], [CampaingType]) VALUES (2, 2, N'Préstamos Banco Nación', N'Campaña para promocionar prestamos a clientes preferenciales del Banco Nación', @beginDate2, @endDate2, 6.5400, 4.3800, 2.1200, 1)
INSERT [dbo].[Campaings] ([Id], [CustomerId], [Name], [Description], [BeginDate], [EndDate], [OptimalHourlyValue], [ObjectiveHourlyValue], [MinimumHourlyValue], [CampaingType]) VALUES (3, 4, N'Lanzamiento Tarjeta Naranja', N'Campaña de Lanzamiento de la Tarjeta Naranja', @beginDate3, @endDate3, 5.9200, 4.1000, 3.4000, 1)
INSERT [dbo].[Campaings] ([Id], [CustomerId], [Name], [Description], [BeginDate], [EndDate], [OptimalHourlyValue], [ObjectiveHourlyValue], [MinimumHourlyValue], [CampaingType]) VALUES (4, 7, N'Reclamos Banco Provincia', N'Campaña para atender todo tipo de reclamos de los clientes del Banco Provincia', @beginDate2, @endDate2, 6.1200, 5.3000, 4.1500, 0)
SET IDENTITY_INSERT [dbo].[Campaings] OFF

/****** Object:  Table [dbo].[aspnet_Users]    Script Date: 11/07/2010 19:47:45 ******/
INSERT [dbo].[aspnet_Users] ([ApplicationId], [UserId], [InnerUserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'e3c0a5be-e7fc-4197-9827-4ba75bf8a9a3', 13, N'Administrator', N'administrator', NULL, 0, CAST(0x00009DFD01593C23 AS DateTime))
INSERT [dbo].[aspnet_Users] ([ApplicationId], [UserId], [InnerUserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'328c5a9a-c099-4245-b4a4-66b61a9c0cc7', 9, N'Gonzalo.Farias', N'gonzalo.farias', NULL, 0, CAST(0x00009E270175FE98 AS DateTime))
INSERT [dbo].[aspnet_Users] ([ApplicationId], [UserId], [InnerUserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'24b12ff3-d7ed-45e4-ba56-1e0a1ec4adc8', 24, N'john.doe', N'john.doe', NULL, 0, CAST(0x00009E1201664914 AS DateTime))
INSERT [dbo].[aspnet_Users] ([ApplicationId], [UserId], [InnerUserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'cab8467d-5792-43e3-a499-ab66dd9f1d79', 2, N'Jose.Flores', N'jose.flores', NULL, 0, CAST(0x00009E270175FE59 AS DateTime))
INSERT [dbo].[aspnet_Users] ([ApplicationId], [UserId], [InnerUserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'f1d63044-f0bc-4bd3-8e6c-0b4054f8c6f8', 1, N'Juan.Perez', N'juan.perez', NULL, 0, CAST(0x00009E270175FDB2 AS DateTime))
INSERT [dbo].[aspnet_Users] ([ApplicationId], [UserId], [InnerUserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'f496cbeb-4f24-43a3-adcf-9aa393966762', 3, N'Lorena.Garcia', N'lorena.garcia', NULL, 0, CAST(0x00009E270175FE65 AS DateTime))
INSERT [dbo].[aspnet_Users] ([ApplicationId], [UserId], [InnerUserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'4ccdc1f0-b743-45d6-acfc-d05d6cba7e2a', 7, N'Lorena.Gomez', N'lorena.gomez', NULL, 0, CAST(0x00009E270175FE7D AS DateTime))
INSERT [dbo].[aspnet_Users] ([ApplicationId], [UserId], [InnerUserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'55da1a2e-010c-4099-9d5a-b80d3f3f7d5b', 8, N'Martin.Martinez', N'martin.martinez', NULL, 0, CAST(0x00009E270175FE8C AS DateTime))
INSERT [dbo].[aspnet_Users] ([ApplicationId], [UserId], [InnerUserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'7b641be6-8859-4156-a59e-aca5522e28a6', 4, N'Matias.Gonzalez', N'matias.gonzalez', NULL, 0, CAST(0x00009E270175FE71 AS DateTime))
INSERT [dbo].[aspnet_Users] ([ApplicationId], [UserId], [InnerUserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'4ff99f57-d24c-427c-9012-9a1bcd209947', 6, N'sample_user_1', N'sample_user_1', NULL, 0, CAST(0x00009E1201667D2D AS DateTime))
INSERT [dbo].[aspnet_Users] ([ApplicationId], [UserId], [InnerUserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'8f3b8810-fb75-4512-95de-b835a9bead20', 12, N'sample_user_2', N'sample_user_2', NULL, 0, CAST(0x00009E1201517C27 AS DateTime))
INSERT [dbo].[aspnet_Users] ([ApplicationId], [UserId], [InnerUserId], [UserName], [LoweredUserName], [MobileAlias], [IsAnonymous], [LastActivityDate]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'07739e87-01c1-4284-9dc4-584c2969f3b2', 10, N'Violeta.Rivero', N'violeta.rivero', NULL, 0, CAST(0x00009E270175FEA1 AS DateTime))

/****** Object:  Table [dbo].[aspnet_Roles]    Script Date: 11/07/2010 19:47:45 ******/
INSERT [dbo].[aspnet_Roles] ([ApplicationId], [RoleId], [RoleName], [LoweredRoleName], [Description]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'8afe09e1-2767-4d10-accf-812c70bbf224', N'AccountManager', N'accountmanager', NULL)
INSERT [dbo].[aspnet_Roles] ([ApplicationId], [RoleId], [RoleName], [LoweredRoleName], [Description]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'71484297-a405-476c-a3c3-f903255eedfc', N'Agent', N'agent', NULL)
INSERT [dbo].[aspnet_Roles] ([ApplicationId], [RoleId], [RoleName], [LoweredRoleName], [Description]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'797bd93e-ec2c-4992-a989-12fe81af0a4c', N'ITManager', N'itmanager', NULL)
INSERT [dbo].[aspnet_Roles] ([ApplicationId], [RoleId], [RoleName], [LoweredRoleName], [Description]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'd7b9d5b7-17a1-4b15-b6d7-c67a6da57c00', N'Supervisor', N'supervisor', NULL)

/****** Object:  Table [dbo].[aspnet_Profile]    Script Date: 11/07/2010 19:47:45 ******/
INSERT [dbo].[aspnet_Profile] ([UserId], [PropertyNames], [PropertyValuesString], [PropertyValuesBinary], [LastUpdatedDate]) VALUES (N'f1d63044-f0bc-4bd3-8e6c-0b4054f8c6f8', N'Workday:S:0:3:IncorporationDate:S:3:10:Surname:S:13:5:Names:S:18:4:GrossSalary:S:22:4:DNI:S:26:8:Status:S:34:7:', N'PTE13/02/2009PerezJuan200030345235 activo', 0x, CAST(0x00009E270175FDB2 AS DateTime))
INSERT [dbo].[aspnet_Profile] ([UserId], [PropertyNames], [PropertyValuesString], [PropertyValuesBinary], [LastUpdatedDate]) VALUES (N'07739e87-01c1-4284-9dc4-584c2969f3b2', N'Workday:S:0:3:IncorporationDate:S:3:10:Surname:S:13:6:Names:S:19:7:GrossSalary:S:26:4:DNI:S:30:8:Status:S:38:7:', N'PTE30/06/2009RiveroVioleta180032564224 activo', 0x, CAST(0x00009E270175FEA1 AS DateTime))
INSERT [dbo].[aspnet_Profile] ([UserId], [PropertyNames], [PropertyValuesString], [PropertyValuesBinary], [LastUpdatedDate]) VALUES (N'328c5a9a-c099-4245-b4a4-66b61a9c0cc7', N'Workday:S:0:3:IncorporationDate:S:3:10:Surname:S:13:6:Names:S:19:7:GrossSalary:S:26:4:DNI:S:30:8:Status:S:38:7:', N'PTE30/06/2009FariasGonzalo180029456789 activo', 0x, CAST(0x00009E270175FE98 AS DateTime))
INSERT [dbo].[aspnet_Profile] ([UserId], [PropertyNames], [PropertyValuesString], [PropertyValuesBinary], [LastUpdatedDate]) VALUES (N'4ff99f57-d24c-427c-9012-9a1bcd209947', N'Workday:S:0:3:IncorporationDate:S:3:10:Surname:S:13:5:Names:S:18:9:DNI:S:27:8:Status:S:35:6:', N'FTE09/01/2007LopezJose Luis30325134activo', 0x, CAST(0x00009E1201517C21 AS DateTime))
INSERT [dbo].[aspnet_Profile] ([UserId], [PropertyNames], [PropertyValuesString], [PropertyValuesBinary], [LastUpdatedDate]) VALUES (N'f496cbeb-4f24-43a3-adcf-9aa393966762', N'Workday:S:0:3:IncorporationDate:S:3:10:Surname:S:13:6:Names:S:19:6:GrossSalary:S:25:4:DNI:S:29:8:Status:S:37:7:', N'PTE13/02/2009GarciaLorena200034877111 activo', 0x, CAST(0x00009E270175FE65 AS DateTime))
INSERT [dbo].[aspnet_Profile] ([UserId], [PropertyNames], [PropertyValuesString], [PropertyValuesBinary], [LastUpdatedDate]) VALUES (N'cab8467d-5792-43e3-a499-ab66dd9f1d79', N'Workday:S:0:3:IncorporationDate:S:3:10:Surname:S:13:6:Names:S:19:4:GrossSalary:S:23:4:DNI:S:27:8:Status:S:35:7:', N'PTE13/02/2009FloresJose200032877122 activo', 0x, CAST(0x00009E270175FE59 AS DateTime))
INSERT [dbo].[aspnet_Profile] ([UserId], [PropertyNames], [PropertyValuesString], [PropertyValuesBinary], [LastUpdatedDate]) VALUES (N'7b641be6-8859-4156-a59e-aca5522e28a6', N'Workday:S:0:3:IncorporationDate:S:3:10:Surname:S:13:8:Names:S:21:6:GrossSalary:S:27:4:DNI:S:31:8:Status:S:39:7:', N'PTE13/02/2009GonzalezMatias200029111333 activo', 0x, CAST(0x00009E270175FE71 AS DateTime))
INSERT [dbo].[aspnet_Profile] ([UserId], [PropertyNames], [PropertyValuesString], [PropertyValuesBinary], [LastUpdatedDate]) VALUES (N'55da1a2e-010c-4099-9d5a-b80d3f3f7d5b', N'Workday:S:0:3:IncorporationDate:S:3:10:Surname:S:13:8:Names:S:21:6:GrossSalary:S:27:4:DNI:S:31:8:Status:S:39:7:', N'PTE30/06/2009MartinezMartin180031474235 activo', 0x, CAST(0x00009E270175FE8C AS DateTime))
INSERT [dbo].[aspnet_Profile] ([UserId], [PropertyNames], [PropertyValuesString], [PropertyValuesBinary], [LastUpdatedDate]) VALUES (N'8f3b8810-fb75-4512-95de-b835a9bead20', N'Workday:S:0:3:Surname:S:3:7:Names:S:10:6:GrossSalary:S:16:4:DNI:S:20:8:Status:S:28:6:', N'FTERamirezMartin250030357963activo', 0x, CAST(0x00009E1201517C27 AS DateTime))
INSERT [dbo].[aspnet_Profile] ([UserId], [PropertyNames], [PropertyValuesString], [PropertyValuesBinary], [LastUpdatedDate]) VALUES (N'4ccdc1f0-b743-45d6-acfc-d05d6cba7e2a', N'Workday:S:0:3:IncorporationDate:S:3:10:Surname:S:13:5:Names:S:18:6:GrossSalary:S:24:4:DNI:S:28:8:Status:S:36:7:', N'FTE15/04/2009GomezLorena190028723090 activo', 0x, CAST(0x00009E270175FE7D AS DateTime))

/****** Object:  Table [dbo].[aspnet_Membership]    Script Date: 11/07/2010 19:47:45 ******/
INSERT [dbo].[aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'e3c0a5be-e7fc-4197-9827-4ba75bf8a9a3', N'6Njljdbm6qQGxNj1u/jg40vE2/4=', 1, N'ic7GW1lX408qKLMSloGcKA==', NULL, N'administrator@callcenter.com', N'administrator@callcenter.com', NULL, NULL, 1, 0, CAST(0x00009DFD01593B4C AS DateTime), CAST(0x00009DFD01593C23 AS DateTime), CAST(0x00009DFD01593B4C AS DateTime), CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), NULL)
INSERT [dbo].[aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'328c5a9a-c099-4245-b4a4-66b61a9c0cc7', N'SjrJ/qrutyPdbrNtptn16egE+/c=', 1, N'bg+zFhb8YgHh4cgrC1tFQA==', NULL, N'Gonzalo.Farias@selfmanagement.com', N'gonzalo.farias@selfmanagement.com', NULL, NULL, 1, 0, CAST(0x00009E270175FE30 AS DateTime), CAST(0x00009E270175FE30 AS DateTime), CAST(0x00009E270175FE30 AS DateTime), CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), NULL)
INSERT [dbo].[aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'24b12ff3-d7ed-45e4-ba56-1e0a1ec4adc8', N'epZFyGLMtJylFXZ0SmlAU99zxPw=', 1, N'G0ZWLLgnIr0tujllfDyfEQ==', NULL, N'john.doe@callcenter.com', N'john.doe@callcenter.com', NULL, NULL, 1, 0, CAST(0x00009DFD01597134 AS DateTime), CAST(0x00009E1201664914 AS DateTime), CAST(0x00009DFD01597134 AS DateTime), CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), NULL)
INSERT [dbo].[aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'cab8467d-5792-43e3-a499-ab66dd9f1d79', N'dhYPAvnLJsGB2YZXqLT+m3QGkVI=', 1, N'J/hKLNQ49eWa3mxWOlWcQg==', NULL, N'Jose.Flores@selfmanagement.com', N'jose.flores@selfmanagement.com', NULL, NULL, 1, 0, CAST(0x00009E270175FE30 AS DateTime), CAST(0x00009E270175FE30 AS DateTime), CAST(0x00009E270175FE30 AS DateTime), CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), NULL)
INSERT [dbo].[aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'f1d63044-f0bc-4bd3-8e6c-0b4054f8c6f8', N'cLxyfBaFTU1TpBJk5m6SJ3aNZUU=', 1, N'7JLyLshPCG7mCUTHcIqwPQ==', NULL, N'Juan.Perez@selfmanagement.com', N'juan.perez@selfmanagement.com', NULL, NULL, 1, 0, CAST(0x00009E270175FD04 AS DateTime), CAST(0x00009E270175FD04 AS DateTime), CAST(0x00009E270175FD04 AS DateTime), CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), NULL)
INSERT [dbo].[aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'f496cbeb-4f24-43a3-adcf-9aa393966762', N'F0HXMzjQrBTpGaEzilvOOYsWTZk=', 1, N'YOT6y0mHN6+LoM7LPF9mnw==', NULL, N'Lorena.Garcia@selfmanagement.com', N'lorena.garcia@selfmanagement.com', NULL, NULL, 1, 0, CAST(0x00009E270175FE30 AS DateTime), CAST(0x00009E270175FE30 AS DateTime), CAST(0x00009E270175FE30 AS DateTime), CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), NULL)
INSERT [dbo].[aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'4ccdc1f0-b743-45d6-acfc-d05d6cba7e2a', N'Cs8Es3PSFTzjh/zuMVKMVot1Uyo=', 1, N'Cw0KJFNQ9JhnbH+8wBDSRQ==', NULL, N'Lorena.Gomez@selfmanagement.com', N'lorena.gomez@selfmanagement.com', NULL, NULL, 1, 0, CAST(0x00009E270175FE30 AS DateTime), CAST(0x00009E270175FE30 AS DateTime), CAST(0x00009E270175FE30 AS DateTime), CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), NULL)
INSERT [dbo].[aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'55da1a2e-010c-4099-9d5a-b80d3f3f7d5b', N'btPmwceYaCKW0vqdBzRWni9Sdvg=', 1, N'NkJt0ho79LmhMqPx8l//Sg==', NULL, N'Martin.Martinez@selfmanagement.com', N'martin.martinez@selfmanagement.com', NULL, NULL, 1, 0, CAST(0x00009E270175FE30 AS DateTime), CAST(0x00009E270175FE30 AS DateTime), CAST(0x00009E270175FE30 AS DateTime), CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), NULL)
INSERT [dbo].[aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'7b641be6-8859-4156-a59e-aca5522e28a6', N'Y5xgTaNRRsFBjkhQ5+ebI7Kf/ug=', 1, N'bWbpMj3cjhKL1kxctEpD2w==', NULL, N'Matias.Gonzalez@selfmanagement.com', N'matias.gonzalez@selfmanagement.com', NULL, NULL, 1, 0, CAST(0x00009E270175FE30 AS DateTime), CAST(0x00009E270175FE30 AS DateTime), CAST(0x00009E270175FE30 AS DateTime), CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), NULL)
INSERT [dbo].[aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'4ff99f57-d24c-427c-9012-9a1bcd209947', N'h+lFKiqi8xd8vzTh/njZaLVLw6s=', 1, N'vNWlJXAYevcCdZg8VBDhJw==', NULL, N'sample_user_1@callcenter.com', N'sample_user_1@callcenter.com', NULL, NULL, 1, 0, CAST(0x00009E1201517B50 AS DateTime), CAST(0x00009E1201667D2D AS DateTime), CAST(0x00009E1201517B50 AS DateTime), CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), NULL)
INSERT [dbo].[aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'8f3b8810-fb75-4512-95de-b835a9bead20', N'+beHgJi8csN9sjPJg9xHXLwAvqY=', 1, N'dp8qxMGyDhTdFwtvPehqUw==', NULL, N'sample_user_2@callcenter.com', N'sample_user_2@callcenter.com', NULL, NULL, 1, 0, CAST(0x00009E1201517B50 AS DateTime), CAST(0x00009E1201517B50 AS DateTime), CAST(0x00009E1201517B50 AS DateTime), CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), NULL)
INSERT [dbo].[aspnet_Membership] ([ApplicationId], [UserId], [Password], [PasswordFormat], [PasswordSalt], [MobilePIN], [Email], [LoweredEmail], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStart], [FailedPasswordAnswerAttemptCount], [FailedPasswordAnswerAttemptWindowStart], [Comment]) VALUES (N'edfa09ce-920b-4d73-8421-45569162b63f', N'07739e87-01c1-4284-9dc4-584c2969f3b2', N'FENprKtvNH0/QJTcbdgSkw8Srj8=', 1, N'T8i+5sD9Qi81BI/Hrn5quQ==', NULL, N'Violeta.Rivero@selfmanagement.com', N'violeta.rivero@selfmanagement.com', NULL, NULL, 1, 0, CAST(0x00009E270175FE30 AS DateTime), CAST(0x00009E270175FE30 AS DateTime), CAST(0x00009E270175FE30 AS DateTime), CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), NULL)

/****** Object:  Table [dbo].[aspnet_UsersInRoles]    Script Date: 11/07/2010 19:47:45 ******/
INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'e3c0a5be-e7fc-4197-9827-4ba75bf8a9a3', N'797bd93e-ec2c-4992-a989-12fe81af0a4c')
INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'24b12ff3-d7ed-45e4-ba56-1e0a1ec4adc8', N'8afe09e1-2767-4d10-accf-812c70bbf224')
INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'4ff99f57-d24c-427c-9012-9a1bcd209947', N'd7b9d5b7-17a1-4b15-b6d7-c67a6da57c00')
INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'8f3b8810-fb75-4512-95de-b835a9bead20', N'd7b9d5b7-17a1-4b15-b6d7-c67a6da57c00')
INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'f1d63044-f0bc-4bd3-8e6c-0b4054f8c6f8', N'71484297-a405-476c-a3c3-f903255eedfc')
INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'07739e87-01c1-4284-9dc4-584c2969f3b2', N'71484297-a405-476c-a3c3-f903255eedfc')
INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'328c5a9a-c099-4245-b4a4-66b61a9c0cc7', N'71484297-a405-476c-a3c3-f903255eedfc')
INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'f496cbeb-4f24-43a3-adcf-9aa393966762', N'71484297-a405-476c-a3c3-f903255eedfc')
INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'cab8467d-5792-43e3-a499-ab66dd9f1d79', N'71484297-a405-476c-a3c3-f903255eedfc')
INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'7b641be6-8859-4156-a59e-aca5522e28a6', N'71484297-a405-476c-a3c3-f903255eedfc')
INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'55da1a2e-010c-4099-9d5a-b80d3f3f7d5b', N'71484297-a405-476c-a3c3-f903255eedfc')
INSERT [dbo].[aspnet_UsersInRoles] ([UserId], [RoleId]) VALUES (N'4ccdc1f0-b743-45d6-acfc-d05d6cba7e2a', N'71484297-a405-476c-a3c3-f903255eedfc')

/****** Object:  Table [dbo].[CampaingUsers]    Script Date: 11/07/2010 19:47:45 ******/
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (1, 1, @beginDate, @endDate)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (1, 2, @beginDate, @endDate)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (1, 3, @beginDate, @endDate)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (1, 4, @beginDate, @endDate)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (1, 6, @beginDate, @endDate)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (1, 7, @beginDate, @endDate)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (1, 8, @beginDate, @endDate)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (1, 9, @beginDate, @endDate)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (1, 10, @beginDate, @endDate)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (1, 12, @beginDate, @endDate)

INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (2, 1, @beginDate2, @endDate2)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (2, 2, @beginDate2, @endDate2)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (2, 3, @beginDate2, @endDate2)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (2, 4, @beginDate2, @endDate2)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (2, 6, @beginDate2, @endDate2)

INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (3, 1, @beginDate3, @endDate3)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (3, 2, @beginDate3, @endDate3)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (3, 3, @beginDate3, @endDate3)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (3, 4, @beginDate3, @endDate3)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (3, 6, @beginDate3, @endDate3)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (3, 7, @beginDate3, @endDate3)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (3, 8, @beginDate3, @endDate3)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (3, 9, @beginDate3, @endDate3)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (3, 10, @beginDate3, @endDate3)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (3, 12, @beginDate3, @endDate3)

INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (4, 7, @beginDate2, @endDate2)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (4, 8, @beginDate2, @endDate2)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (4, 9, @beginDate2, @endDate2)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (4, 10, @beginDate2, @endDate2)
INSERT [dbo].[CampaingUsers] ([CampaingId], [InnerUserId], [BeginDate], [EndDate]) VALUES (4, 12, @beginDate2, @endDate2)

/****** Object:  Table [dbo].[SupervisorAgents]    Script Date: 10/17/2010 18:46:55 ******/
INSERT [dbo].[SupervisorAgents] ([AgentId], [SupervisorId]) VALUES (1, 6)
INSERT [dbo].[SupervisorAgents] ([AgentId], [SupervisorId]) VALUES (2, 6)
INSERT [dbo].[SupervisorAgents] ([AgentId], [SupervisorId]) VALUES (3, 6)
INSERT [dbo].[SupervisorAgents] ([AgentId], [SupervisorId]) VALUES (4, 6)
INSERT [dbo].[SupervisorAgents] ([AgentId], [SupervisorId]) VALUES (7, 12)
INSERT [dbo].[SupervisorAgents] ([AgentId], [SupervisorId]) VALUES (8, 12)
INSERT [dbo].[SupervisorAgents] ([AgentId], [SupervisorId]) VALUES (9, 12)
INSERT [dbo].[SupervisorAgents] ([AgentId], [SupervisorId]) VALUES (10, 12)

/****** Object:  Table [dbo].[CampaingMetricLevels]    Script Date: 10/17/2010 18:46:55 ******/
INSERT [dbo].[CampaingMetricLevels] ([CampaingId], [MetricId], [OptimalLevel], [ObjectiveLevel], [MinimumLevel], [Enabled]) VALUES (1, 1, 92, 86, 77, 1)
INSERT [dbo].[CampaingMetricLevels] ([CampaingId], [MetricId], [OptimalLevel], [ObjectiveLevel], [MinimumLevel], [Enabled]) VALUES (1, 13, 95, 85, 78, 1)
INSERT [dbo].[CampaingMetricLevels] ([CampaingId], [MetricId], [OptimalLevel], [ObjectiveLevel], [MinimumLevel], [Enabled]) VALUES (1, 14, 300, 250, 200, 1)

INSERT [dbo].[CampaingMetricLevels] ([CampaingId], [MetricId], [OptimalLevel], [ObjectiveLevel], [MinimumLevel], [Enabled]) VALUES (2, 5, 195, 220, 295, 1)
INSERT [dbo].[CampaingMetricLevels] ([CampaingId], [MetricId], [OptimalLevel], [ObjectiveLevel], [MinimumLevel], [Enabled]) VALUES (2, 6, 18, 21, 25, 1)
INSERT [dbo].[CampaingMetricLevels] ([CampaingId], [MetricId], [OptimalLevel], [ObjectiveLevel], [MinimumLevel], [Enabled]) VALUES (2, 8, 1, 1.5, 2.2, 1)

INSERT [dbo].[CampaingMetricLevels] ([CampaingId], [MetricId], [OptimalLevel], [ObjectiveLevel], [MinimumLevel], [Enabled]) VALUES (3, 12, 8, 6, 4, 1)
INSERT [dbo].[CampaingMetricLevels] ([CampaingId], [MetricId], [OptimalLevel], [ObjectiveLevel], [MinimumLevel], [Enabled]) VALUES (3, 18, 90, 85, 80, 1)
INSERT [dbo].[CampaingMetricLevels] ([CampaingId], [MetricId], [OptimalLevel], [ObjectiveLevel], [MinimumLevel], [Enabled]) VALUES (3, 20, 40, 50, 75, 1)

INSERT [dbo].[CampaingMetricLevels] ([CampaingId], [MetricId], [OptimalLevel], [ObjectiveLevel], [MinimumLevel], [Enabled]) VALUES (4, 5, 198, 215, 285, 1)
INSERT [dbo].[CampaingMetricLevels] ([CampaingId], [MetricId], [OptimalLevel], [ObjectiveLevel], [MinimumLevel], [Enabled]) VALUES (4, 6, 12, 19, 23, 1)
INSERT [dbo].[CampaingMetricLevels] ([CampaingId], [MetricId], [OptimalLevel], [ObjectiveLevel], [MinimumLevel], [Enabled]) VALUES (4, 8, 0.95, 1.2, 1.95, 1)

/****** Object:  Table [dbo].[Holidays]    Script Date: 10/19/2010 23:02:31 ******/
INSERT INTO [dbo].[Holidays] ([Date], [Description]) VALUES (N'2010-01-01', N'Año Nuevo')
INSERT INTO [dbo].[Holidays] ([Date], [Description]) VALUES (N'2010-03-24', N'Día Nacional de la Memoria por la Verdad y la Justicia (Ley N° 26.085)')
INSERT INTO [dbo].[Holidays] ([Date], [Description]) VALUES (N'2010-04-01', N'Jueves Santo Festividad Cristiana')
INSERT INTO [dbo].[Holidays] ([Date], [Description]) VALUES (N'2010-04-02', N'Viernes Santo - Día del Veterano y de los Caídos en la Guerra de Malvinas (Ley N° 26.110)')
INSERT INTO [dbo].[Holidays] ([Date], [Description]) VALUES (N'2010-05-01', N'Día del Trabajador')
INSERT INTO [dbo].[Holidays] ([Date], [Description]) VALUES (N'2010-05-24', N'Feriado Nacional')
INSERT INTO [dbo].[Holidays] ([Date], [Description]) VALUES (N'2010-05-25', N'Primer Gobierno Patrio')
INSERT INTO [dbo].[Holidays] ([Date], [Description]) VALUES (N'2010-06-21', N'Paso a la Inmortalidad del General Manuel Belgrano')
INSERT INTO [dbo].[Holidays] ([Date], [Description]) VALUES (N'2010-07-09', N'Día de la Independencia')
INSERT INTO [dbo].[Holidays] ([Date], [Description]) VALUES (N'2010-08-16', N'Paso a la Inmortalidad del General José de San Martín')
INSERT INTO [dbo].[Holidays] ([Date], [Description]) VALUES (N'2010-10-11', N'Dia de la Raza')
INSERT INTO [dbo].[Holidays] ([Date], [Description]) VALUES (N'2010-10-27', N'Censo Nacional')
INSERT INTO [dbo].[Holidays] ([Date], [Description]) VALUES (N'2010-12-08', N'Inmaculada Concepción de María')
INSERT INTO [dbo].[Holidays] ([Date], [Description]) VALUES (N'2010-12-25', N'Navidad')
INSERT INTO [dbo].[Holidays] ([Date], [Description]) VALUES (N'2011-01-01', N'Año Nuevo')