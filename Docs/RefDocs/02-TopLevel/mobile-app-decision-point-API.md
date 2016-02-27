# Decision Point: Ways to Implement an API

As you probably know, a web API is simply a set of URLs through which you can make HTTP requests to retrieve or upload data. The beauty of this model is that the client, working entirely through HTTP requests, is not at all concerned with how the API is itself implemented. API developers have complete freedom of choice where server-side technologies are concerned, so long as something's in place to respond to HTTP requests. 

Within Azure you have several ways to build an API:

- Virtual machines
- Cloud services
- Web apps (part of App Service)
- Mobile apps/Easy APIs (part of App Service)

Which path you choose depends on the level of control you need over the environment and the programming tools you'd like to use. 

**Virtual Machines**
 
Virtual Machines are the basic infrastructure (IaaS) component in Azure. You start by choosing from a wide variety of pre-provisioned VM images (Windows, Linux, and Ubuntu), and once the VM is up and running you can login with Remote Desktop and install whatever other software you want. This gives you complete control over every aspect: the HTTP server, databases, and the language runtimes with which you implement the APIs. And because you control the machine, you can also rely on the persistence of the file system and use it freely. Azure also assigns a permanant IP address to your VM, to which you can forward DNS names. And of course you can use the VM for any other server needs you might have, which is especially useful if you need to migrate existing applications from on-premises servers.

The downside is that you are completely responsible to maintain and update the VM yourself. Scaling is also a manual process: when you need more capacity, you'll have to create and provision additional VMs and handle both load balancing and URL routing. Similarly, when capacity needs drop, you'll have to manually stop the VMs you no longer need (which deallocates their IP addresses) so that they don't continue to incur charges. Those VMs can be restarted later (with new IP addresses); Azure maintains your VM images even when they're stopped. 

**Cloud Services**
Azure Cloud Services is the basic platform-as-a-service (PaaS) component in Azure that is hosted within Azure-managed VMs. This means that Azure automatically patches and updates the operating system, automatically scales to meet demand (according to your chosen configuration), monitors service health, and provides a single DNS name that's automatically mapped across however many VMs get spun up. All this lets you focus on your applications rather than infrastructure. 

The tradeoff is that because the software that's installed on a Cloud Services VM is fixed (thus allowing Azure to create new VMs as necessary), those are the tools you have to work with to implement your API. To be specific, Cloud Services always run on Windows Server VMs with IIS to handle HTTP requests. To implement your APIs you choose from .NET, Java, Node.js, PHP, Python, and Ruby—a wide range of choices that satisfy most developers, but not all, especially if you rely on custom tools. Furthermore, because you don't have access to the file system, you must use other storage mechanisms like databases, blobs, or tables.

Another consideration is that any given instance of Cloud Services has its own dedicated VM that you can't reapportion for other purposes. This means that Cloud Services is best suited for API implementations that are rich enough to justify the cost of at least that one VM.
 

**App Services — Web Apps**
Azure App Services is another platform-as-a-service that was described earlier in the App Services Primer <TODO: link>. Like Cloud Services, Azure automatically provides for scaling and DNS name forwarding, and further insulates you from having to think much about the underlying VMs. To be more specific, you create an instance of App Services within an existing Azure Compute Plan that can be shared by multiple instances. This makes App Services highly suitable for low-demand APIs because you can share the cost of the Plan (which is based on the cost of a VM) with other instances. Of the choices outlined here, App Services is also the only one that gives you a free pricing tier that you can use until you reach production. 

With App Services, you work with the server through FTP like you do with a typical web host. This means you implement APIs exactly as you would with any other such host by uploading files and code through FTP, so long as the host supports the necessary language runtime. The App Services host presently supports .NET, PHP, Java, Node.js, and Python. Thus you can easily create, for example, an ASP.NET website in Visual Studio, including a rich API built on Entity Framework for data access, and deploy it from there directly to App Services.

If you're happy working with the supported languages, there really aren't any additional downsides with App Services. App Services, in fact, is what Microsoft generally recommends for new projects because it meets or exceeds the capabilities of Cloud Services.  
 

**App Services — Easy APIs**

There is one more way to implement an API that's even simpler: the Easy APIs feature within App Services.

Easy APIs is built entirely on Node.js (just like the Custom API feature of the previous-generation Azure Mobile Services). You can even write your code entirely within the context of the browser and the Azure portal—no external tools are necessary.

With Easy APIs, HTTP requests are always handled through a Node.js server using Express for routing. This implies that anything else you want to host at the same DNS name must be something you can handle through that same server. This could make it tricky, for example, to build a website at the same base URL with ASP.NET or Java. 

Endpoints are implemented in JavaScript with Node.js and Azure-provided methods. Storage options are limited to Azure SQL databases and Azure tables, but because these mechanisms exist separate from an App Service instance they are easily shared with any other services that can access them. This means you can create an ASP.NET web app in another App Service instance (on the same or another underlying Plan) and share data with your Easy APIs endpoints.

Taken all together, Easy APIs are best suited for mobile backend scenarios that are focused on the specific and private needs of one app or perhaps a set of related apps. Typical uses include creating endpoint to receive push notification channel IDs from apps, endpoints to roam configuration data across apps on different platforms (or this can be done transparently through offline sync), and endpoints to manage user identities to store per-user data. Public endpoints, on the other hand, typically have a related website and would be implemented alongside that site through App Services/Web Apps rather than Easy APIs. 

**Our choice**

In this reference architecture, the API endpoints are implemented with App Services using C# and ASP.NET, a choice that's motivated by several factors:

- The team's existing expertise in C#
- The ability to use Entity Framework to interact with the rich backend data sets
- The ability to easily test and debug APIs locally from Visual Studio
- The ease of integration with GitHub version control through Visual Studio.
 
In short, working locally in Visual Studio and then deploying to App Services integrates most easily with common developer workflows. With Easy APIs you can, of course, work with Node.js locally, manage your code in version control, and upload files to App Services, but Easy APIs is primarily oriented around working directly in the browser. 


For a further general comparison of these alternatives, see [Compute Hosting Options Provided by Azure](https://azure.microsoft.com/en-us/documentation/articles/fundamentals-application-models/). 


