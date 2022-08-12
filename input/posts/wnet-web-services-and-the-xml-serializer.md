Title: Watch Out! .NET Web Services and the XML Serializer
Published: 4/5/2010
Tags:
    - .NET Application Architecture
---
Sometimes you know that something works a certain way but you haven’t really internalized it, you haven’t grok’ed it, until you experience it firsthand. Such was my knowledge of the interaction between .NET web services and the XML Serializer a couple of weeks ago.

While troubleshooting calls made from a smart client to back-end web services, I was using [Process Monitor](https://docs.microsoft.com/en-us/sysinternals/downloads/procmon) to see what was going on under the hood. It didn’t dawn on me at first why I was seeing dynamic generation of temporary C# files and compilation of these files using the C# compiler (csc.exe) as shown in the image below. Since the services in question were provided by a legacy mainframe and were necessarily granular and immutable, the client application was generating a lot of new proxies, meaning a lot of wasted time.

![Dynamic Proxy Compilation](http://s3.beckshome.com/20100405-Dynamic-Proxy-Compilation.png)

The items being generated and compiled, XML serialization assemblies, are necessary to guarantee speedy serialization and de-serialization of type-specific data. Although you’d ideally like to have these assemblies there all of the time, I’ve found several instances under which these assemblies are not present by default:

* If you compile your assemblies using the compiler (e.g. csc.exe) directly. This is true even if you build in release mode with optimizations enabled.
* If you build in debug mode using Visual Studio, as illustrated by the image below.
* If you’re using a third party product that leverages .NET assemblies for service-based interoperation, you might be subject to dynamic proxy compilation and not even be aware of it.

![Debug Versus Release Build](http://s3.beckshome.com/20100405-Debug-Versus-Release-Build.png)

In the final part of this post, I’ll assume that you’re interested in knowing the ways you can get around dynamic XmlSerializer generation and compilation. As you’ve probably derived from the previous section, building a solution with VisualStudio or directly from the underlying MSBuild tool will take care of call the [XML Serializer Generator (sgen.exe)](https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-2.0/bk3w6240(v=vs.80)?redirectedfrom=MSDN) for you, as illustrated in the following image.

![Visual Studio SGEN](http://s3.beckshome.com/20100405-Visual-Studio-Sgen.png)

You can also invoke sgen.exe directly. If you do so, be sure to use the /C flag to pass compiler options. Alternately, if you’re using WCF, you can choose the alternative DataContractSerializer, which has been optimized to avoid the overhead of generating and using the extra assembly and shared only the data contract without sharing the underlying type data.