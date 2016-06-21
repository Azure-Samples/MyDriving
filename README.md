---
services: iot-hub, applications-insights, app-service, data-factory, hdinsight, hockeyapp, key-vault, machine-learning, power-bi, sql-database, storage, stream-analytics, visual-studio-team-services 
platforms: dotnet, ios, android, xamarin
author: harikm86
---

# MyDriving - An Azure IOT and Mobile Sample Application

This repository contains the MyDriving sample that demonstrates the design and implementation of a comprehensive Internet of Things (IoT) solution that gathers telemetry from devices, processes that data in the cloud, and applies machine learning to provide an adaptive response. The demonstration logs data about your car trips using both your mobile phone and an On-Board Diagnostics (OBD) adaptor that collects information from your vehicle's control system. The Azure backend uses this data to provide feedback on your driving style in comparison to other users.

## Quick Start and Reference Guide

In addition to the readme documentation included in this repository, please see:

- [MyDriving home page](http://azure.com/mydriving)
- [Try out the MyDriving solution as a user](https://azure.microsoft.com/documentation/articles/iot-solution-get-started/)
- [Build and deploy your own MyDriving solution](https://azure.microsoft.com/documentation/articles/iot-solution-build-system/)
- [MyDriving Reference Guide: Building Integrated IoT Systems that Collect, Process, and Visualize Data](http://download.microsoft.com/download/F/E/4/FE484B73-061C-4171-B95B-D16A500BDAC6/MyDriving%20Reference%20Guide.pdf)

## Repository contents

### [scripts](https://github.com/Azure-Samples/MyDriving/tree/master/scripts)

A collection of resources to enable you to deploy and configure the Azure backend for MyDriving to your own Azure subscription. This includes Azure Resource Manager (ARM) templates for deploying all the necessary Azure services, Bash scripts, and PowerShell scripts. 

It also includes scripts you can import into Visual Studio Team Services to set up build definitions for the Xamarin mobile app projects and the API endpoint project that's deployed to Azure App Service.

Refer to the [scripts readme](https://github.com/Azure-Samples/MyDriving/blob/master/scripts/README.md) for additional details.

### [src](https://github.com/Azure-Samples/MyDriving/tree/master/src)

Open the individual solutions in their respective folders to build and deploy.

More information on deploying to your own environment is given in the [Getting Started guide](https://azure.microsoft.com/documentation/articles/iot-solution-get-started/).

#### [src/DataFactory](https://github.com/Azure-Samples/MyDriving/tree/master/src/DataFactory)

Data structure definitions used by the Data Factory service. The MyDriving system uses the Data Factory service to create an HDInsight cluster on demand. For details, refer to Chapter 8 of the [MyDriving Reference Guide](http://download.microsoft.com/download/F/E/4/FE484B73-061C-4171-B95B-D16A500BDAC6/MyDriving%20Reference%20Guide.pdf).

#### [src/Extensions](https://github.com/Azure-Samples/MyDriving/tree/master/src/Extensions)

An example extension to the MyDriving system that performs a the Vehicle Identification Number (VIN) lookup in the course of processing IoT data. For details, refer to Chapter 9 of the [MyDriving Reference Guide](http://download.microsoft.com/download/F/E/4/FE484B73-061C-4171-B95B-D16A500BDAC6/MyDriving%20Reference%20Guide.pdf).

#### [src/HDInsight](https://github.com/Azure-Samples/MyDriving/tree/master/src/HDInsight)

Copies of the resources the ARM templates use to configure the HDInsight cluster.

#### [src/MobileApps](https://github.com/Azure-Samples/MyDriving/tree/master/src/MobileApps)

Visual Studio 2015 solution and project files for the Android, iOS, and UWP mobile apps for MyDriving. These are implemented with Xamarin. For details, refer to Chapter 3 of the [MyDriving Reference Guide](http://download.microsoft.com/download/F/E/4/FE484B73-061C-4171-B95B-D16A500BDAC6/MyDriving%20Reference%20Guide.pdf).

#### [src/MobileAppService](https://github.com/Azure-Samples/MyDriving/tree/master/src/MobileAppService)

Visual Studio 2015 solution and project files for the API endpoints in Azure App Service. For details, refer to Chapter 4 of the [MyDriving Reference Guide](http://download.microsoft.com/download/F/E/4/FE484B73-061C-4171-B95B-D16A500BDAC6/MyDriving%20Reference%20Guide.pdf).

#### [src/OBDLibrary](https://github.com/Azure-Samples/MyDriving/tree/master/src/OBDLibrary)

Visual Studio 2015 projects containing the OBD client libraries for communicating with OBD dongles.

#### [src/PowerBI](https://github.com/Azure-Samples/MyDriving/tree/master/src/PowerBI)

PowerBI report definition. For details on how Power BI is used in MyDriving, refer to Chapters 6 and 8 of the [MyDriving Reference Guide](http://download.microsoft.com/download/F/E/4/FE484B73-061C-4171-B95B-D16A500BDAC6/MyDriving%20Reference%20Guide.pdf).

#### [src/SQLDatabase](https://github.com/Azure-Samples/MyDriving/tree/master/src/SQLDatabase)

Copies of the SQL scripts executed by the PowerShell script to populate the SQL databases created by the ARM templates.

#### [src/StreamAnalytics](https://github.com/Azure-Samples/MyDriving/tree/master/src/StreamAnalytics)

Copies of queries used by the ARM template to configure Stream Analytics jobs.

--
This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.


