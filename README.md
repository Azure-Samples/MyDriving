---
services: applications-insights, app-service, data-factory, hdinsight, hockeyapp, iot-hub, key-vault, machine-learning, power-bi, sql-database, storage, stream-analytics, visual-studio-team-services 
platforms: dotnet, ios, android, xamarin
author: harikm86
---

# MyDriving - An Azure IOT and Mobile Sample Application

This repository contains the MyDriving sample that demonstrates the design and implementation of a comprehensive Internet of Things (IoT) solution that gathers telemetry from devices, processes that data in the cloud, and applies machine learning to provide an adaptive response. The demonstration logs data about your car trips using both your mobile phone and an On-Board Diagnostics (OBD) adaptor that collects information from your vehicle's control system. The Azure backend uses this data to provide feedback on your driving style in comparison to other users.

## Quick Start and Reference Guide

In addition to the readme documentation included in this repository, please see:
- [MyDriving home page](http://aka.ms/iotsampleapp)
- [Try out the MyDriving solution as a user](http://aka.ms/mydriving-use)
- [Build and deploy your own MyDriving solution](http://aka.ms/mydriving-start)
- [MyDriving Reference Guide: Building Integrated IoT Systems that Collect, Process, and Visualize Data](http://aka.ms/mydriving-keynote)

## Repository contents

### [scripts](./scripts)

A collection of resources to enable you to deploy and configure the Azure backend for MyDriving to your own Azure subscription. This includes Azure Resource Manager (ARM) templates for deploying all the necessary Azure services, Bash scripts, and Powershell scripts. 

It also includes scripts you can import into Visual Studio Team Services to set up build definitions for the Xamarin mobile app projects and the API endpoint project that's deployed to Azure App Service.

Refer to the [scripts readme](https://github.com/Azure-Samples/MyDriving/blob/master/scripts/README.md) for additional details.

### [src](./src)

**MyDriving.sln** makes it easy to view all the source in the repository in one place, including various queries and scripts that are used in the ARM templates to configure services. **NOTE**: this solution is just for reference and will not build all the projects in one go. Open the individual solutions in their respective folders to build and deploy.

More information on deploying to your own environment is given in the [Getting Started guide](http://aka.ms/mydriving-start).

#### [src/DataFactory](./src/DataFactory)

Data structure definitions used by the Data Factory service. The MyDriving system uses the Data Factory service to create an HDInsight cluster on demand. For details, refer to Chapter 8 of the [MyDriving Reference Guide](http://aka.ms/mydriving-keynote).

#### [src/Extensions](./src/Extensions)

An example extension to the MyDriving system that performs a the Vehicle Identification Number (VIN) lookup in the course of processing IoT data. For details, refer to Chapter 9 of the [MyDriving Reference Guide](http://aka.ms/mydriving-keynote).

#### [src/HDInsight](./src/HDInsight)

Copies of the resources the ARM templates use to configure the HDInsight cluster.

#### [src/MobileApps](./src/MobileApps)

Visual Studio 2015 solution and project files for the Android, iOS, and UWP mobile apps for MyDriving. These are implemented with Xamarin. For details, refer to Chapter 3 of the [MyDriving Reference Guide](http://aka.ms/mydriving-keynote).

#### [src/MobileAppService](./src/MobileAppService)

Visual Studio 2015 solution and project files for the API endpoints in Azure App Service. For details, refer to Chapter 4 of the [MyDriving Reference Guide](http://aka.ms/mydriving-keynote).

#### [src/OBDLibrary](./src/OBDLibrary)

Visual Studio 2015 projects containing the OBD client libraries for communicating with OBD dongles.

#### [src/PowerBI](./src/PowerBI)

PowerBI report definition. For details on how Power BI is used in MyDriving, refer to Chapters 6 and 8 of the [MyDriving Reference Guide](http://aka.ms/mydriving-keynote).

#### [src/SQLDatabase](./src/SQLDatabase)

Copies of the SQL scripts executed by the PowerShell script to populate the SQL databases created by the ARM templates.

#### [src/StreamAnalytics](./src/StreamAnalytics)

Copies of queries used by the ARM template to configure Stream Anaytics jobs.

