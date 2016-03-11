# Primer: Azure App Service for mobile backends

The best mobile apps—those that best engage consumers and thus perform well for their publishers—are consistently those that have great backends to power the experience. Many apps, for example, clearly rely on data retrieved from a backend, which could be collecting and processing information from a variety of other sources. In fact, it’s often helpful to think of such apps (especially in branded and enterprise scenarios) as customizing a mobile device to be a viewport onto the backend. Even games often use backend services to roam scores and game state across devices, manage leaderboards, and alert players to new challenges.

A backend in the cloud, accessed through HTTP requests, is also by nature shared across all potential clients and irrespective of client platforms like iOS, Android, and Windows. When combined with cross-platform app technologies like Xamarin and Apache Cordova, a large portion of your total application—backend plus clients—is platform-independent. What’s more, you can update your backend code any time without going through specific app store submissions.

Across the whole gamut of cloud-connected mobile apps, there has emerged a core set of backend capabilities that many apps require:

- **Push notifications** let the backend send messages to any number of devices, independent of specific mobile platform. Push notifications can be broadcast generally or targeted to an individual user’s devices or even a single device.
- **Cloud storage** easily shares data between the mobile app and the backend, and handles local offline caching and synchronization to accommodate the varied connectivity on mobile devices. Data can be organized on per-user, per-app, and global bases, and the backend can attach business logic to operations like insert, update, and delete. This makes it possible, for example, to detect changes made by one user or client that then generates push notifications to a group of others.
- **Authentication** provides (ideally) enterprise-grade user authentication through mechanisms like Azure Active Directory and OAuth providers like Microsoft, Google, Facebook, and Twitter. A unique identity for every user allows the backend to send personal push notifications and to partition user data so that it’s available in the same app on multiple devices.




Building on this, many mobile apps also need a couple of other capabilities:

- **Background jobs** do continuous and/or scheduled processing in the always-on and always-connected backend, commonly used to retrieve, process, and cache data from other services so it’s immediately available to the mobile app. By centralizing such processing in the backend, you minimize power and data usage on the mobile devices and avoid having to deal with sporadic mobile connectivity. Collecting data in the backend can also minimize the number of requests to other services that might impose their own usage limits.
 
- **Custom REST APIs** define endpoints that can be called by an app to invoke operations that aren’t strictly tied to data access, such as registering a user or device with a service, triggering business logic, sending notifications to another user, or running complex queries that aren’t easily done on the client.



Going still further, you often want both mobile and web experiences connected to the same backend—especially for shared data—and for enterprise apps especially you might need to connect to on-premises resources and tie into other business processes.

The question, then, is how to build up a backend with such capabilities that will also easily scale up and down with customer demand. It’s certainly possible, of course, to build all these from scratch inside virtual machines, but like many developers you’re probably much more interested in delivering great user experiences to your mobile customers than you are in building and maintaining custom infrastructure.


**Mobile Apps Azure App Service**

Recognizing this, Microsoft has packaged these features together as the Platform-as-a-Service (PaaS) offering called [Mobile Apps](https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-value-prop/), which you can [use for free](https://azure.microsoft.com/en-us/pricing/details/app-service/) until you’re ready to ramp up toward production and then scale out from there.

Mobile Apps one of four distinct “app types” that are all part of [Azure App Service](https://azure.microsoft.com/en-us/documentation/articles/app-service-value-prop-what-is/): Web Apps, Mobile Apps, API Apps, and Logic Apps. All of these are built upon a common web-based runtime, and Web, Mobile, and API Apps all share the same UI in the Azure portal. This blurs the distinction between the types, but in generally it’s best to simply focus on the features you need instead of the types. For mobile scenarios, it’s best to create a “Mobile App” and then use one of the options under **Tools > Quick Start** to get starter templates for the client code.

Mobile Apps provides a variety of features that you can turn on individually by configuring them in the Azure portal, including those in the following table:

<style>
    table, th, td {
        border: 1px solid black;
        border-collapse: collapse;
    }
    th, td {
        padding: 5px;
    }
</style>
<table>
<tr>
	<td><strong>Feature</strong></td>
	<td><strong>Location in Azure UI</strong></td>
	<td><strong>Description</strong></td>
</tr>
<tr>
	<td><strong>Authentication</strong></td>
	<td>Features > Authentication / Authorization</td>
	<td>Provides authentication using Microsoft, Google, Facebook, Twitter, or Azure Active Directory identity providers, and authorization to control access to backend features.</td>
</tr>
<tr>
	<td><strong>Storage</strong></td>
	<td>Mobile > Easy tables</td>
	<td>Using Azure SQL databases or table storage, the app works with the storage through in-memory objects, which the Azure libraries you include with the app take care of all the HTTP requests to the backend to keep the local and cloud copies in sync. For scenarios with variable connectivity, you can use storage with offline data sync as well.</td>
</tr>
<tr>
	<td><strong>Push notifications</strong></td>
	<td>Mobile > Push</td>
	<td>Uses Azure Notification Hubs to send notifications easily to iOS, Android, and Windows devices, which can scale from a single device to millions of devices</td>	
</tr>
<tr>
	<td><strong>REST APIs</strong></td>
	<td>Mobile > Easy APIs</td>
	<td>Creates the necessary Node.js infrastructure to expose and manage API endpoints, leaving you to focus on the core API implementation that you can do directly in the Azure portal.<br/>Alternately, you can use ASP.NET Web APIs with C# to create API endpoints. In this case you start with a template in Visual Studio and then publish the code to the App Service as described later in section 2.3.2.</td>
</tr>
<tr>
	<td><strong>Web Jobs</strong></td>
	<td>Web Jobs > Webjobs</td>
	<td>Upload code files that will run as background tasks in the App Service, or you can run them manually through the portal.</td>
</tr>
</table>



> **Note**: Azure App Service is the next-generation Azure product that combines the capabilities of older products such as Azure Websites and Azure Mobile Services, among others. You’ll see this on occasion in code. The client-side library for working with Mobile Apps, for example, is still called the Azure Mobile Services SDK, and the main class you use in that library is *MobileServicesClient*.



**Additional notes**

Generally speaking, when you commit a Mobile App in Azure to a technology like Node.js using Easy APIs or Easy Tables, then any Web App you might create in the same App Service must also need to be built with Node.js. Similarly, if you start with a Web App built on ASP.NET, you can also deploy endpoints built with ASP.NET Web APIs to the same App Service, but you can’t use Easy APIs and Easy Tables because Node.js won’t be there. (The Azure portal will say “Unsupported service” if you try to configure these features in App Service that’s already set up for ASP.NET.)

In short, avoid trying to mix-and-match technologies within the same App Service. Instead, simply create another Web App or Mobile App in a different App Service, which you then configure independently.

Don’t worry about incurring higher costs by doing this. The scaling unit for your pricing tier within Azure is called a plan, and you can create multiple App Services within the same plan. For details, see the [Azure App Service plans in-depth overview](https://azure.microsoft.com/en-us/documentation/articles/azure-web-sites-web-hosting-plans-in-depth-overview/).

Furthermore, it’s not a problem to share data between these different App Service. Azure SQL databases and Azure table storage are not bound to an App Service instance, which means that any number of App Service instances running in any number of plans can all share the same database, which is perhaps also accessed by other Azure services in your overall solution.

In the {Token:ProjectName} project, for example, a single Azure SQL database is shared across the App Service instance that implements the APIs, the Stream Analytics that processes data from the IoT Hub, and the Machine Learning component. As you extend this sample in your own projects, you can of course make use of this same central store for other needs.
