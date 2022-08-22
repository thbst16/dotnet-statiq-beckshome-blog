Title: "Microsoft Service and Integration Technologies - WCF 4.0, AppFabric, BizTalk 2010"
Published: 3/28/2010
Tags:
    - .NET Application Architecture
---
One of the things I’m often asked to do for clients is to create an applicability matrix. That is, which technology applies best to which particular challenges in an enterprise? There would seem to be an acute need for this type of clarification in the realm of Microsoft’s service technologies. With the recent releases of Web Process Activations Services (WAS) on Windows Server 2008, WCF 3.5 and 4.0, Windows Server AppFabric, BizTalk 2009 and 2010, and Windows Azure AppFabric, the waters of Microsoft’s service and integration technologies is muddy indeed. In this post, I’m going to provide some clarification; explaining what new service and integration offerings are on the way from Microsoft, offering a frame of reference on how I see them applying to enterprise customers, and furnishing references to materials you can use to educate yourself in these technologies.

Let’s start off with a quick tour of Microsoft’s new service and integration offerings. Specifically, I’m going to cover WCF 4.0, Server AppFabric, and Azure AppFabric. In this overview, I’m going to restrict the discussion to technologies that specifically relate to the challenges of traditional large enterprise application integrations. Interesting aspects of Microsoft’s new offerings such as WCF 4.0 RESTful service support (incorporated from the WCF Rest Starter Kit) and [AppFabric Caching](https://docs.microsoft.com/en-us/previous-versions/appfabric/ff383731(v=azure.10)?redirectedfrom=MSDN) (formerly known as ‘Velocity’) will not be covered in detail.

**Windows Communications Foundation (WCF) 4**

Release focuses on ease of use along with new features, such as routing, support for WS-Discover, and enhancements from the WCF Rest Starter Kit.

<u>Key Enterprise Application Features</u>

* A complete message routing solution that is useful for the following scenarios: redundancy, load balancing, protocol bridging, and versioning
* Support for the WS-Discovery protocol that allows the discovery of services on a network. Support is provided via managed mode discovery which uses a centralized discovery proxy and via adhoc mode, in which the location of the service is broadcast.

<u>References</u>

* [Extend your WCF Services Beyond HTTP with WAS](https://docs.microsoft.com/en-us/archive/msdn-magazine/2007/september/iis-7-0-extend-your-wcf-services-beyond-http-with-was)
* [A Developer’s Introduction to Windows Communication Foundation 4](https://docs.microsoft.com/en-us/previous-versions/dotnet/articles/ee354381(v=msdn.10)?redirectedfrom=MSDN)
* PDC’ 09: What’s New for Windows Communication Foundation 4

**Windows Server AppFabric**

The best way to think of Windows Server AppFabric is as a replacement for the COM+ hosting environment. In the same way that WCF unified/replaced web services, remoting, and DCOM, AppFabric is replacing the COM+ hosting environment. Hosting administration, monitoring services, and management tools allow AppFabric to play this role. Also includes workflow persistence and a distributed caching platform.

<u>Key Enterprise Application Features</u>

* A WAS-based hosting environment, which includes durable workflow hosting. Includes tools for managing, monitoring, and querying in-flight services and workflows.
* Workflow persistence that allows AppFabric workflows to scale across machines. This includes the ability to monitor long-running workflows.
* Health monitoring and troubleshooting of running WCF and WF services. High performance instrumentation based on Event Tracing for Windows (ETW) with analytics from a SQL monitoring store leveraging SQL Server Reporting Services (SSRS).
* Management of services and workflows through the AppFabric dashboard, an extension to the IIS manager. PowerShell cmdlts enable management of services and workflows from the PowerShell console and enable further automation of AppFabric.

<u>References</u>

* Microsoft .NET Framework 4 and Windows Server AppFabric Virtual Labs
* PDC ’09: Application Server Extensibility with Microsoft .NET 4 and Windows Server AppFabric
* Apress: Pro Windows Server AppFabric

**Windows Azure AppFabric**

Branded as the Azure cloud-based version of its Windows Server-based counterpart, Azure AppFabric is perhaps better understood as a parallel service in the cloud. It provides features that Server AppFabric doesn’t, such as cloud-based relay, a service registry, and a service bus buffer. At the same time, several of Server AppFabric’s core features such as workflow persistence and health monitoring either aren’t built in or don’t make sense for the cloud-based version. Remains to be seen if these two products ever achieve true parity.

<u>Key Enterprise Application Features</u>

*Relay service that removes the need for point-to-point bindings, instead routing non-transactional calls through the cloud.
* Service bus registry that provides an ATOM feed of services listening on a particular namespace.
* Variety of service bindings that represents a rough subset of the WCF bindings. Includes a WS-compliant binding as well as a TCP binging that operates in several modes, including a hybrid mode that can promote a connection from a cloud-based relay to a more direct connection.
* Cloud-based service bus buffer queuing service. MSMQ-like, utilizable by both the client and the server with the condition that the queues are cloud-based. Allows the messages to be stored on the bus for a configurable amount of time, even if the service endpoint is not available.
* Robust service authentication service, based upon claims-based application-to-application authentication.

<u>References</u>

* Windows Azure Platform Training Kit
* Mix ’10: Connecting Your Applications in the Cloud with Windows Azure AppFabric
* [Working with the .NET Service Bus](https://docs.microsoft.com/en-us/archive/msdn-magazine/2009/april/working-with-the-net-service-bus)

What I’ve found is that knowledge of these new service and integration offerings alone does not get you to the point where you intuit when to apply them to enterprise application integration challenges. Therefore, I have begun to cluster these technologies together and think about what the best use cases are for each of the respective technologies. The image below represents these clusters, along with the archetype use case and particular features of the clusters’ technologies. This clustering represents a fundamental simplification of reality and doesn’t account for many of the shades of gray. Decisions such as whether workflows are best hosted in WF under AppFabric or under BizTalk are best made by application architects, based upon their knowledge of the organizational, business and technical constraints that impact their applications. That said, these clusters represent what I feel to be sound heuristics for Microsoft service and integration decisions over the next several years.

![Microsoft Service Integration Technologies](https://s3.amazonaws.com/s3.beckshome.com/20100328-Microsoft-Service-Integration-Technologies.png)