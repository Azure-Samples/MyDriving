set hive.execution.engine=tez;


DROP TABLE IF EXISTS tripDataWIP;

CREATE TABLE tripDataWIP STORED AS ORC as 
SELECT TripId,
UserId,
vin,
AVG(cLat) as cLat,
AVG(cLon) as cLon,
AVG(AverageSpeed) as AverageSpeed,
CASE
 WHEN DistanceWithMIL>0 THEN true
 ELSE false
END as DroveWithMIL,
MAX(Runtime) as TripRuntime
FROM tripDataInt
GROUP BY TripId, UserId, vin, CASE
 WHEN DistanceWithMIL>0 THEN true
 ELSE false
END;

-- and Lat is not null and Lon is not null


-- Hard Brakes & Hard Accels
-- Assuming we're getting exactly 1 event per second per tripid, we are defining Hard Accelerations and hard brakes over a 5-second period. Ideally, you'd have a more complex definition and would likely need to create UDF to solve the problem.

DROP VIEW IF EXISTS speedCalc;

CREATE VIEW speedCalc as 
SELECT 
a.TripId,
a.RecordedTimeStamp,
a.unixtimestamp,
a.AverageSpeed as lastSpeed,
CASE 
 WHEN LAG(a.unixtimestamp,1,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) >= a.unixtimestamp-5 THEN a.AverageSpeed - LAG(a.AverageSpeed,1,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC)
 ELSE 0
END diffSpeed,
CASE 
 WHEN LAG(a.unixtimestamp,5,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) = a.unixtimestamp-5 THEN LAG(a.AverageSpeed,5,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) 
 WHEN LAG(a.unixtimestamp,4,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) >= a.unixtimestamp-5 THEN LAG(a.AverageSpeed,4,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) 
 WHEN LAG(a.unixtimestamp,3,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) >= a.unixtimestamp-5 THEN LAG(a.AverageSpeed,3,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) 
 WHEN LAG(a.unixtimestamp,2,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) >= a.unixtimestamp-5 THEN LAG(a.AverageSpeed,2,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) 
 WHEN LAG(a.unixtimestamp,1,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) >= a.unixtimestamp-5 THEN LAG(a.AverageSpeed,1,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC)
ELSE AverageSpeed  
END as firstSpeed,
CASE 
 WHEN LAG(a.unixtimestamp,5,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) = a.unixtimestamp-5 THEN MIN(a.AverageSpeed) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC ROWS BETWEEN 5 PRECEDING AND CURRENT ROW) 
 WHEN LAG(a.unixtimestamp,4,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) >= a.unixtimestamp-5 THEN MIN(a.AverageSpeed) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC ROWS BETWEEN 4 PRECEDING AND CURRENT ROW)
 WHEN LAG(a.unixtimestamp,3,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) >= a.unixtimestamp-5 THEN MIN(a.AverageSpeed) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC ROWS BETWEEN 3 PRECEDING AND CURRENT ROW) 
 WHEN LAG(a.unixtimestamp,2,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) >= a.unixtimestamp-5 THEN MIN(a.AverageSpeed) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC ROWS BETWEEN 2 PRECEDING AND CURRENT ROW) 
 WHEN LAG(a.unixtimestamp,1,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) >= a.unixtimestamp-5 THEN MIN(a.AverageSpeed) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC ROWS BETWEEN 1 PRECEDING AND CURRENT ROW)
ELSE AverageSpeed 
END as minSpeed,
CASE
 WHEN LAG(a.unixtimestamp,5,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) = a.unixtimestamp-5 THEN MAX(a.AverageSpeed) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC ROWS BETWEEN 5 PRECEDING AND CURRENT ROW) 
 WHEN LAG(a.unixtimestamp,4,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) >= a.unixtimestamp-5 THEN MAX(a.AverageSpeed) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC ROWS BETWEEN 4 PRECEDING AND CURRENT ROW)
 WHEN LAG(a.unixtimestamp,3,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) >= a.unixtimestamp-5 THEN MAX(a.AverageSpeed) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC ROWS BETWEEN 3 PRECEDING AND CURRENT ROW) 
 WHEN LAG(a.unixtimestamp,2,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) >= a.unixtimestamp-5 THEN MAX(a.AverageSpeed) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC ROWS BETWEEN 2 PRECEDING AND CURRENT ROW) 
 WHEN LAG(a.unixtimestamp,1,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) >= a.unixtimestamp-5 THEN MAX(a.AverageSpeed) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC ROWS BETWEEN 1 PRECEDING AND CURRENT ROW)
ELSE AverageSpeed 
END as maxSpeed,
CASE 
 WHEN LAG(a.unixtimestamp,5,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) = a.unixtimestamp-5 THEN AVG(a.AverageSpeed) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC ROWS BETWEEN 5 PRECEDING AND CURRENT ROW) 
 WHEN LAG(a.unixtimestamp,4,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) >= a.unixtimestamp-5 THEN AVG(a.AverageSpeed) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC ROWS BETWEEN 4 PRECEDING AND CURRENT ROW)
 WHEN LAG(a.unixtimestamp,3,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) >= a.unixtimestamp-5 THEN AVG(a.AverageSpeed) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC ROWS BETWEEN 3 PRECEDING AND CURRENT ROW) 
 WHEN LAG(a.unixtimestamp,2,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) >= a.unixtimestamp-5 THEN AVG(a.AverageSpeed) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC ROWS BETWEEN 2 PRECEDING AND CURRENT ROW) 
 WHEN LAG(a.unixtimestamp,1,0) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC) >= a.unixtimestamp-5 THEN AVG(a.AverageSpeed) OVER (Partition BY a.TripId ORDER BY a.unixtimestamp ASC ROWS BETWEEN 1 PRECEDING AND CURRENT ROW)
ELSE AverageSpeed 
END as avgSpeed
FROM TripdataInt a;


DROP VIEW IF EXISTS harddrivingInt;
CREATE VIEW harddrivingInt as 
SELECT TripId,
RecordedTimeStamp,
unixtimestamp,
lastSpeed,
firstSpeed,
minSpeed,
maxSpeed,
avgSpeed,
IF(firstSpeed - lastSpeed > 40 OR diffSpeed<=-15,1,0) as hard_brake,
IF(lastSpeed - firstSpeed > 50 OR maxSpeed - minSpeed > 50 OR lastSpeed >= 130 OR diffSpeed >= 15, 1,0) as hard_accel
FROM speedCalc;

-- maxSpeed - avgSpeed >= avgSpeed - minSpeed + GREATEST(0.40*(maxSpeed - minSpeed),3.5)

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
AverageSpeed double,
Hard_Accel int,
Hard_Brakes int,
CLat double,
CLon double,
DroveWithMIL boolean,
LengthOfTrip int,
vin string
) ROW FORMAT DELIMITED FIELDS TERMINATED BY '|' LINES TERMINATED BY '\n'
STORED AS TEXTFILE 
LOCATION 'wasb://tripdata@mydrivingstr.blob.core.windows.net/factTripDataoutput';

INSERT INTO TABLE tripdatafinal
SELECT a.TripId,
a.UserId,
a.cLat,
a.cLon,
a.AverageSpeed,
b.Hard_Brake,
b.Hard_Accel,
a.DroveWithMIL,
a.TripRuntime,
a.vin
FROM tripDataWIP a JOIN harddriving b
ON a.TripId = b.TripId;
