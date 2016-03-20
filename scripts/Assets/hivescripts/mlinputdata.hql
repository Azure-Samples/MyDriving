set hive.execution.engine=tez;

DROP VIEW IF EXISTS metricCalc;
CREATE VIEW metricCalc as 
SELECT TripId,
UserId,
unixtimestamp,
AverageSpeed as lastSpeed,
LAG(AverageSpeed,5,0) OVER (Partition BY TripId, UserId ORDER BY unixtimestamp ASC) as firstSpeed,
EngineRPM,
LAG(EngineRPM,5,0) OVER (Partition BY TripId, UserId ORDER BY unixtimestamp ASC) as firstRPM,
EngineLoad,
EngineRPM - LAG(EngineRPM,5,0) OVER (Partition BY TripId, UserId ORDER BY unixtimestamp ASC) as ERPMdiff,
AverageSpeed - LAG(AverageSpeed,5,0) OVER (Partition BY TripId, UserId ORDER BY unixtimestamp ASC) as Speeddiff,
IF(EngineLoad>=80,1,0) as EngineLoad80,
IF(EngineLoad<=20,1,0) as EngineLoad20
FROM tripDataInt;



-- Ensuring we have 1 file
set hive.exec.reducers.max = 1;
set mapred.reduce.tasks = 1;


INSERT INTO table tripDataMLFinal Partition (year=2016, month=03, day=12)
SELECT TripId, 
UserId, 
FROM_UNIXTIME(MIN(unixtimestamp)) as starttime,
stddev_pop(ERPMdiff) as ERPMdiffsd,
stddev_pop(Speeddiff) as Speeddiffsd,
CAST(SUM(EngineLoad80) as DOUBLE)/CAST(SUM(EngineLoad20) as DOUBLE) as ELoutlier
FROM metricCalc
GROUP BY TripId, 
UserId;
