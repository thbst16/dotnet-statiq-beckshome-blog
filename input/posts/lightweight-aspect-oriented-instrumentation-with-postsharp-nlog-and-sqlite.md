Title: Lightweight, Aspect-Oriented Instrumentation with PostSharp, NLog, and SQLite
Published: 4/7/2010
Tags:
    - .NET Application Architecture
---
How would you like to achieve detailed exception and trace logging, including method timing and correlation all within a lightweight in-memory database that you can easily manage and query, as exhibited below?

![.NET Logging Output](https://s3.amazonaws.com/s3.beckshome.com/20100407-Logging-Output.png)

All of this requiring nothing more of you than simply decorating your methods with a very simple attribute, as highlighted below.

![.NET Attribute Usage](https://s3.amazonaws.com/s3.beckshome.com/20100407-Attribute-Usage.png)

In this post, I’m going to demonstrate how to configure [PostSharp](https://www.postsharp.net/), an aspect-oriented framework, along with NLog and SQLite to achieve the benefits highlighted above. Before I get into the details of the configuration and aspect source code, I’ll provide a bit of background on PostSharp.

**PostSharp**

PostSharp is a powerful framework that supports [aspect-oriented programming](https://en.wikipedia.org/wiki/Aspect-oriented_programming) using .NET attributes. Attributes have been around in the .NET Framework since version 1.0. If you weren’t used to using attributes in the past, their increased usage in WCF (including WCF RIA Services and Data Services), ASP.NET MVC, the Entity Framework, the Enterprise Library and most of Microsoft’s other application frameworks will surely mean you’ll be encountering them in the very near future. PostSharp allows you to create your own attributes to meet a variety of needs (cross-cutting concerns, in aspect-oriented parlance) you may have such as persistence, security, monitoring, multi-threading, and data binding.

PostSharp has recently moved from a freely available to a commercially supported product. PostSharp 1.5 is the last open source version of the product with PostSharp 2.0 being the first release of the commercially supported product. Don’t let the commercial product stigma scare you away, both PostSharp 1.5 and 2.0 are excellent products. If you chose to go with PostSharp 2.0 you can select either a pretty liberal community edition or more powerful yet reasonably priced Professional edition. For the purpose of this post, I’ll be using the community edition of PostSharp for forward compatibility. The Community Edition includes method, field, and property-level aspects, which is more than enough for the purposes of this post. You will also find examples of PostSharp aspects on their site, in the blogosphere, and on community projects such as [PostSharp User Plug-ins](https://code.google.com/archive/p/postsharp-user-plugins/).

What makes PostSharp stand out among competing aspect-oriented frameworks is how it creates the aspects. PostSharp uses a mechanism called compile-time IL weaving to apply aspects to your business code. What this essentially means is that, at build time, PostSharp opens up the .NET intermediate language binary where you’ve included an aspect and injects the IL specific to your aspect into the binary. I’ve illustrated below what this looks like when you use .NET Reflector to disassemble an assembly that’s been instrumented by PostSharp. The first image is before a PostSharp attribute is applied to the About() method on the controller. The second image represents what the code looks like after PostSharp compile-time weaving.

**Before PostSharp Attribute Applied to About() Method**

![Disassembly Before Postsharp](https://s3.amazonaws.com/s3.beckshome.com/20100407-Disassembled-Before-Postsharp.png)

**After PostSharp Attribute Applied to About() Method**

![Disassembly After Postsharp](https://s3.amazonaws.com/s3.beckshome.com/20100407-Disassembled-After-Postsharp.png)

What this means is that you get very good performance of aspects but will need to pay a higher price at build/compile time. [Ayende provides a good overview of various AOP approaches](https://ayende.com/blog/2615/7-approaches-for-aop-in-net), including the one that PostSharp uses. Don’t be concerned by his “hard to implement” comment. The hard part was done by the creators of PostSharp, who have made it easy for you.

**Implementation of Aspect-Oriented Instrumentation**

The remainder of this post will focus on the actual implementation of the solution. Much of the code I have here was cobbled together from a blog post I archived long ago from an unknown author. I’d love to provide attribution but, like many blogs out there, it seemed to have disappeared over time. I’ll start off first with the SQLite table structure, which can be found below.

![Loggong to SQLite Table](https://s3.amazonaws.com/s3.beckshome.com/20100407-Logging-SQLite-Table.png)

The logging configuration file is very similar to [my post on logging with SQLite and NLog](http://blog.beckshome.com/index.php/2010/03/logging-to-sqlite-with-nlog/2010/03/logging-to-sqlite-with-nlog/) with minor changes to the SQLite provider version.

```xml
1	<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
2	  <targets>
3	    <target name="File" xsi:type="File" fileName="C:Temp${shortdate}.nlog.txt"/>
4	    <target name="Database" xsi:type="Database" keepConnection="false" useTransactions="false"
5	            dbProvider="System.Data.SQLite.SQLiteConnection, System.Data.SQLite, Version=1.0.60.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139, processorArchitecture=x86"
6	            connectionString="Data Source=C:ProjectsMyApp_Logging.s3db;Version=3;"
7	            commandText="INSERT into LOGTABLE(Timestamp, Loglevel, ThreadId, Message, Context, User, DurationInMs, Exception) values(@Timestamp, @Loglevel, @ThreadId, @Message, @Context, @User, @DurationInMs, @Exception)">
8	      <parameter name="@Timestamp" layout="${longdate}"/>
9	      <parameter name="@Loglevel" layout="${level:uppercase=true}"/>
10	      <parameter name="@ThreadId" layout="${threadid}"/>
11	      <parameter name="@Message" layout="${message}"/>
12	      <parameter name="@Context" layout="${ndc}"/>
13	      <parameter name="@User" layout="${aspnet-user-identity}"/>
14	      <parameter name="@DurationInMs" layout="${mdc:item=DurationInMs}"/>
15	      <parameter name="@Exception" layout="${mdc:item=exception}"/>
16	    </target>
17	  </targets>
18	  <rules>
19	    <logger name="*" minlevel="Debug" writeTo="Database" />
20	  </rules>
21	</nlog>
```
The most important component of the solution is the source code for the PostSharp aspect. Before letting you loose, I’ve highlighted some of the features of the source code to avoid cluttering it with comments:

* You need to have PostSharp (the DLLs and the necessary build/compilation configuration) set up on your machine for the aspects to work correctly. Specifically, my code works against PostSharp 2.0
* For those of you not familiar with Log4Net or the original implementations of the NDC (NestedDiagnosticContext) and MDC (MappedDiagnosticContext), the original documentation from the Log4J project provides good background.
* The NDC is used to push GUID’s on the stack which can then be used as correlation ID’s to trace calls through the stack for methods annotated with the [LogMethodCall] attribute that this code implements.
* The MDC map stores timing information in all cases and exception information in the case of an Exception in one of the calling methods annotated with the [LogMethodCall] attribute.
* To use the attribute, just decorate the method you wish to instrument with the [LogMethodCall] attribute. Then sit back and enjoy detailed instrumentation for free.

```cs
1	using System;
2	using System.Diagnostics;
3	using NLog;
4	using NLog.Targets;
5	using PostSharp;
6	using PostSharp.Aspects;
7	 
8	namespace MvcApp.Web.Aspects
9	{
10	    [Serializable]
11	    public class LogMethodCallAttribute : MethodInterceptionAspect
12	    {
13	        public override void OnInvoke(MethodInterceptionArgs eventArgs){
14	            var methodName = eventArgs.Method.Name.Replace("~", String.Empty);
15	            var className = eventArgs.Method.DeclaringType.ToString();
16	            className = className.Substring(className.LastIndexOf(".")+1, (className.Length - className.LastIndexOf(".")-1));
17	            var log = LogManager.GetCurrentClassLogger();
18	            var stopWatch = new Stopwatch();
19	 
20	            var contextId = Guid.NewGuid().ToString();
21	            NLog.NDC.Push(contextId);
22	 
23	            log.Info("{0}() called", methodName);
24	            stopWatch.Start();
25	            NLog.NDC.Pop();
26	 
27	            try
28	            {
29	                eventArgs.Proceed();
30	            }
31	            catch (Exception ex)
32	            {
33	                var innermostException = GetInnermostException(ex);
34	                MDC.Set("exception", innermostException.ToString().Substring(0, Math.Min(innermostException.ToString().Length, 2000)));
35	                log.Error("{0}() failed with error: {1}", methodName, innermostException.Message);
36	                MDC.Remove("exception");
37	                throw innermostException;
38	           }
39	 
40	           NLog.NDC.Push(contextId);
41	           stopWatch.Stop();
42	           NLog. MDC.Set("DurationInMs", stopWatch.ElapsedMilliseconds.ToString());
43	           log.Info("{0}() completed", methodName);
44	           NLog.MDC.Remove("DurationInMs");
45	           stopWatch = null;
46	           NLog.NDC.Pop();
47	        }
48	 
49	        private static Exception GetInnermostException(Exception ex)
50	        {
51	            var exception = ex;
52	            while (null != exception.InnerException)
53	            {
54	                exception = exception.InnerException;
55	            }
56	            return exception;
57	        }
58	    }
59	}
```