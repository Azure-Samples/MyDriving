ALTER TABLE tripdata ADD IF NOT EXISTS PARTITION (year=${hiveconf:Year},month=${hiveconf:Month},day=${hiveconf:Day}) location 'wasb://rawdata@mydrivingstr.blob.core.windows.net/tripdata/${hiveconf:Year}/${hiveconf:Month}/${hiveconf:Day}'; 

ALTER TABLE tripDataMLFinal ADD IF NOT EXISTS PARTITION (year=${hiveconf:Year},month=${hiveconf:Month},day=${hiveconf:Day}) location 'wasb://tripdata@mydrivingstr.blob.core.windows.net/mlinput/${hiveconf:Year}/${hiveconf:Month}/${hiveconf:Day}'; 
