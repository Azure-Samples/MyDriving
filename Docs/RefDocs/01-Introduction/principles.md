# Principles in the reference architecture design

The overarching goal in the design of this reference architecture is to bring together IoT devices and mobile apps through the orchestration of a rich cloud backend running on Azure. 

In our own demonstrations we're really just showing an instance of this architecture centered around ODB data from automobiles. But data from any other kinds of devices can make the same journey through this architecture and be customized by changing the configuration of the analytics and machine learning components, and customizing the API endpoints to serve up the appropriate data sets. This reference architecture, in other words, provide a common pattern that is applicable to many different IoT+cloud+mobile scenarios.

What's most important in the whole story, in fact, has little do to with the IoT or mobile devices. It's rather how the backend takes large amounts of raw data from IoT devices and transforms it through analytics and machine learning to produce meaningful insights that human beings can see on their mobile phones.  

Such is true of many great apps: the greatest app experiences are really not about what's happening in the app itself on the mobile device, it's that the app is turning that device into an engaging viewport onto the sophisticated processing and orchestrations that's happening in the cloud. Yes, consumers think of "the app" as the experience, but for developers, the heart and soul of the experience is built in the backend, even to the point where the mobile app can be rebuilt or swapped out at any time without sacrificing any part of that experience.

**Inexpensive and purposeful IoT devices**

In an architecture like this, the IoT devices involved should be simple and low-cost, allowing for data collection from many individual points. We designed the system to use use a mobile phone as a field gateway to collect data from a non-networked device (the Bluetooth-capable ODB dongle in our demonstration) and transmit it to the cloud backend.

For a demonstration that's focused on vehicle data, it's an appropriate assumption that most drivers would already have a mobile phone and wouldn't want to purchase a separate Internet-capable gateway device. All that's needed is an off-the-shelf ODB device. In other scenarios, though, a mobile phone wouldn't be appropriate or would be much more expensive than a custom IoT device built with a Raspberry Pi. Even with ODB data, there might be scenarios with a service fleet, for example, where you want to build dedicated IoT devices that can collect a richer set of data than we're doing in the demonstration.

What's important to understand, then, is that there's nothing particular about having a phone serve in the gateway role—it's merely acting in a narrow capacity to connect the ODB data source to the backend.
 
**Cross-platform mobile app to present a data experience**

On the other side of the picture, we wanted the mobile app to be available on all major platforms, leading to the choice of a cross-platform mobile technology like Xamarin. The primary role of the app is to *present* data from the backend through a beautiful and engaging user interface. It need not, therefore, be concerned with *processing* of that data. This is a general principle for the design of cloud-connected mobile apps. By their nature, mobile devices have limited power and spotty connectivity. The cloud, on the other hand, is always on and always connected.

Thus it's best to have the backend do the heavy-lifting of data collection, storage, analysis, and so on, and then make mobile-optimized data available through web API endpoints. In this way we can minimize the network traffic (that is, data usage) from the mobile device and minimize its power requirements while still delivering a great experience.

**Azure-powered backend**

Within the backend—the heart and soul of this reference architecture—we sought to showcase how to combine many Azure services into a meaningful whole without the need to write a lot of code or build such services from scratch. We've built the backend primarily through the configuration and interconnection of services like IoT Hub, Stream Analytics, SQL databases, SQL Data Warehouse, Machine Leaning, App Services, Azure Functions, and Event Hub, along with external services like HockeyApp and PowerBI. In fact, the only part of the backend that involves coding is the implementation of the API endpoints in the App Services component.

We also wanted to design the architecture to be extensible so that you could customize it for additional scenarios that are not included in the [Project Name TODO] demo. This happens primarily through Azure Functions and Event Hub as described in the Extensibility section of this documentation.

**Short ramp-up and rapid iterations**

One other characteristic of this architecture is that it was conceptualized, implemented, and polished over the course only 8-10 weeks. This proves that creating a system similar to this need not take 12-18 months as you might expect.

Indeed, with this reference architecture in hand we expect that you can deploy a customized instance in as little as 2-3 weeks, allowing you to quickly start collecting, analyzing, and visualizing the data that's available to you. This short ramp-up time means that you can get into an agile process almost immediately to continually improve, refine, and extend the system and deliver increasing value to your business.



  