set hive.execution.engine=tez;

DROP VIEW IF EXISTS tripDataWithoutNulls;
CREATE VIEW tripDataWithoutNulls as 
SELECT TripId,
UserId,
vin,
RecordedTimeStamp,
IF(Lat==-255,NULL,Lat) as Lat,
IF(Lon==-255,NULL,Lon) as Lon,
IF(Speed<0,NULL,Speed) as Speed,
IF(EngineLoad==-255,NULL,EngineLoad) as EngineLoad,
IF(EngineRPM==-255,NULL,EngineRPM) as EngineRPM,
IF(DistanceWithMIL<0,NULL,DistanceWithMIL) as DistanceWithMIL,
IF(Runtime<0,NULL,Runtime) as Runtime
FROM tripdata 
WHERE year=${hiveconf:Year} and month=${hiveconf:Month} and day=${hiveconf:Day}
and TripId is not null and TripId!=''
and UserId is not null and UserId!='';


-- Averaging the data to 1 event per sec per trip
DROP TABLE IF EXISTS tripDataInt;
CREATE TABLE tripDataInt 
(
TripId string,
UserId string,
vin string,
unixtimestamp bigint,
cLat double,
cLon double,
AverageSpeed double,
EngineLoad double,
EngineRPM double,
DistanceWithMIL double,
Runtime int
) LOCATION 'wasb://tripdata@${hiveconf:DataStorageAccount}.blob.core.windows.net/tables/onesecagg';

TRUNCATE TABLE tripDataInt;

INSERT INTO TABLE tripDataInt 
SELECT TripId,
UserId,
vin,
UNIX_TIMESTAMP(CONCAT(SPLIT(RecordedTimeStamp,'T')[0],' ',SUBSTR(SPLIT(RecordedTimeStamp,'T')[1],0,8)),'yyyy-MM-dd hh:mm:ss') as unixtimestamp,
AVG(Lat) as cLat,
AVG(Lon) as cLon,
AVG(Speed) as AverageSpeed,
MAX(EngineLoad) as EngineLoad,
MAX(EngineRPM) as EngineRPM,
MAX(DistanceWithMIL) as DistanceWithMIL,
MAX(Runtime) as Runtime
FROM tripdata 
WHERE RecordedTimeStamp is not null
GROUP BY TripId, UserId, vin, UNIX_TIMESTAMP(CONCAT(SPLIT(RecordedTimeStamp,'T')[0],' ',SUBSTR(SPLIT(RecordedTimeStamp,'T')[1],0,8)),'yyyy-MM-dd hh:mm:ss');


-- year=${hiveconf:Year} and month=${hiveconf:Month} and day=${hiveconf:Day}
