SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

/****** Object: Table [dbo].[dimUser] Script Date: 3/18/2016 9:25:46 AM ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dimUser]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[dimUser] (
    [userId] NVARCHAR (100) NOT NULL,
    [vinNum] NVARCHAR (20)  NULL
);
END

/****** Object: Table [dbo].[dimUserTemp] Script Date: 3/18/2016 9:26:02 AM ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dimUserTemp]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[dimUserTemp] (
    [userId] NVARCHAR (100) NOT NULL,
    [vinNum] NVARCHAR (20)  NOT NULL
);
END

/****** Object: Table [dbo].[factMLOutputData] Script Date: 3/18/2016 9:27:10 AM ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[factMLOutputData]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[factMLOutputData] (
    [tripId]     NVARCHAR (200) NOT NULL,
    [userId]     NVARCHAR (100) NOT NULL,
    [driverType] NVARCHAR (10)  NOT NULL
);
END

/****** Object: Table [dbo].[factTripData] Script Date: 3/18/2016 9:27:36 AM ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[factTripData]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[factTripData] (
    [tripId]          NVARCHAR (200) NOT NULL,
    [userId]          NVARCHAR (100) NOT NULL,
    [vin]             NVARCHAR (20)  NULL,
    [driverType]      NVARCHAR (10)  NULL,
    [avgSpeed]        FLOAT (53)     NULL,
    [numOfHardAccel]  INT            NULL,
    [numOfHardBrakes] INT            NULL,
    [wasMILOn]        BIT            NULL,
    [tripTime]        INT            NULL,
    [cLatitude]       FLOAT (53)     NULL,
    [cLongitude]      FLOAT (53)     NULL
);
END

/****** Object: Table [dbo].[factTripDataTemp] Script Date: 3/18/2016 9:27:46 AM ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[factTripDataTemp]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[factTripDataTemp] (
    [tripId]          NVARCHAR (200) NOT NULL,
    [userId]          NVARCHAR (100) NOT NULL,
    [vin]             NVARCHAR (20)  NULL,
    [avgSpeed]        FLOAT (53)     NULL,
    [numOfHardAccel]  INT            NULL,
    [numOfHardBrakes] INT            NULL,
    [wasMILOn]        BIT            NULL,
    [tripTime]        INT            NULL,
    [cLat]            FLOAT (53)     NULL,
    [cLon]            FLOAT (53)     NULL
);
END

/****** Object: Table [dbo].[dimVinLookup] Script Date: 3/18/2016 9:26:21 AM ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dimVinLookup]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[dimVinLookup] (
    [vinNum]  NVARCHAR (20)  NOT NULL,
    [make]    NVARCHAR (200) NULL,
    [model]   NVARCHAR (200) NULL,
    [carYear] INT            NULL,
    [carType] NVARCHAR (200) NULL
);

/****** Populate [dbo].[dimVinLookup] table - Script Date: 3/18/2016 9:27:46 AM ******/
INSERT INTO [dbo].[dimVinLookup] ([vinNum], [make], [model], [carYear], [carType]) VALUES (N'2HKRM4H39D0000000', N'Honda', N'Honda', 2013, N'SUV')
INSERT INTO [dbo].[dimVinLookup] ([vinNum], [make], [model], [carYear], [carType]) VALUES (N'2HKRM4H39DH614569', N'Honda', N'Honda', 2013, N'SUV')
INSERT INTO [dbo].[dimVinLookup] ([vinNum], [make], [model], [carYear], [carType]) VALUES (N'3FA6P0H74G0000000', N'Ford', N'Fusion', 2016, N'Passenger')
INSERT INTO [dbo].[dimVinLookup] ([vinNum], [make], [model], [carYear], [carType]) VALUES (N'3FA6P0H74GR330716', N'Ford', N'Fusion', 2016, N'Passenger')
INSERT INTO [dbo].[dimVinLookup] ([vinNum], [make], [model], [carYear], [carType]) VALUES (N'3FA6P0H76GR319796', N'Ford', N'Fusion', 2016, N'Passenger')
INSERT INTO [dbo].[dimVinLookup] ([vinNum], [make], [model], [carYear], [carType]) VALUES (N'3VWGG71K870000000', N'Volkswagen', N'Unknown', 2007, N'Unknown')
INSERT INTO [dbo].[dimVinLookup] ([vinNum], [make], [model], [carYear], [carType]) VALUES (N'3VWGG71K87M096130', N'Volkswagen', N'Unknown', 2007, N'Unknown')
INSERT INTO [dbo].[dimVinLookup] ([vinNum], [make], [model], [carYear], [carType]) VALUES (N'JHLRM4H70C0000000', N'Honda', N'Honda', 2012, N'SUV')
INSERT INTO [dbo].[dimVinLookup] ([vinNum], [make], [model], [carYear], [carType]) VALUES (N'JHLRM4H70CC002017', N'Honda', N'Honda', 2012, N'SUV')
INSERT INTO [dbo].[dimVinLookup] ([vinNum], [make], [model], [carYear], [carType]) VALUES (N'SIMULATORANDROID1', N'Unknown', N'Unknown', 2010, N'Unknown')
INSERT INTO [dbo].[dimVinLookup] ([vinNum], [make], [model], [carYear], [carType]) VALUES (N'WBA3A5G51F0000000', N'BMW', N'Unknown', 2015, N'Unknown')
INSERT INTO [dbo].[dimVinLookup] ([vinNum], [make], [model], [carYear], [carType]) VALUES (N'WBA3A5G51FNS90945', N'BMW', N'Unknown', 2015, N'Unknown')
INSERT INTO [dbo].[dimVinLookup] ([vinNum], [make], [model], [carYear], [carType]) VALUES (N'WBA3B9G59E0000000', N'BMW', N'Unknown', 2014, N'Unknown')
END
