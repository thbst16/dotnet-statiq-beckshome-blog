Title: "IIS and Apache Side-by-Side"
Published: 11/27/2006
Tags:
    - Technology Guides
---
This weekend, I set out on the daunting task of trying to add an Apache Web server to my existing Windows 2003 production server installation. “Why would you go about doing such a crazy thing”, you might ask. The answer is that, in short, it’s the only way to host an HTTP-accessible instance of Subversion on a Windows box. I’m looking to consolidate all of my hosted software: .NET, Java, and infrastructure, onto a single platform. Since I use Subversion to enable my location independence with respect to computers I use, this application needed to be ported as well.

![IIS and Apache Side-by-Side](https://s3.amazonaws.com/s3.beckshome.com/20061127-IIS-And-Apache-Side-By-Side.png)

I expected to learn a lot of new stuff about Subversion and Apache through this process. I didn’t expect (perhaps ignorantly so) that I’d encounter “learning opportunities” on Windows as well. As it turns out, Microsoft has gone ahead and made the assumption that if you’re running IIS on your Windows Server, nothing else should be running on port 80. That is, out of the box, IIS exhibits very greedy behavior, binding to all available IP addresses on the server, not just the ones that are explicitly assigned to Web sites.

It turns out that this is a case of socket pooling gone awry. To solve this problem, you must literally slap IIS on the hand and tell it to stop listening to all of those ports. After trying a couple of legacy solutions, such as setting the DisableSocketPooling property, I landed upon a solution that works in the IIS 6.0 post-Winsock era. This Microsoft Support Article provides all of the details you would need to manage the IP inclusion list in IIS 6.0 and solve this problem should you run into it. Also, should you need the Windows 2003 support tools referenced in this article but not have immediate access to your Windows 2003 CD, you can [find the tools here as well](http://www.microsoft.com/downloads/details.aspx?familyid=6EC50B78-8BE1-4E81-B3BE-4E7AC4F0912D&displaylang=en).

I’m happy to report that I now have IIS and Apache running side-by-side on the same Windows machine; each on port 80 using different IP addresses. Getting Subversion up and running was another issue entirely...