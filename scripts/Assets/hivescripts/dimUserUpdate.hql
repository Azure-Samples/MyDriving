DROP TABLE IF EXISTS dimUser;

CREATE TABLE IF EXISTS dimUser
(
userid string,
vin string
) ROW FORMAT DELIMITED FIELDS TERMINATED BY '|' LINES TERMINATED BY '\n'
LOCATION 'wasb://rawdata@mydrivingstr.blob.core.windows.net/tripdata'
tblproperties ("skip.header.line.count"="1");


INSERT INTO dimUser
SELECT distinct UserId, vin FROM tripdata WHERE year=2016 and month=03 and day=12
and TripId is not null and TripId!='' and UserId is not null and UserId != '';