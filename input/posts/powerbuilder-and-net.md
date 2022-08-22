Title: "PowerBuilder and .NET"
Published: 3/22/2010
Tags:
    - .NET Application Architecture
---
I’ve had the opportunity to spend the last week or so investigating a system integration challenge involving PowerBuilder and .NET communicating with web services hosted on a mainframe. It’s been an interesting experience that’s enabled me to dive deep into .NET and to learn a bit about where PowerBuilder is at and where it’s heading. My outtakes follow:

* **What’s .NET got to do with it?** I was totally perplexed at first when I got asked to assist with a challenge integrating a PowerBuilder client with mainframe web services. I saw .NET in the middle of the diagram but was perplexed “what’s .NET have to do with a PowerBuilder client-to- mainframe integration?”
* **.NET is PowerBuilder’s future.** Seems that the future of PowerBuilder is bound to .NET. Check out the diagram below or the PowerBuilder 11.5 (current) and 12.0 (beta) features. Support for Code Access Security (CAS), IIS7, WPF and WCF.

    ![Powerbuilder Diagram](https://s3.amazonaws.com/s3.beckshome.com/20100322-PowerBuilder-Diagram.gif)

* **Why Use PowerBuilder at all?** Been asking myself this here. PowerBuilder 12 is being touted as a “Complete and highly productive .NET development solution”. Uh… isn’t that VisualStudio? Even with legacy compatibility considerations, isn’t it eventually just time to cut the cord? Didn’t we learn anything from VB.NET?
* **Diving Deep.** To my final point, as a non PowerBuilder guy, I’ve been relegated to analyzing the PowerBuilder-to-.NET interaction from the outside in. The process of generating proxies (shown below), first in .NET for a SOAP-based service and then in PowerBuilder to interoperate with .NET, provides lots of opportunities for suboptimal behavior. [ProcessMonitor](https://docs.microsoft.com/en-us/sysinternals/downloads/procmon) from SysInternals really is man’s best friend in these situations. When I compare the traces from the integration attempts with my pure .NET test calls, I’m seeing a bunch of opportunities for optimization. Now I need to find someone who understands how PowerBuilder works under the hood so that we can determine how to optimize the .NET invocation.

    ![Generating PowerBuilder Proxies for .NET](https://s3.amazonaws.com/s3.beckshome.com/20100322-Generating-PowerBuilder-Proxies-for-DotNet.png)