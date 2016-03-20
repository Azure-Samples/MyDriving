set hive.execution.engine=tez;

-- Averaging the data to 1 event per sec per trip
DROP TABLE IF EXISTS tripDataInt;
CREATE TABLE tripDataInt STORED as ORC LOCATION 'wasb://tripdata@mydrivingstr.blob.core.windows.net/onesecagg' as 
SELECT TripId,
UserId,
vin,
RecordedTimeStamp,
Unix_timestamp(RecordedTimeStamp,'MM/dd/yyyy hh:mm:ss') as unixtimestamp,
AVG(Lat) as cLat,
AVG(Lon) as cLon,
AVG(Speed) as AverageSpeed,
MAX(EngineLoad) as EngineLoad,
MAX(EngineRPM) as EngineRPM,
MAX(DistanceWithMIL) as DistanceWithMIL,
MAX(Runtime) as Runtime
FROM tripdata 
WHERE year=${hiveconf:Year} and month=${hiveconf:Month} and day=${hiveconf:Day}
and TripId is not null and TripId!=''
and UserId is not null and UserId!=''
GROUP BY TripId, UserId, vin, RecordedTimeStamp, Unix_timestamp(RecordedTimeStamp,'MM/dd/yyyy hh:mm:ss');

-- year=${hiveconf:year} and month=${hiveconf:month} and day=${hiveconf:day}
