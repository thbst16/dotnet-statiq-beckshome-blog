Title: WCF Performance Counter Sizing - Do the Math
Published: 4/13/2010
Tags:
    - .NET Application Architecture
---
Performance counters for WCF have been available ever since the first release of WCF with the .NET 3.0 Framework. As long as these counters have been available, [Microsoft has been cautioning](https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-3.5/ms735098(v=vs.90)?redirectedfrom=MSDN) about the memory requirements and potential performance degradation associated with insufficient shared memory allocation. I thought that I had heard at the PDC that WCF 4 would fix some of this but going back to the WCF session video, it looks as if these counters won’t really be addressed by WCF 4 but instead superseded by the ETW instrumentation present in AppFabric. So, until everyone moves to AppFabric, I see a need for a bit more guidance than the “allocate enough memory” that Microsoft offers us.

**Enabling WCF Performance Counters**

Enabling WCF performance counters is a breeze and is covered pretty well elsewhere.  The configuration change below will turn on all three types of WCF counters: Endpoint, Operation, and Service.

```xml
1	<configuration>
2	    <system.serviceModel>
3	        <diagnostics performanceCounters="All" />
4	    </system.serviceModel>
5	</configuration>
```
Your options for enabling the counters are: All, ServiceOnly, and Off. WCF performance counters are included for a reason so I wouldn’t recommend disabling them entirely. Instead, as a rule of thumb you should enable “All” if you’re performing specific service debugging activities that require all the counters and should leave on “ServiceOnly” for normal operations, including in a production environment.

**Calculating Performance Counter Memory Size**

Before diving into sizing, it’s best to provide a bit of background on performance counter memory allocation. Managed performance counters consume memory that is shared across all the .NET processes running on a machine; essentially a memory-mapped file. Although the .NET Framework 1.0 and 1.1 used global shared memory, .NET 2.0 and above use separate shared memory per performance counter category, with each category having a default size of approximately 128KB (that is ¼ the default global shared memory).

You also need to know about services, endpoints, operations – the WCF counter groups:

* **Services.** Services are at the root of the WCF hierarchy. Services can have multiple endpoints and expose multiple operations. WCF has 33 performance counters for each service
* **Endpoints.** WCF endpoints provide the client access to a service through address, binding, and contract. You can provide multiple endpoints per service. WCF has 19 performance counters for each instance of an endpoint across a service.
* **Operations.** A WCF service operation is a discrete function performed by a WCF service. WCF provides 15 performance counters: per endpoint, per service.

What you’re ultimately looking to come up with is a sizing for each one of the WCF performance counter categories. Without providing a mathematical formula, I’ll walk through a brief hypothetical example to calculate the sizes. In this example, I’ll assume that we have 20 services on a machine, each of these services has 3 endpoints, and each service has 10 operations exposed across each of the three endpoints:

* We’ll assume an average size per performance counter of 350 bytes, which is a fairly conservative yet accurate estimate.
* For the service counters, we have 20 services * 33 performance counters * 350 bytes = 231,000 bytes (231 KB)
* For the endpoint counters, we’ll need 20 services * 3 endpoints * 19 performance counters * 350 bytes = 399,000 bytes (399 KB)
* Operations counters come in the largest, due to the multiplicative effect, at 20 services * 3 endpoints * 10 operations * 15 counters * 350 bytes = 3,150,000 bytes (3.15 MB).

From the above numbers, you’ll hopefully notice two things. First, I hope you now understand why I recommend the “ServiceOnly” setting unless you’re in an environment where you absolutely need the other counters. Second, even with a medium size service load, we’ve exceeded the default performance counter category maximum memory and are quickly heading for the dreaded “System.InvalidOperationException: Custom counters file view is out of memory” exception.

**Setting Performance Counter Memory Size**

Aside from the mechanics of setting the performance counter category memory size, there is only so much guidance I can provide. What you will set these values to will depend upon a couple of factors:

* Whether you’ve set WCF counters to “ServiceOnly” or to “All”. If you’ve used the former, you’ll only need to tweak the service-specific private memory. If you go with “All”, you’ll want to set each category’s memory space individually.
* The math you do for your counter categories based upon the example I provided in the previous section.

For the size of separate shared memory, the DWORD FileMappingSize value in the registry key HKEY_LOCAL_MACHINESYSTEMCurrentControlSetServices<category name>Performance is referenced first, followed by the value specified for the global shared memory in the configuration file. If the FileMappingSize value does not exist, then the separate shared memory size is set to ¼ the global shared memory setting in the configuration file, which is 528KB by default.

To specify the WCF category-specific sizes, simply set the registry value for each of the three registry keys associated with the three WCF service categories and then reboot the machine. Keep in mind that, like other application sizing activities, sizing of the WCF counter memory will need to be repeated as the number of services, endpoints, and operations change on a particular machine.