set hive.execution.engine=tez;

-- Averaging the data to 1 event per sec per trip
DROP TABLE IF EXISTS tripDataInt;
CREATE TABLE tripDataInt as 
SELECT TripId,
Unix_timestamp(RecordedTimeStamp,'MM/dd/yyyy hh:mm:ss') as unixtimestamp,
AVG(Lat) as cLat,
AVG(Lon) as cLon,
AVG(Speed) as AverageSpeed,
MAX(DistanceWithMIL) as DistanceWithMIL,
MAX(Runtime) as Runtime
FROM tripdata 
WHERE year=2016 and month=03 and day=12
and TripId is not null and TripId!=''
GROUP BY TripId, Unix_timestamp(RecordedTimeStamp,'MM/dd/yyyy hh:mm:ss');



DROP TABLE IF EXISTS tripDataWIP;

CREATE TABLE tripDataWIP as 
SELECT TripId,
AVG(Lat) as cLat,
AVG(Lon) as cLon,
AVG(Speed) as AverageSpeed,
CASE
 WHEN DistanceWithMIL>0 THEN true
 ELSE false
END as DroveWithMIL,
MAX(Runtime) as TripRuntime
FROM tripDataInt
WHERE year=2016 and month=03 and day=12
GROUP BY TripId, CASE
 WHEN DistanceWithMIL>0 THEN true
 ELSE false
END;

-- year=${hiveconf:year} and month=${hiveconf:month} and day=${hiveconf:day}

-- Hard Brakes & Hard Accels
DROP TABLE IF EXISTS harshdriving;

CREATE TABLE harshdriving as
SELECT
a.TripId,
a.unixtimestamp,
a.Speed,
MIN(b.unixtimestamp)
FROM TripdataInt a JOIN TripdataInt b
ON a.TripId = b.TripId AND a.unixtimestamp - 5 = b.unixtimestamp;


-- and Lat is not null and Lon is not null


-- Assuming we're getting exactly 1 event per second per tripid
SELECT 
TripId,
RecordedTimeStamp,
unix_timestamp,
Speed,
MAX(Speed) OVER (PARTITION BY TripId OVER BY unix_timestamp ASC ROWS BETWEEN Unbounded )
MIN(Speed) as min_speed
FROM harshdriving


-- Start Here
SELECT TripId, 
recordedtimestamp, 
Speed, 
PrevSpeed, 
diffSpeed,
diffSpeed + SUM(diffSpeed) OVER (PARTITION by TripId ORDER BY RecordedTimeStamp ASC) as diffDiffSpeed
FROM harshDriving a
LIMIT 50;



CASE
 WHEN diffSpeed < 0 AND (LAG(a.diffSpeed, 1, 0) OVER (Partition BY a.TripId ORDER BY RecordedTimeStamp ASC)) >= 0 then diffSpeed
 WHEN diffSpeed > 0 AND (LAG(a.diffSpeed, 1, 0) OVER (Partition BY a.TripId ORDER BY RecordedTimeStamp ASC)) <= 0 then diffSpeed
 ELSE SUM(diffSpeed) OVER (Partition BY a.TripId ORDER BY RecordedTimeStamp ASC ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
END as tempSpeed