---
services: applications-insights, app-service, data-factory, hdinsight, hockeyapp, iot-hub, machine-learning, power-bi, sql-database, storage, stream-analytics, visual-studio-team-services 
platforms: dotnet, ios, android, xamarin
author: harikm86
---


# MyDriving - An IOT Sample Application

This repository contains the MyDriving starter kit that demonstrates the design and implementation of a typical Internet of Things (IoT) solution that gathers telemetry from devices, processes that data in the cloud, and applies machine learning to provide an adaptive response. The demonstration logs data about your car trips, using data both from your mobile phone and an OBD adaptor that collects information from your car’s control system. It uses this data to provide feedback on your driving style in comparison to other users. For more information check out the [MyDriving Page](http://aka.ms/iotsampleapp)

## Quick Start and Reference Guide

In addition to the readme documentation included in this repository, please see:

- [Try out the MyDriving Solution](http://aka.ms/mydriving-use)
- [Build your own MyDriving Solution](http://aka.ms/mydriving-start)
- [MyDriving Reference Guide: Building Integrated IoT Systems that Collect, Process, and Visualize Data](http://aka.ms/mydriving-keynote)

## Repository contents

### [scripts](./scripts)

A collection of resources to enable you to deploy the starter kit Azure services to your own Azure subscription.

#### [scripts/ARM](./scripts/ARM)

ARM templates to configure the Azure services, such as azure IoT Hub, Azure Stream Analytics, and Azure Machine Learning used by the MyDriving sample.

#### [scripts/Assets](./scripts/Assets)

Additional resources referenced by the ARM templates to configure the HDInsight cluster used by the MyDriving sample.

#### [scripts/PowerShell](./scripts/PowerShell)

PowerShell script to execute the ARM templates and perform additional setup operations such as initializing the SQL database.

#### [scripts/SQLDatabase](./scripts/SQLDatabase)

SQL scripts executed by the PowerShell script to initialize the SQL database created by the ARM templates.

### [src](./src)

The **MyDriving.sln** Visual Studio 2015 solution enables you to build and deploy the MyDriving starter kit to your environment. The solution contains multiple projects such as those related to phone app, the Azure App Service Mobile Apps service, and the VIN Lookup service.

#### [src/Components](./src/Components)

The [calabash](.src\Components\calabash-16.2\component\DEtails.md) and [btprogresshub](.\src\Components\btprogresshud-1.20\component\Details.md) components used in the iOS MyDriving app.

#### [src/DataFactory](./src/DataFactory)

Data structure definitions used by the Data Factory service. The starter kit uses the Data Factory service to create an HDInsight cluster on demand.

#### [src/Extensions](./src/Extensions)

Extensions to the MyDriving starter kit. This includes the Vehicle Identification Number lookup service example in the MyDriving system.

#### [src/HDInsight](./src/HDInsight)

Copies of the resources the ARM templates use to configure the HDInsight cluster. The **MyDriving.sln** Visual Studio 2015 solution includes these for reference.

#### [src/IOTHubClientSDK](./src/IOTHubClientSDK)

Customized versions of the Microsoft Azure IoT device SDK for .NET used in the MyDriving starter kit.

#### [src/MobileApps](./src/MobileApps)

Visual Studio 2015 solutions and projects for the Android, iOS, and UWP mobile apps in the MyDriving starter kit.

#### [src/MobileAppService](./src/MobileAppService)

Visual Studio 2015 project and solution for the MyDriving Azure App Service Mobile Apps service. This project defines the endpoints the mobile apps use to access the MyDriving backend services.

#### [src/OBDLibrary](./src/OBDLibrary)

Visual Studio 2015 projects containing the OBD client libraries that enable the mobile apps to interface with an OBD dongle.

#### [src/PowerBI](./src/PowerBI)

PowerBI report definition.

#### [src/SQLDatabase](./src/SQLDatabase)

Copies of the SQL scripts executed by the PowerShell script to populate the SQL database created by the ARM templates. The **MyDriving.sln** Visual Studio 2015 solution includes these for reference.

#### [src/StreamAnalytics](./src/StreamAnalytics)

Copies of Stream Analytics queries created by the ARM template. The **MyDriving.sln** Visual Studio 2015 solution includes these for reference.


