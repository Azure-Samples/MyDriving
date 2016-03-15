# Implementation notes and best practices

## Power BI accounts

Power BI provides two types of [accounts](https://powerbi.microsoft.com/en-us/pricing/): Free and Power BI Pro. It is strongly recommended the account specified in a Stream Analytics output job be a Power BI Pro account. Power BI Free has additional limits to the number of rows per hour that can be shown in stream visualizations. Data consumers, those users signed in to Power BI to view streaming data in reports and dashboards, also have constraints on the number of rows per hour that can be displayed.

Data consumers of historical, archived data through the cold path must be signed in to Power BI with a Power BI Pro account. This is because in our cold path scenario, archived data is stored in Azure SQL Database. Connections from the Power BI service to Azure SQL Database are live. Live connections are supported only for users with a Power BI Pro account.

**Note:** If the account you specify for the Stream Analytics job output has a password that expires, you will likely get an Authenticate user error in the Operations Logs. To resume streaming, you must stop the job and renew authorization.

## Throughput constraints

Power BI employs concurrency and throughput constraints, depending on user account type. You specify throughput (as a push event) in the query for the Stream Analytics output. For Power BI, it is recommended you use [TumblingWindow](https://msdn.microsoft.com/en-us/library/azure/dn835055.aspx) or [HoppingWindow](https://msdn.microsoft.com/en-us/library/azure/dn835041.aspx)  to ensure data push is at most one push per second. In this scenario, our query defines data push as TumblingWindow, 5 seconds. That is, a data push to the dataset occurs once every five seconds.  

**Decision point:** Power BI supports live streaming through Azure Stream Analytics job output or through the [Power BI REST API]((https://msdn.microsoft.com/en-us/library/dn877544.aspx). For this architecture, an Azure Stream Analytics output job is easily configured to authenticate a connection through a Power BI user account, create a dataset in Power BI, and output streaming data to the dataset specified in the query. The Power BI REST API is best for custom apps to push data directly from custom applications to a Power BI dataset.
