ALTER TABLE tripdata ADD IF NOT EXISTS PARTITION (year=${hiveconf:Year},month=${hiveconf:Month},day=${hiveconf:Day}) location 'wasb://rawdata@${hiveconf:DataStorageAccount}.blob.core.windows.net/tripdata/${hiveconf:Year}/${hiveconf:Month}/${hiveconf:Day}'; 

ALTER TABLE tripDataMLFinal ADD IF NOT EXISTS PARTITION (year=${hiveconf:Year},month=${hiveconf:Month},day=${hiveconf:Day}) location 'wasb://tripdata@${hiveconf:DataStorageAccount}.blob.core.windows.net/tables/mlinput/${hiveconf:Year}/${hiveconf:Month}/${hiveconf:Day}'; 
