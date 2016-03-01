# Principles in the reference architecture design

The overarching goal in the design of this reference architecture is to bring together IoT devices and mobile apps through the orchestration of a rich cloud backend running on Azure. We've also designed it to be applicable to a variety of real-world scenarios and not merely a specific demonstration. It's out intention that you can replace and customize different components within the architecture as you own scenarios demand. 

Our own demonstration really just shows an *instance* of this architecture, with a scenario centered on OBD data from automobiles that makes a journey through the whole backend system. That same journey can be taken by any other kind of data using the same backend components. You'll need only customize the analytics and machine learning components for that specific data, and customize the API endpoints that serve up the results to client apps. 

This reference architecture, in other words, is not specific to automotive data as used in the demonstration. It rather provides a common pattern that is applicable to many different IoT + cloud + mobile scenarios.  

The essence of this pattern has little do to with the IoT or mobile devices. It's rather how the backend takes large amounts of raw data from IoT devices and transforms it through analytics and machine learning to produce meaningful insights that human beings can see and consume through clients apps running on mobile phones, tablets, and PCs.

  
**Inexpensive and purposeful IoT devices**

In an architecture like this, the IoT devices involved should be simple and low-cost, allowing for data collection from many individual points. We designed the system to use use a mobile phone as a field gateway to collect data from a non-networked device (the Bluetooth-capable ODB dongle in our demonstration) and transmit it to the cloud backend.

For a demonstration that's focused on vehicle data, it's an appropriate assumption that most drivers would already have a mobile phone and wouldn't want to purchase a separate Internet-capable gateway device. All that's needed is an off-the-shelf ODB device. In other scenarios, though, a mobile phone wouldn't be appropriate or would be much more expensive than a custom IoT device built with a Raspberry Pi. Even with ODB data, there might be scenarios with a service fleet, for example, where you want to build dedicated IoT devices that can collect a richer set of data than we're doing in the demonstration.

What's important to understand, then, is that there's nothing particular about having a phone serve in the gateway role—it's merely acting in a narrow capacity to connect the ODB data source to the backend.
 
**Cross-platform mobile app to present a data experience**

On the other side of the picture, we wanted the mobile app to be available on all major platforms, leading to the choice of a cross-platform mobile technology like Xamarin. The primary role of the app is to *present* data from the backend through a beautiful and engaging user interface. It need not, therefore, be concerned with *processing* of that data. This is a general principle for the design of cloud-connected mobile apps. By their nature, mobile devices have limited power and spotty connectivity. The cloud, on the other hand, is always on and always connected.

Thus it's best to have the backend do the heavy-lifting of data collection, storage, analysis, and so on, and then make mobile-optimized data available through web API endpoints. In this way we can minimize the network traffic (that is, data usage) from the mobile device and minimize its power requirements while still delivering a great experience.

Indeed, although the apps oftentimes get all the credit for delivering a great experience, they're merely providing an engaging viewport for the sophisticated processing and orchestrations that are happening in the cloud. The mobile app that's used in our demonstration, in fact, isn't really part of the reference architecture because it's entirely oriented around the specific type of data. When you apply this reference architecture to different scenarios with different data, you'll be replacing the client experience entirely. 


**Azure-powered backend**

Within the backend—the heart and soul of this reference architecture—we sought to showcase how many Azure services combine into a meaningful whole without the need to write a lot of code or build such services from scratch. We've built the backend primarily through the configuration and interconnection of services like IoT Hub, Stream Analytics, SQL databases, SQL Data Warehouse, Machine Leaning, App Services, Azure Functions, and Event Hub, along with external services like HockeyApp and PowerBI. In fact, the only part of the backend that involves coding is the implementation of the API endpoints in the App Service component.

We also wanted to design the architecture to be extensible so that you could customize it for additional scenarios that are not included in the [Project Name TODO] demo. This happens primarily through Azure Functions and Event Hub as described in the Extensibility section of this documentation.

To help you get started, we also made certain choices to keep the cost of running the Azure backend relatively low. In your own deployments you might opt to invest in higher-performing services as your scenario requires.  


**Short ramp-up and rapid iterations**

One other characteristic of this architecture is that it was conceptualized, implemented, and polished over the course only 8-10 weeks. This proves that creating a system similar to this need not take 12-18 months as you might expect.

Indeed, with this reference architecture in hand we expect that you can deploy a customized instance in as little as 2-3 weeks, allowing you to quickly start collecting, analyzing, and visualizing the data that's available to you. This short ramp-up time means that you can get into an agile process almost immediately to continually improve, refine, and extend the system and deliver increasing value to your business.



  