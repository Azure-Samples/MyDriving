DROP TABLE IF EXISTS dimUser;

CREATE TABLE dimUser
(
userid string,
vin string
) ROW FORMAT DELIMITED FIELDS TERMINATED BY '|' LINES TERMINATED BY '\n'
LOCATION 'wasb://tripdata@${hiveconf:DataStorageAccount}.blob.core.windows.net/tables/dimUser';

TRUNCATE TABLE dimUser;

INSERT INTO TABLE dimUser
SELECT distinct UserId, 
IF(vin is NULL,"Unknown",IF(length(vin)>20,substr(vin,0,19),vin)) as vin 
FROM tripdata WHERE year=${hiveconf:Year} and month=${hiveconf:Month} and day=${hiveconf:Day}
and TripId is not null and TripId!='' and UserId is not null and UserId != '';