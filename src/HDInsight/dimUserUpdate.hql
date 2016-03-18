DROP TABLE IF EXISTS dimUserSQL;

CREATE TABLE IF EXISTS dimUserSQL
(
userid string,
vin string
) ROW FORMAT DELIMITED FIELDS TERMINATED BY '|' LINES TERMINATED BY '\n'
LOCATION 'wasb://rawdata@mydrivingstr.blob.core.windows.net/tripdata'
tblproperties ("skip.header.line.count"="1");


