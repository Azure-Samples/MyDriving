DROP TABLE IF EXISTS tripdata;

CREATE EXTERNAL TABLE tripdata
(
    TripId string,
    Name string,
    TripPointId string,
    Lat double,
    Lon double,
    Speed double,
    RecordedTimeStamp timestamp,
    Sequence int,
    EngineRPM double double,
    ShortTermFuelBank double,
    LongTermFuelBank double,
    ThrottlePosition double,
    RelativeThrottlePosition double,
    TRuntime double,
    DistancewithMIL double,
    EngineLoad double,
    MAFFlowRate double,
    OutsideTemperature double,
    EngineFuelRate double,
    vin string
) PARTITIONED BY (year int, month int, day int) CLUSTERED BY (TripId) sorted by (RecordedTimeStamp) into 24 buckets 
ROW FORMAT DELIMITED FIELDS TERMINATED BY '|' LINES TERMINATED BY '\n'
STORED AS TEXTFILE 
LOCATION 'wasb://rawdata@mydrivingstr.blob.core.windows.net/tripdata'
tblproperties ("skip.header.line.count"="1");