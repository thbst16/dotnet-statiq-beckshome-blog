Title: WCF Concurrency and Instancing
Published: 3/22/2010
Tags:
    - .NET Application Architecture
---
I’ve been ranting to some colleagues about a particularly useful table that showed the interactions between WCF’s *InstanceContextMode* and *ConcurrencyMode* behaviors.  I referenced it in a conversation again today and decided that I needed to go hunt down the phantom table so that it haunted me no longer.

I thought the table was in Lowy’s Programming WCF Services. Full attribution to [Essential Windows Communication Foundation: For .NET Framework 3.5](https://www.amazon.com/Essential-Windows-Communication-Foundation-WCF/dp/0321440064/), one of my other favorite WCF books. I’ve copied the table below so that I can refer to it by hyperlink forever more.

The table is awesome because it shows you the results of the different ways of combining these two important WCF concurrency settings, including the default combination.  Without some trial and error, it’s not always easy to intuit what the combination of these settings means. This table makes it easy.

![WCF Context and Concurrency](https://s3.amazonaws.com/s3.beckshome.com/20100323-WCF-Context-and-Concurrency.png)