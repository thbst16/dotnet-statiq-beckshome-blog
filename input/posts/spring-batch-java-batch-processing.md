Title: Spring Batch - Java Batch Processing
Published: 5/7/2007
Tags:
    - New Technology
---
Eliminating or reducing enterprise system batch processing is the bane of many architects looking to convert large-scale legacy systems to current platforms. Some believe, rightly or wrongly so, that mainframe-style batch has no place in modern system architectures and attempt to eradicate its existence entirely. Others are a bit more accepting and attempt to understand the role that batch processing fills in enterprise application architectural space. Even for these people, finding people with the skills to engineer batch processing systems with these newer technologies is not an easy proposition since little or nothing has been written about batch on the Java or .NET platforms.

I’ve seen various attempts at non-mainframe batch processing over the years from the simple CRON / Quartz type attempts to more sophisticated approaches that handled concepts such as scheduling, job control, retry and rollback, and parallel processing. There are commercial applications that purport to take care of much of this for you. Many of these are, for better or worse, nothing more than mainframe tools ported to the Java and .NET platforms with little regard for the differences between mainframe and non-mainframe architectures.

I’ve seen several attempts at batch processing frameworks in .NET. The earlier versions (pre .NET 2.0) of Rocky Lhotka’s [Component Scalable Logical Architecture (CSLA)](https://cslanet.com/) included admittedly simple batch functionality. Avanade’s Avanade Connected Architecture (ACA.NET), which formed the backbone of Microsoft’s Enterprise Library, had a batch element, entitled fittingly ACA Batch. Until recently, the Java open source community had only produced several half-hearted batch processing architectures which, given their lineage as the first “true” legacy replacement technology, is actually more than a bit disappointing.

A couple of month’s back, word got out that Rod Johnson, the brains behind the widely respected Spring framework, was going to be presenting the Spring Batch Framework at this year’s JavaOne conference. Yesterday, the formal announcement was made of the addition of Spring Batch to the Spring portfolio. A combination of Interface21 (the folks behind Spring) and Accenture (the folks behind Avanade) resources have collaborated in the creation of this batch framework.

I haven’t checked out the source code yet but Spring Batch purports to be non-dependant on other Spring facilities. The architectural diagram for Spring, which is shown below, won’t tell you much.

![Spring Batch - Java Batch Processing](https://s3.amazonaws.com/s3.beckshome.com/20070508-Spring-Batch-Java-Batch-Processing.png)

Perhaps more telling are the use cases upon which their framework is based:

* Simple Batch Repeat
* Automatic Retry After Failure
* Commit Batch Process Periodically
* Asynchronous Chunk Processing
* Copy File to File in a Batch
* Massively Parallel Batch Processing
* Manual Restart After Failure
* Sequential Processing of Dependant Steps
* Partial Processing
* Whole-Batch Transaction
* Scheduled Processing.

If any or all of these things are present needs of your existing system and will need to be replaced over time, I encourage you to look at Spring Batch. Even if you’re developing in a language other than Java, take a look at Spring Batch. If historic success provides even the slightest indicator of future success, than Spring Batch will be another exciting and innovative addition to the Spring Suite.