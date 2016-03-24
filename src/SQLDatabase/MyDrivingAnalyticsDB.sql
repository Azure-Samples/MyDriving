/****** Object:  Table [dbo].[dimUser]    Script Date: 3/22/2016 7:39:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dimUser]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[dimUser](
	[userId] [nvarchar](100) NOT NULL,
	[vin] [nvarchar](20) NOT NULL,
	[driverType] [nvarchar](10) NULL,
 CONSTRAINT [PK_dimUser] PRIMARY KEY CLUSTERED 
(
	[userId] ASC,
	[vin] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
/****** Object:  Table [dbo].[dimUserTemp]    Script Date: 3/22/2016 7:39:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dimUserTemp]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[dimUserTemp](
	[userId] [nvarchar](100) NOT NULL,
	[vin] [nvarchar](20) NULL
)
END
GO
/****** Object:  Table [dbo].[dimVinLookup]    Script Date: 3/22/2016 7:39:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dimVinLookup]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[dimVinLookup](
	[vinNum] [nvarchar](20) NOT NULL,
	[make] [nvarchar](200) NULL,
	[model] [nvarchar](200) NULL,
	[carYear] [int] NULL,
	[carType] [nvarchar](200) NULL,
 CONSTRAINT [PK_dimVinLookup] PRIMARY KEY CLUSTERED 
(
	[vinNum] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
/****** Object:  Table [dbo].[factMLOutputData]    Script Date: 3/22/2016 7:39:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[factMLOutputData]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[factMLOutputData](
	[tripId] [nvarchar](200) NOT NULL,
	[userId] [nvarchar](100) NOT NULL,
	[tripstarttime] [datetime] NOT NULL,
	[driverType] [nvarchar](10) NOT NULL
)
END
GO
/****** Object:  Table [dbo].[factTripData]    Script Date: 3/22/2016 7:39:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[factTripData]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[factTripData](
	[TripId] [nvarchar](200) NOT NULL,
	[UserId] [nvarchar](100) NOT NULL,
	[vin] [nvarchar](20) NULL,
	[tripStartTime] [datetime] NULL,
	[driverType] [nvarchar](10) NULL,
	[AverageSpeed] [float] NULL,
	[Hard_Accel] [int] NULL,
	[Hard_Brakes] [int] NULL,
	[DroveWithMILOn] [bit] NULL,
	[LengthOfTrip] [int] NULL,
	[cLatitude] [float] NULL,
	[cLongitude] [float] NULL,
 CONSTRAINT [PK_factTripData] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[TripId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
/****** Object:  Table [dbo].[factTripDataTemp]    Script Date: 3/22/2016 7:39:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[factTripDataTemp]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[factTripDataTemp](
	[TripId] [nvarchar](200) NOT NULL,
	[UserId] [nvarchar](100) NOT NULL,
	[vin] [nvarchar](20) NULL,
	[tripStartTime] [nvarchar](50) NULL,
	[AverageSpeed] [float] NULL,
	[Hard_Accel] [int] NULL,
	[Hard_Brakes] [int] NULL,
	[DroveWithMILOn] [bit] NULL,
	[LengthOfTrip] [int] NULL,
	[cLat] [float] NULL,
	[cLon] [float] NULL
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dimUser]') AND parent_object_id = OBJECT_ID(N'[dbo].[dimUser]'))
ALTER TABLE [dbo].[dimUser]  WITH CHECK ADD  CONSTRAINT [FK_dimUser] FOREIGN KEY([vin])
REFERENCES [dbo].[dimVinLookup] ([vinNum])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dimUser]') AND parent_object_id = OBJECT_ID(N'[dbo].[dimUser]'))
ALTER TABLE [dbo].[dimUser] CHECK CONSTRAINT [FK_dimUser]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_facTripData]') AND parent_object_id = OBJECT_ID(N'[dbo].[factTripData]'))
ALTER TABLE [dbo].[factTripData]  WITH CHECK ADD  CONSTRAINT [FK_facTripData] FOREIGN KEY([UserId], [vin])
REFERENCES [dbo].[dimUser] ([userId], [vin])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_facTripData]') AND parent_object_id = OBJECT_ID(N'[dbo].[factTripData]'))
ALTER TABLE [dbo].[factTripData] CHECK CONSTRAINT [FK_facTripData]
GO
/****** Object:  StoredProcedure [dbo].[sp_cleanupTempTables]    Script Date: 3/22/2016 7:39:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_cleanupTempTables]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_cleanupTempTables] AS' 
END
GO
ALTER PROCEDURE [dbo].[sp_cleanupTempTables]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
TRUNCATE TABLE dbo.factTripDataTemp;
TRUNCATE TABLE dbo.dimUserTemp;
TRUNCATE TABLE dbo.factMLOutputData;
END

GO
/****** Object:  StoredProcedure [dbo].[sp_mergeDimUser]    Script Date: 3/22/2016 7:39:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_mergeDimUser]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_mergeDimUser] AS' 
END
GO
ALTER PROCEDURE [dbo].[sp_mergeDimUser] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

DECLARE @vinTable table(
 vin nvarchar(20)
);

INSERT INTO @vinTable
 SELECT distinct x.vin 
		FROM dbo.dimUserTemp x 
		WHERE x.vin IS NOT NULL
		AND x.vin != ''
		AND vin != '-255'
		AND x.vin NOT IN (SELECT 
			                 distinct vinNum 
							 FROM dbo.dimVinLookup);

	DECLARE @vinCount int;
	SELECT @vinCount = COUNT(*) FROM @vinTable;


	IF (@vinCount > 0)
	BEGIN
		INSERT INTO dbo.dimVinLookup 
		SELECT distinct vin, 'Unknown','Unknown',1995,'Unknown' 
		from dbo.dimUserTemp
		WHERE vin IS NOT NULL 
		AND vin != ''
		AND vin != '-255'
		AND vin NOT IN (SELECT distinct vinNum from dbo.dimVinLookup);
	END
	ELSE
		Begin
			SELECT * from dbo.dimVinLookup;
		End

	INSERT into dbo.dimUser (userId, vin)
	SELECT distinct a.userId, a.vin from dbo.dimUserTemp a
		WHERE a.userId IS NOT NULL
		AND a.vin IS NOT NULL
		AND vin != ''
		AND vin != '-255'
		AND userId != ''
		AND userId != '-255'
		AND CONCAT(a.userId,'_',a.vin) NOT IN (SELECT distinct CONCAT(userId,'_',vin) FROM dbo.dimUser)
		AND a.vin in (SELECT distinct vinNum from dbo.dimVinLookup);
END

GO
/****** Object:  StoredProcedure [dbo].[sp_mergeFactTripData]    Script Date: 3/22/2016 7:39:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_mergeFactTripData]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_mergeFactTripData] AS' 
END
GO
ALTER PROCEDURE [dbo].[sp_mergeFactTripData] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
DECLARE @userCount int;
DECLARE @userTable table(
 userId nvarchar(100) NOT NULL,
 vin nvarchar(20)
);
DECLARE @vinTable table(
  vin nvarchar(20)
);

INSERT INTO @vinTable
 SELECT distinct x.vin 
		FROM dbo.factTripDataTemp x 
		WHERE x.vin IS NOT NULL
		AND x.vin != ''
		AND vin != '-255'
		AND x.vin NOT IN (SELECT 
			                 distinct vinNum 
							 FROM dbo.dimVinLookup);

	DECLARE @vinCount int;
	SELECT @vinCount = COUNT(*) FROM @vinTable;

IF (@vinCount > 0)
 BEGIN
   INSERT INTO dbo.dimVinLookup 
   SELECT distinct vin, 'Unknown','Unknown',1995,'Unknown' 
   from dbo.factTripDataTemp
   WHERE vin IS NOT NULL 
		AND vin != ''
		AND vin != '-255'
		AND vin not in (SELECT distinct vinNum from dbo.dimVinLookup);
 END

INSERT INTO @userTable
SELECT distinct userId, vin from dbo.factTripDataTemp x 
WHERE userId IS NOT NULL
AND userId != ''
AND userID != '-255'
AND vin IS NOT NULL
AND vin != ''
AND vin != '-255'
AND CONCAT(x.userId,'_',x.vin) not in (SELECT distinct CONCAT(userId,'_',vin) FROM dbo.dimUser);

SELECT @userCount = count(*)
FROM @userTable;

IF (@userCount > 0)
 BEGIN
   INSERT INTO dbo.dimUser (userId, vin)
   SELECT *
   FROM @userTable;
 END

INSERT INTO dbo.factTripData
SELECT distinct
	a.tripId,
	a.userId,
	MIN(a.vin),
	MIN(a.tripStartTime),
	MIN(b.driverType),
	MAX(a.AverageSpeed),
	MAX(a.Hard_Accel),
	MAX(a.Hard_Brakes),
	CAST(MAX(CAST(a.DroveWithMILOn as int)) as BIT),
	MAX(a.LengthOfTrip),
	AVG(a.cLat),
	AVG(a.cLon) 
FROM dbo.factTripDataTemp a JOIN dbo.factMLOutputData b
ON a.tripId = b.tripId AND a.userId = b.userId
WHERE a.tripId is not null
AND a.userId is not null
AND a.userId != ''
AND a.userID != '-255'
AND a.vin IS NOT NULL
AND a.vin != ''
AND a.vin != '-255'
AND CONCAT(a.userId,'_',a.vin) IN (SELECT distinct CONCAT(userId,'_',vin) FROM dbo.dimUser)
AND a.vin IN (SELECT distinct vinNum from dbo.dimVinLookup)
AND b.driverType is not NULL
AND a.tripId not in (SELECT distinct tripId from dbo.factTripData)
GROUP BY a.tripId,
	a.userId;

END

GO
