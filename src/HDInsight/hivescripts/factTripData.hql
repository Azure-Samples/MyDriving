set hive.execution.engine=tez;


DROP VIEW IF EXISTS tripDataWIPView;

CREATE VIEW tripDataWIPView as 
SELECT TripId,
UserId,
vin,
MIN(unixtimestamp) as minUnixTimestamp,
AVG(cLat) as cLat,
AVG(cLon) as cLon,
AVG(AverageSpeed) as AverageSpeed,
MAX(Runtime) as TripRuntime,
MAX(DistanceWithMIL) as DistanceWithMIL
FROM tripDataInt
GROUP BY TripId, UserId, vin;


DROP TABLE IF EXISTS tripDataWIP;
CREATE TABLE tripDataWIP STORED AS ORC as 
SELECT TripId,
UserId,
vin,
FROM_UNIXTIME(minUnixTimestamp) as tripStartTime,
cLat,
cLon,
AverageSpeed,
CAST(TripRuntime/60 as INT) as TripRunTime,
CASE
 WHEN DistanceWithMIL>0 THEN true
 ELSE false
END as DroveWithMIL
FROM tripDataWIPView;



-- and Lat is not null and Lon is not null


-- Hard Brakes & Hard Accels
-- Assuming we're getting exactly 1 event per second per tripid, we are defining Hard Accelerations and hard brakes over a 5-second period. Ideally, you'd have a more complex definition and would likely need to create UDF to solve the problem.

DROP VIEW IF EXISTS speedCalc;

CREATE VIEW speedCalc as 
SELECT 
a.TripId,
a.unixtimestamp,
LAG(a.unixtimestamp,1,0) OVER (Partition BY a.TripId Order by a.unixtimestamp ASC) as prevTimestamp,
a.AverageSpeed as lastSpeed,
LAG(a.AverageSpeed,1,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) as firstSpeed
FROM TripdataInt a;


DROP VIEW IF EXISTS harddrivingInt;
CREATE VIEW harddrivingInt as 
SELECT TripId,
unixtimestamp,
IF ((lastSpeed - firstSpeed)*0.278/(unixtimestamp - prevTimestamp)>=6,1,0) as hard_accel,
IF ((lastSpeed - firstSpeed)*0.278/(unixtimestamp - prevTimestamp)<=-6,1,0) as hard_brake
FROM speedCalc;



DROP TABLE IF EXISTS harddriving;
CREATE TABLE harddriving STORED as ORC as 
SELECT a.TripId,
SUM(a.hard_brake) as Hard_Brake,
SUM(a.hard_accel) as Hard_Accel
FROM harddrivingint a
GROUP BY a.TripId;


DROP TABLE IF EXISTS tripdataFinal;
CREATE TABLE tripdatafinal
(
TripId string,
UserId string,
vin string,
tripStartTime string,
AverageSpeed double,
Hard_Accel int,
Hard_Brakes int,
DroveWithMIL boolean,
LengthOfTrip int,
cLat double,
cLon double
) ROW FORMAT DELIMITED FIELDS TERMINATED BY '|' LINES TERMINATED BY '\n'
STORED AS TEXTFILE 
LOCATION 'wasb://tripdata@${hiveconf:DataStorageAccount}.blob.core.windows.net/tables/factTripDataoutput';

TRUNCATE TABLE tripdatafinal;

INSERT INTO TABLE tripdatafinal
SELECT a.TripId,
a.UserId,
IF(a.vin is NULL OR a.vin=='', "Unknown", IF(length(a.vin)>20,substr(a.vin,0,19),a.vin)) as vin,
a.tripStartTime,
a.AverageSpeed,
b.Hard_Accel,
b.Hard_Brake,
a.DroveWithMIL,
a.TripRuntime,
a.cLat,
a.cLon
FROM tripDataWIP a JOIN harddriving b
ON a.TripId = b.TripId;
