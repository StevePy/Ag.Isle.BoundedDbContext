
/****** Object:  Table [dbo].[Lookups]    Script Date: 23/08/2024 2:08:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Lookups](
	[LookupId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[LastModAt] [datetime] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Lookups] PRIMARY KEY CLUSTERED 
(
	[LookupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Lookups] ON 
GO
INSERT [dbo].[Lookups] ([LookupId], [Name], [Description], [IsActive], [CreatedAt], [LastModAt]) VALUES (1, N'Test Value #1', NULL, 1, CAST(N'2024-08-23T14:06:18.790' AS DateTime), CAST(N'2024-08-23T14:06:18.790' AS DateTime))
GO
INSERT [dbo].[Lookups] ([LookupId], [Name], [Description], [IsActive], [CreatedAt], [LastModAt]) VALUES (2, N'Test Value #2', NULL, 1, CAST(N'2024-08-23T14:06:24.443' AS DateTime), CAST(N'2024-08-23T14:06:24.443' AS DateTime))
GO
INSERT [dbo].[Lookups] ([LookupId], [Name], [Description], [IsActive], [CreatedAt], [LastModAt]) VALUES (3, N'Test Value #3', NULL, 1, CAST(N'2024-08-23T14:06:29.617' AS DateTime), CAST(N'2024-08-23T14:06:29.617' AS DateTime))
GO
INSERT [dbo].[Lookups] ([LookupId], [Name], [Description], [IsActive], [CreatedAt], [LastModAt]) VALUES (4, N'Test Value #4', NULL, 1, CAST(N'2024-08-23T14:06:34.820' AS DateTime), CAST(N'2024-08-23T14:06:34.820' AS DateTime))
GO
INSERT [dbo].[Lookups] ([LookupId], [Name], [Description], [IsActive], [CreatedAt], [LastModAt]) VALUES (5, N'Test Value #5', NULL, 1, CAST(N'2024-08-23T14:06:45.397' AS DateTime), CAST(N'2024-08-23T14:06:45.397' AS DateTime))
GO
INSERT [dbo].[Lookups] ([LookupId], [Name], [Description], [IsActive], [CreatedAt], [LastModAt]) VALUES (6, N'Test Value #6', NULL, 1, CAST(N'2024-08-23T14:06:51.173' AS DateTime), CAST(N'2024-08-23T14:06:51.173' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[Lookups] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Lookups_Name]    Script Date: 23/08/2024 2:08:07 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Lookups_Name] ON [dbo].[Lookups]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Lookups] ADD  CONSTRAINT [DF_Lookups_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Lookups] ADD  CONSTRAINT [DF_Lookups_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Lookups] ADD  CONSTRAINT [DF_Lookups_LastModAt]  DEFAULT (getdate()) FOR [LastModAt]
GO
