/****** Object:  Table [dbo].[Devices]    Script Date: 3/24/2016 7:53:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Devices]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Devices](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Version] [timestamp] NOT NULL,
	[CreatedAt] [datetimeoffset](7) NOT NULL,
	[UpdatedAt] [datetimeoffset](7) NULL,
	[Deleted] [bit] NOT NULL,
	[UserProfile_Id] [nvarchar](128) NULL,
 CONSTRAINT [PK_dbo.Devices] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
/****** Object:  Index [IX_CreatedAt]    Script Date: 3/24/2016 7:53:08 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Devices]') AND name = N'IX_CreatedAt')
CREATE CLUSTERED INDEX [IX_CreatedAt] ON [dbo].[Devices]
(
	[CreatedAt] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Table [dbo].[factMLOutputData]    Script Date: 3/24/2016 7:53:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[factMLOutputData]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[factMLOutputData](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[tripId] [nvarchar](50) NULL,
	[userId] [nvarchar](50) NULL,
	[tripstarttime] [nvarchar](50) NULL,
	[driverType] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
/****** Object:  Table [dbo].[IOTHubDatas]    Script Date: 3/24/2016 7:53:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IOTHubDatas]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[IOTHubDatas](
	[Id] [nvarchar](128) NOT NULL,
	[Version] [timestamp] NOT NULL,
	[CreatedAt] [datetimeoffset](7) NOT NULL,
	[UpdatedAt] [datetimeoffset](7) NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.IOTHubDatas] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
/****** Object:  Index [IX_CreatedAt]    Script Date: 3/24/2016 7:53:09 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[IOTHubDatas]') AND name = N'IX_CreatedAt')
CREATE CLUSTERED INDEX [IX_CreatedAt] ON [dbo].[IOTHubDatas]
(
	[CreatedAt] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Table [dbo].[POIs]    Script Date: 3/24/2016 7:53:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[POIs]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[POIs](
	[Id] [nvarchar](128) NOT NULL,
	[TripId] [nvarchar](max) NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[POIType] [int] NOT NULL,
	[RecordedTimeStamp] [nvarchar](50) NULL,
	[Version] [timestamp] NOT NULL,
	[CreatedAt] [datetimeoffset](7) NOT NULL,
	[UpdatedAt] [datetimeoffset](7) NULL,
	[Deleted] [bit] NOT NULL,
	[Timestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.POIs] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
/****** Object:  Table [dbo].[TripPoints]    Script Date: 3/24/2016 7:53:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TripPoints]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TripPoints](
	[Id] [nvarchar](128) NOT NULL,
	[TripId] [nvarchar](128) NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[Speed] [float] NOT NULL,
	[RecordedTimeStamp] [datetime] NOT NULL,
	[Sequence] [int] NOT NULL,
	[RPM] [float] NOT NULL,
	[ShortTermFuelBank] [float] NOT NULL,
	[LongTermFuelBank] [float] NOT NULL,
	[ThrottlePosition] [float] NOT NULL,
	[RelativeThrottlePosition] [float] NOT NULL,
	[Runtime] [float] NOT NULL,
	[DistanceWithMalfunctionLight] [float] NOT NULL,
	[EngineLoad] [float] NOT NULL,
	[MassFlowRate] [float] NOT NULL,
	[EngineFuelRate] [float] NOT NULL,
	[VIN] [nvarchar](max) NULL,
	[HasOBDData] [bit] NOT NULL,
	[HasSimulatedOBDData] [bit] NOT NULL,
	[Version] [timestamp] NOT NULL,
	[CreatedAt] [datetimeoffset](7) NOT NULL,
	[UpdatedAt] [datetimeoffset](7) NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.TripPoints] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
/****** Object:  Index [IX_CreatedAt]    Script Date: 3/24/2016 7:53:13 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[TripPoints]') AND name = N'IX_CreatedAt')
CREATE CLUSTERED INDEX [IX_CreatedAt] ON [dbo].[TripPoints]
(
	[CreatedAt] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Table [dbo].[Trips]    Script Date: 3/24/2016 7:53:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Trips]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Trips](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[UserId] [nvarchar](max) NULL,
	[RecordedTimeStamp] [datetime] NOT NULL,
	[EndTimeStamp] [datetime] NOT NULL,
	[Rating] [int] NOT NULL,
	[IsComplete] [bit] NOT NULL,
	[HasSimulatedOBDData] [bit] NOT NULL,
	[AverageSpeed] [float] NOT NULL,
	[FuelUsed] [float] NOT NULL,
	[HardStops] [bigint] NOT NULL,
	[HardAccelerations] [bigint] NOT NULL,
	[MainPhotoUrl] [nvarchar](max) NULL,
	[Distance] [float] NOT NULL,
	[Version] [timestamp] NOT NULL,
	[CreatedAt] [datetimeoffset](7) NOT NULL,
	[UpdatedAt] [datetimeoffset](7) NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Trips] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
/****** Object:  Index [IX_CreatedAt]    Script Date: 3/24/2016 7:53:15 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Trips]') AND name = N'IX_CreatedAt')
CREATE CLUSTERED INDEX [IX_CreatedAt] ON [dbo].[Trips]
(
	[CreatedAt] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Table [dbo].[UserProfiles]    Script Date: 3/24/2016 7:53:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[UserProfiles](
	[Id] [nvarchar](128) NOT NULL,
	[FirstName] [nvarchar](max) NULL,
	[LastName] [nvarchar](max) NULL,
	[UserId] [nvarchar](max) NULL,
	[ProfilePictureUri] [nvarchar](max) NULL,
	[Rating] [int] NOT NULL,
	[Ranking] [int] NOT NULL,
	[TotalDistance] [float] NOT NULL,
	[TotalTrips] [bigint] NOT NULL,
	[TotalTime] [bigint] NOT NULL,
	[HardStops] [bigint] NOT NULL,
	[HardAccelerations] [bigint] NOT NULL,
	[FuelConsumption] [float] NOT NULL,
	[MaxSpeed] [float] NOT NULL,
	[Version] [timestamp] NOT NULL,
	[CreatedAt] [datetimeoffset](7) NOT NULL,
	[UpdatedAt] [datetimeoffset](7) NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.UserProfiles] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
/****** Object:  Index [IX_CreatedAt]    Script Date: 3/24/2016 7:53:17 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND name = N'IX_CreatedAt')
CREATE CLUSTERED INDEX [IX_CreatedAt] ON [dbo].[UserProfiles]
(
	[CreatedAt] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UserProfile_Id]    Script Date: 3/24/2016 7:53:17 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Devices]') AND name = N'IX_UserProfile_Id')
CREATE NONCLUSTERED INDEX [IX_UserProfile_Id] ON [dbo].[Devices]
(
	[UserProfile_Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_TripId]    Script Date: 3/24/2016 7:53:17 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[TripPoints]') AND name = N'IX_TripId')
CREATE NONCLUSTERED INDEX [IX_TripId] ON [dbo].[TripPoints]
(
	[TripId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Devices__Id__4E88ABD4]') AND type = 'D')
IF (SELECT object_definition(default_object_id) AS definition FROM sys.columns WHERE name ='Id' AND object_id = object_id('dbo.Devices')) IS NULL
BEGIN
ALTER TABLE [dbo].[Devices] ADD  DEFAULT (newid()) FOR [Id]
END

GO
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Devices__Created__4F7CD00D]') AND type = 'D')
IF (SELECT object_definition(default_object_id) AS definition FROM sys.columns WHERE name ='CreatedAt' AND object_id = object_id('dbo.Devices')) IS NULL
BEGIN
ALTER TABLE [dbo].[Devices] ADD  DEFAULT (sysutcdatetime()) FOR [CreatedAt]
END

GO
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__IOTHubDatas__Id__36B12243]') AND type = 'D')
IF (SELECT object_definition(default_object_id) AS definition FROM sys.columns WHERE name ='Id' AND object_id = object_id('dbo.IOTHubDatas')) IS NULL
BEGIN
ALTER TABLE [dbo].[IOTHubDatas] ADD  DEFAULT (newid()) FOR [Id]
END

GO
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__IOTHubDat__Creat__37A5467C]') AND type = 'D')
IF (SELECT object_definition(default_object_id) AS definition FROM sys.columns WHERE name ='CreatedAt' AND object_id = object_id('dbo.IOTHubDatas')) IS NULL
BEGIN
ALTER TABLE [dbo].[IOTHubDatas] ADD  DEFAULT (sysutcdatetime()) FOR [CreatedAt]
END

GO
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__POIs__Id__3B75D760]') AND type = 'D')
IF (SELECT object_definition(default_object_id) AS definition FROM sys.columns WHERE name ='Id' AND object_id = object_id('dbo.POIs')) IS NULL
BEGIN
ALTER TABLE [dbo].[POIs] ADD  DEFAULT (newid()) FOR [Id]
END

GO
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__POIs__CreatedAt__3C69FB99]') AND type = 'D')
IF (SELECT object_definition(default_object_id) AS definition FROM sys.columns WHERE name ='CreatedAt' AND object_id = object_id('dbo.POIs')) IS NULL
BEGIN
ALTER TABLE [dbo].[POIs] ADD  DEFAULT (sysutcdatetime()) FOR [CreatedAt]
END

GO
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__POIs__Timestamp__5535A963]') AND type = 'D')
IF (SELECT object_definition(default_object_id) AS definition FROM sys.columns WHERE name ='Timestamp' AND object_id = object_id('dbo.POIs')) IS NULL
BEGIN
ALTER TABLE [dbo].[POIs] ADD  DEFAULT ('1900-01-01T00:00:00.000') FOR [Timestamp]
END

GO
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__TripPoints__Id__403A8C7D]') AND type = 'D')
IF (SELECT object_definition(default_object_id) AS definition FROM sys.columns WHERE name ='Id' AND object_id = object_id('dbo.TripPoints')) IS NULL
BEGIN
ALTER TABLE [dbo].[TripPoints] ADD  DEFAULT (newid()) FOR [Id]
END

GO
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__TripPoint__Creat__412EB0B6]') AND type = 'D')
IF (SELECT object_definition(default_object_id) AS definition FROM sys.columns WHERE name ='CreatedAt' AND object_id = object_id('dbo.TripPoints')) IS NULL
BEGIN
ALTER TABLE [dbo].[TripPoints] ADD  DEFAULT (sysutcdatetime()) FOR [CreatedAt]
END

GO
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Trips__Id__44FF419A]') AND type = 'D')
IF (SELECT object_definition(default_object_id) AS definition FROM sys.columns WHERE name ='Id' AND object_id = object_id('dbo.Trips')) IS NULL
BEGIN
ALTER TABLE [dbo].[Trips] ADD  DEFAULT (newid()) FOR [Id]
END

GO
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Trips__CreatedAt__45F365D3]') AND type = 'D')
IF (SELECT object_definition(default_object_id) AS definition FROM sys.columns WHERE name ='CreatedAt' AND object_id = object_id('dbo.Trips')) IS NULL
BEGIN
ALTER TABLE [dbo].[Trips] ADD  DEFAULT (sysutcdatetime()) FOR [CreatedAt]
END

GO
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__UserProfiles__Id__49C3F6B7]') AND type = 'D')
IF (SELECT object_definition(default_object_id) AS definition FROM sys.columns WHERE name ='Id' AND object_id = object_id('dbo.UserProfiles')) IS NULL
BEGIN
ALTER TABLE [dbo].[UserProfiles] ADD  DEFAULT (newid()) FOR [Id]
END

GO
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__UserProfi__Creat__4AB81AF0]') AND type = 'D')
IF (SELECT object_definition(default_object_id) AS definition FROM sys.columns WHERE name ='CreatedAt' AND object_id = object_id('dbo.UserProfiles')) IS NULL
BEGIN
ALTER TABLE [dbo].[UserProfiles] ADD  DEFAULT (sysutcdatetime()) FOR [CreatedAt]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.Devices_dbo.UserProfiles_UserProfile_Id]') AND parent_object_id = OBJECT_ID(N'[dbo].[Devices]'))
ALTER TABLE [dbo].[Devices]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Devices_dbo.UserProfiles_UserProfile_Id] FOREIGN KEY([UserProfile_Id])
REFERENCES [dbo].[UserProfiles] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.Devices_dbo.UserProfiles_UserProfile_Id]') AND parent_object_id = OBJECT_ID(N'[dbo].[Devices]'))
ALTER TABLE [dbo].[Devices] CHECK CONSTRAINT [FK_dbo.Devices_dbo.UserProfiles_UserProfile_Id]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.TripPoints_dbo.Trips_TripId]') AND parent_object_id = OBJECT_ID(N'[dbo].[TripPoints]'))
ALTER TABLE [dbo].[TripPoints]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TripPoints_dbo.Trips_TripId] FOREIGN KEY([TripId])
REFERENCES [dbo].[Trips] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.TripPoints_dbo.Trips_TripId]') AND parent_object_id = OBJECT_ID(N'[dbo].[TripPoints]'))
ALTER TABLE [dbo].[TripPoints] CHECK CONSTRAINT [FK_dbo.TripPoints_dbo.Trips_TripId]
GO
/****** Object:  Trigger [dbo].[TR_dbo_Devices_InsertUpdateDelete]    Script Date: 3/24/2016 7:53:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TR_dbo_Devices_InsertUpdateDelete]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TR_dbo_Devices_InsertUpdateDelete] ON [dbo].[Devices] AFTER INSERT, UPDATE, DELETE AS BEGIN UPDATE [dbo].[Devices] SET [dbo].[Devices].[UpdatedAt] = CONVERT(DATETIMEOFFSET, SYSUTCDATETIME()) FROM INSERTED WHERE inserted.[Id] = [dbo].[Devices].[Id] END' 
GO
ALTER TABLE [dbo].[Devices] ENABLE TRIGGER [TR_dbo_Devices_InsertUpdateDelete]
GO
/****** Object:  Trigger [dbo].[UpdateRatings]    Script Date: 3/24/2016 7:53:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[UpdateRatings]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[UpdateRatings] ON [dbo].[factMLOutputData]
FOR INSERT
AS
        -- Do it for all rows (maybe bulk insert) inserted
        DECLARE crs CURSOR FOR
                SELECT userId FROM inserted
        
        DECLARE @UId varchar(100)
        
        OPEN crs
        FETCH NEXT FROM crs INTO @UId
        WHILE @@FETCH_STATUS = 0
        BEGIN

				WITH all_scores AS (
						SELECT userId, COUNT(driverType) as a_s FROM dbo.factMLOutputData GROUP BY userId), 
					good_scores AS (
						SELECT userId, COUNT(driverType) as g_s FROM dbo.factMLOutputData WHERE driverType = ''Good'' GROUP BY userId), 
					ratings AS (
						SELECT all_scores.userId as ui, CAST( (100 * g_s)/a_s AS INT) as r FROM all_scores, good_scores WHERE all_scores.userId = good_scores.userId)
				
				UPDATE UserProfiles SET UserProfiles.Rating = ratings.r FROM ratings WHERE UserProfiles.UserId = ratings.ui AND UserProfiles.UserId = @UId;
        
               FETCH NEXT FROM crs INTO @UId
        END
        CLOSE crs
        DEALLOCATE crs' 
GO
ALTER TABLE [dbo].[factMLOutputData] ENABLE TRIGGER [UpdateRatings]
GO
/****** Object:  Trigger [dbo].[TR_dbo_IOTHubDatas_InsertUpdateDelete]    Script Date: 3/24/2016 7:53:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TR_dbo_IOTHubDatas_InsertUpdateDelete]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TR_dbo_IOTHubDatas_InsertUpdateDelete] ON [dbo].[IOTHubDatas] AFTER INSERT, UPDATE, DELETE AS BEGIN UPDATE [dbo].[IOTHubDatas] SET [dbo].[IOTHubDatas].[UpdatedAt] = CONVERT(DATETIMEOFFSET, SYSUTCDATETIME()) FROM INSERTED WHERE inserted.[Id] = [dbo].[IOTHubDatas].[Id] END' 
GO
ALTER TABLE [dbo].[IOTHubDatas] ENABLE TRIGGER [TR_dbo_IOTHubDatas_InsertUpdateDelete]
GO
/****** Object:  Trigger [dbo].[UpdateUserProfilesOnInsert]    Script Date: 3/24/2016 7:53:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[UpdateUserProfilesOnInsert]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[UpdateUserProfilesOnInsert] ON [dbo].[POIs]
FOR INSERT
AS
        -- Do it for all rows inserted (maybe bulk insert)
        DECLARE crs CURSOR FOR
                SELECT TripId FROM inserted
        
        DECLARE @TId nvarchar(100)
        
        OPEN crs
        FETCH NEXT FROM crs INTO @TId
        WHILE @@FETCH_STATUS = 0
        BEGIN

               -- Update Accelerations      
               WITH ascnt1 AS (
                                  SELECT        p.TripId, up.UserId, COUNT(p.POIType) AS cnt1
                                  FROM            dbo.POIs AS p INNER JOIN
                                                                           dbo.Trips AS t ON p.TripId = t.Id INNER JOIN
                                                                           dbo.UserProfiles AS up ON t.UserId = up.UserId
                                  WHERE        (p.POIType = 1)
                                  GROUP BY p.TripId, up.UserId
               )       
               UPDATE dbo.UserProfiles SET dbo.UserProfiles.HardAccelerations = ascnt1.cnt1 FROM ascnt1
               WHERE ascnt1.TripId = @TId AND dbo.UserProfiles.UserId = ascnt1.UserId;
        
                           -- Update Hard Stops
               WITH ascnt2 AS (
                                  SELECT        p.TripId, up.UserId, COUNT(p.POIType) AS cnt2
                                  FROM            dbo.POIs AS p INNER JOIN
                                                                           dbo.Trips AS t ON p.TripId = t.Id INNER JOIN
                                                                           dbo.UserProfiles AS up ON t.UserId = up.UserId
                                  WHERE        (p.POIType = 2)
                                  GROUP BY p.TripId, up.UserId
               )       
               UPDATE dbo.UserProfiles SET dbo.UserProfiles.HardStops = ascnt2.cnt2 FROM ascnt2
               WHERE ascnt2.TripId = @TId AND dbo.UserProfiles.UserId = ascnt2.UserId;
        
        
               FETCH NEXT FROM crs INTO @TId
        END
        CLOSE crs
        DEALLOCATE crs' 
GO
ALTER TABLE [dbo].[POIs] ENABLE TRIGGER [UpdateUserProfilesOnInsert]
GO
/****** Object:  Trigger [dbo].[TR_dbo_TripPoints_InsertUpdateDelete]    Script Date: 3/24/2016 7:53:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TR_dbo_TripPoints_InsertUpdateDelete]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TR_dbo_TripPoints_InsertUpdateDelete] ON [dbo].[TripPoints] AFTER INSERT, UPDATE, DELETE AS BEGIN UPDATE [dbo].[TripPoints] SET [dbo].[TripPoints].[UpdatedAt] = CONVERT(DATETIMEOFFSET, SYSUTCDATETIME()) FROM INSERTED WHERE inserted.[Id] = [dbo].[TripPoints].[Id] END' 
GO
ALTER TABLE [dbo].[TripPoints] ENABLE TRIGGER [TR_dbo_TripPoints_InsertUpdateDelete]
GO
/****** Object:  Trigger [dbo].[TR_dbo_Trips_InsertUpdateDelete]    Script Date: 3/24/2016 7:53:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TR_dbo_Trips_InsertUpdateDelete]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TR_dbo_Trips_InsertUpdateDelete] ON [dbo].[Trips] AFTER INSERT, UPDATE, DELETE AS BEGIN UPDATE [dbo].[Trips] SET [dbo].[Trips].[UpdatedAt] = CONVERT(DATETIMEOFFSET, SYSUTCDATETIME()) FROM INSERTED WHERE inserted.[Id] = [dbo].[Trips].[Id] END' 
GO
ALTER TABLE [dbo].[Trips] ENABLE TRIGGER [TR_dbo_Trips_InsertUpdateDelete]
GO
/****** Object:  Trigger [dbo].[TR_dbo_UserProfiles_InsertUpdateDelete]    Script Date: 3/24/2016 7:53:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TR_dbo_UserProfiles_InsertUpdateDelete]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TR_dbo_UserProfiles_InsertUpdateDelete] ON [dbo].[UserProfiles] AFTER INSERT, UPDATE, DELETE AS BEGIN UPDATE [dbo].[UserProfiles] SET [dbo].[UserProfiles].[UpdatedAt] = CONVERT(DATETIMEOFFSET, SYSUTCDATETIME()) FROM INSERTED WHERE inserted.[Id] = [dbo].[UserProfiles].[Id] END' 
GO
ALTER TABLE [dbo].[UserProfiles] ENABLE TRIGGER [TR_dbo_UserProfiles_InsertUpdateDelete]
GO
