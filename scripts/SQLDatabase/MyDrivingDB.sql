SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

/****** Object: Table [dbo].[UserProfiles] Script Date: 3/16/2016 12:24:46 PM ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[UserProfiles] (
    [Id]                NVARCHAR (128)     NOT NULL,
    [FirstName]         NVARCHAR (MAX)     NULL,
    [LastName]          NVARCHAR (MAX)     NULL,
    [UserId]            NVARCHAR (MAX)     NULL,
    [ProfilePictureUri] NVARCHAR (MAX)     NULL,
    [Rating]            INT                NOT NULL,
    [Ranking]           INT                NOT NULL,
    [TotalDistance]     FLOAT (53)         NOT NULL,
    [TotalTrips]        BIGINT             NOT NULL,
    [TotalTime]         BIGINT             NOT NULL,
    [HardStops]         BIGINT             NOT NULL,
    [HardAccelerations] BIGINT             NOT NULL,
    [FuelConsumption]   FLOAT (53)         NOT NULL,
    [MaxSpeed]          FLOAT (53)         NOT NULL,
    [Version]           ROWVERSION         NOT NULL,
    [CreatedAt]         DATETIMEOFFSET (7) NOT NULL,
    [UpdatedAt]         DATETIMEOFFSET (7) NULL,
    [Deleted]           BIT                NOT NULL
);

CREATE CLUSTERED INDEX [IX_CreatedAt]
    ON [dbo].[UserProfiles]([CreatedAt] ASC);

ALTER TABLE [dbo].[UserProfiles]
    ADD CONSTRAINT [PK_dbo.UserProfiles] PRIMARY KEY NONCLUSTERED ([Id] ASC);
END


/****** Object: Table [dbo].[POIs] Script Date: 3/16/2016 12:22:01 PM ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[POIs]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[POIs] (
    [Id]        NVARCHAR (128)     NOT NULL,
    [TripId]    NVARCHAR (MAX)     NULL,
    [Latitude]  FLOAT (53)         NOT NULL,
    [Longitude] FLOAT (53)         NOT NULL,
    [POIType]   INT                NOT NULL,
    [Version]   ROWVERSION         NOT NULL,
    [CreatedAt] DATETIMEOFFSET (7) NOT NULL,
    [UpdatedAt] DATETIMEOFFSET (7) NULL,
    [Deleted]   BIT                NOT NULL,
    [Timestamp] DATETIME           NOT NULL
);

CREATE CLUSTERED INDEX [IX_CreatedAt]
    ON [dbo].[POIs]([CreatedAt] ASC);

ALTER TABLE [dbo].[POIs]
    ADD CONSTRAINT [PK_dbo.POIs] PRIMARY KEY NONCLUSTERED ([Id] ASC);
END

/****** Object: Table [dbo].[Devices] Script Date: 3/16/2016 12:22:52 PM ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Devices]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Devices] (
    [Id]             NVARCHAR (128)     NOT NULL,
    [Name]           NVARCHAR (MAX)     NULL,
    [Version]        ROWVERSION         NOT NULL,
    [CreatedAt]      DATETIMEOFFSET (7) NOT NULL,
    [UpdatedAt]      DATETIMEOFFSET (7) NULL,
    [Deleted]        BIT                NOT NULL,
    [UserProfile_Id] NVARCHAR (128)     NULL
);

CREATE CLUSTERED INDEX [IX_CreatedAt]
    ON [dbo].[Devices]([CreatedAt] ASC);

CREATE NONCLUSTERED INDEX [IX_UserProfile_Id]
    ON [dbo].[Devices]([UserProfile_Id] ASC);

ALTER TABLE [dbo].[Devices]
    ADD CONSTRAINT [PK_dbo.Devices] PRIMARY KEY NONCLUSTERED ([Id] ASC);

ALTER TABLE [dbo].[Devices]
    ADD CONSTRAINT [FK_dbo.Devices_dbo.UserProfiles_UserProfile_Id] FOREIGN KEY ([UserProfile_Id]) REFERENCES [dbo].[UserProfiles] ([Id]);
END

/****** Object: Table [dbo].[IOTHubDatas] Script Date: 3/16/2016 12:23:35 PM ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IOTHubDatas]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[IOTHubDatas] (
    [Id]        NVARCHAR (128)     NOT NULL,
    [Version]   ROWVERSION         NOT NULL,
    [CreatedAt] DATETIMEOFFSET (7) NOT NULL,
    [UpdatedAt] DATETIMEOFFSET (7) NULL,
    [Deleted]   BIT                NOT NULL
);

CREATE CLUSTERED INDEX [IX_CreatedAt]
    ON [dbo].[IOTHubDatas]([CreatedAt] ASC);

ALTER TABLE [dbo].[IOTHubDatas]
    ADD CONSTRAINT [PK_dbo.IOTHubDatas] PRIMARY KEY NONCLUSTERED ([Id] ASC);
END

/****** Object: Table [dbo].[Trips] Script Date: 3/16/2016 12:24:25 PM ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Trips]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Trips] (
    [Id]                  NVARCHAR (128)     NOT NULL,
    [Name]                NVARCHAR (MAX)     NULL,
    [UserId]              NVARCHAR (MAX)     NULL,
    [RecordedTimeStamp]   DATETIME           NOT NULL,
    [EndTimeStamp]        DATETIME           NOT NULL,
    [Rating]              INT                NOT NULL,
    [IsComplete]          BIT                NOT NULL,
    [HasSimulatedOBDData] BIT                NOT NULL,
    [AverageSpeed]        FLOAT (53)         NOT NULL,
    [FuelUsed]            FLOAT (53)         NOT NULL,
    [HardStops]           BIGINT             NOT NULL,
    [HardAccelerations]   BIGINT             NOT NULL,
    [MainPhotoUrl]        NVARCHAR (MAX)     NULL,
    [Distance]            FLOAT (53)         NOT NULL,
    [Version]             ROWVERSION         NOT NULL,
    [CreatedAt]           DATETIMEOFFSET (7) NOT NULL,
    [UpdatedAt]           DATETIMEOFFSET (7) NULL,
    [Deleted]             BIT                NOT NULL
);

CREATE CLUSTERED INDEX [IX_CreatedAt]
    ON [dbo].[Trips]([CreatedAt] ASC);

ALTER TABLE [dbo].[Trips]
    ADD CONSTRAINT [PK_dbo.Trips] PRIMARY KEY NONCLUSTERED ([Id] ASC);
END

/****** Object: Table [dbo].[TripPoints] Script Date: 3/16/2016 12:23:53 PM ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IOTHubDatas]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TripPoints] (
    [Id]                           NVARCHAR (128)     NOT NULL,
    [TripId]                       NVARCHAR (128)     NULL,
    [Latitude]                     FLOAT (53)         NOT NULL,
    [Longitude]                    FLOAT (53)         NOT NULL,
    [Speed]                        FLOAT (53)         NOT NULL,
    [RecordedTimeStamp]            DATETIME           NOT NULL,
    [Sequence]                     INT                NOT NULL,
    [RPM]                          FLOAT (53)         NOT NULL,
    [ShortTermFuelBank]            FLOAT (53)         NOT NULL,
    [LongTermFuelBank]             FLOAT (53)         NOT NULL,
    [ThrottlePosition]             FLOAT (53)         NOT NULL,
    [RelativeThrottlePosition]     FLOAT (53)         NOT NULL,
    [Runtime]                      FLOAT (53)         NOT NULL,
    [DistanceWithMalfunctionLight] FLOAT (53)         NOT NULL,
    [EngineLoad]                   FLOAT (53)         NOT NULL,
    [MassFlowRate]                 FLOAT (53)         NOT NULL,
    [EngineFuelRate]               FLOAT (53)         NOT NULL,
    [VIN]                          NVARCHAR (MAX)     NULL,
    [HasOBDData]                   BIT                NOT NULL,
    [HasSimulatedOBDData]          BIT                NOT NULL,
    [Version]                      ROWVERSION         NOT NULL,
    [CreatedAt]                    DATETIMEOFFSET (7) NOT NULL,
    [UpdatedAt]                    DATETIMEOFFSET (7) NULL,
    [Deleted]                      BIT                NOT NULL
);

CREATE CLUSTERED INDEX [IX_CreatedAt]
    ON [dbo].[TripPoints]([CreatedAt] ASC);

CREATE NONCLUSTERED INDEX [IX_TripId]
    ON [dbo].[TripPoints]([TripId] ASC);

ALTER TABLE [dbo].[TripPoints]
    ADD CONSTRAINT [PK_dbo.TripPoints] PRIMARY KEY NONCLUSTERED ([Id] ASC);

ALTER TABLE [dbo].[TripPoints]
    ADD CONSTRAINT [FK_dbo.TripPoints_dbo.Trips_TripId] FOREIGN KEY ([TripId]) REFERENCES [dbo].[Trips] ([Id]);
END
