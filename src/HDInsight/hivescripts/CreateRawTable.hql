set hive.execution.engine=tez;

DROP TABLE IF EXISTS tripdata;

--Figure out timestamp stuff
CREATE EXTERNAL TABLE tripdata
(
    TripId string,
    UserId string,
    Name string,
    TripPointId string,
    Lat double,
    Lon double,
    Speed double,
    RecordedTimeStamp string,
    Sequence int,
    EngineRPM double,
    ShortTermFuelBank double,
    LongTermFuelBank double,
    ThrottlePosition double,
    RelativeThrottlePosition double,
    Runtime double,
    DistancewithMIL double,
    EngineLoad double,
    MAFFlowRate double,
    OutsideTemperature double,
    EngineFuelRate double,
    vin string
) PARTITIONED BY (year int, month int, day int) CLUSTERED BY (TripId) sorted by (RecordedTimeStamp) into 24 buckets 
ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' LINES TERMINATED BY '\n'
STORED AS TEXTFILE 
LOCATION 'wasb://rawdata@${hiveconf:DataStorageAccount}.blob.core.windows.net/tripdata'
tblproperties ("skip.header.line.count"="1");


DROP TABLE IF EXISTS tripDataMLFinal;
CREATE EXTERNAL TABLE tripDataMLFinal
(
tripId string,
userId string,
triptime timestamp,
ERPMdiffsd double,
Speeddiffsd double,
ELoutlier double
) PARTITIONED BY (year int, month int, day int) 
ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' LINES TERMINATED BY '\n'
STORED AS TEXTFILE 
LOCATION 'wasb://tripdata@${hiveconf:DataStorageAccount}.blob.core.windows.net/tables/mlinput'; 
