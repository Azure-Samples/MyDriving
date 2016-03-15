##Primer
 
[Add a statement about the general needs when working with large data sets. For example,] When working with large and/or complext amounts data or "big data"--such as of raw IoT data from many vehicles and drivers--it almost goes without saying that you'll want to apply various analytics before attempting to create meaningful visualizations of that data in both real-time monitoring and historical analysis scenarios.

[Perhaps here you can briefly list some of the main big data tools, like Hadoop and some of the others listed in the later table?]

[Then introduce what Hadoop is/does, something like] Clearly, handling big data requires specialized tools and parallel processing models that go well beyond what one might normally apply to with typical databases. One of the more popular of these is Apache Hadoop, an open-source framework that...<short overview of Hadoop, which should introduce the "clusters" term that you use later. Perhaps also introduce Pig and Hive>

[Now bridge to Azure HDInsight, I'm using most of what you wrote here:] Because of the scale of data and processing involved, working with Hadoop happens best in data centers. Azure HDInsight is Microsoft's software as a service (Saas) offering that provides Apache Hadoop, and software in the Hadoop ecosystem, as a managed service.

HDInsight reduces the complexity of working with Big Data analytics by providing clusters that are pre-configured for specific workloads, and that can be scaled dynamically to meet your workload requirements. For interoperability with other Azure services, and to allow the deletion of clusters when they are no longer needed, HDInsight stores data in either Azure Storage blobs or Azure Data Lake Store ([add one-line description of the latter, with a link]).

HDInsight currently provides clusters that are tuned for the following workloads:

| Workload | HDInsight cluster type |
| ----- | ----- |
| Batch processing (MapReduce) | [Hadoop](hdinsight-hadoop-introduction.md) |
| Batch processing (in memory) | [Spark](hdinsight-apache-spark-overview.md) |
| Real-time stream analytics | [Storm](hdinsight-storm-overview.md) |
| NoSQL data store | [HBase](hdinsight-hbase-overview.md) |

Each cluster type provides utilities and services required to the workload the cluster is tuned for. Core Hadoop technologies for moving, transforming and cleaning data, such as Pig and Hive, are available on all cluster types.


## HDInsight in myDriving

In the myDriving scenario, a Hadoop on HDInsight cluster is used to run batch processing that transform raw, unstructured data stored in Azure Storage blobs into structured data that can then be exported to SQL Server, from which it is brought into PowerBI for visualization.

The HDInsight cluster is created on-demand by an Azure Data Factory workflow [what is an ADF workflow? should there just be a link here?], which also automates the Hive and Pig jobs ran by HDInsight to process the data. the Azure Data Factory Copy activity then modes the data to the Azure SQL database.
 
Once processing is complete, Data Factory deletes the HDInsight cluster as it is no longer needed and moves the processed data into SQL Azure.

[Seems like this is an appropriate place to show a query or something else that configures HDInsight. I'm trying to have some code-looking stuff in most sections. :)]

