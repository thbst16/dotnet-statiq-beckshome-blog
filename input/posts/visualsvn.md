Title: VisualSVN
Published: 3/10/2008
Tags:
    - Technology Guides
---
I’ve been contemplating the move towards a self-hosted Subversion repository for quite a while. My earlier attempts worked but left me with a lot of inconvenient and sometimes quirky side effects. These experiences always led me back to hosting Subversion on Linux, which is really where it works most naturally. Recently, however, I decided to retry my luck with Subversion hosting on Windows and I made the call to go with a “package” instead of doing the Apache / Subversion integration myself.

![VisualSVN](https://s3.amazonaws.com/s3.beckshome.com/20080310-Visual-SVN.png)

The tool that I went with, [VisualSVN](https://www.visualsvn.com/), is a Windows version of Subversion that targets primarily Microsoft developers using VisualStudio as their development platform. Matter of fact, the Subversion server package is freely distributed and the actual product that is sold is the Visual Studio plugin that allows you to tap into Subversion from Visual Studio. With a 30 day trial period and $49 price tag, I decided that it couldn’t hurt to try it out. My findings are below:

* **VisualSVN Server** – The VisualSVN server, as mentioned earlier is a freely distributed product. You can get this piece of software whether or not you ultimately decide to buy and use the Subversion Visual Studio plugin. The server runs exclusively over HTTP / HTTPS (using OpenSSL) and does not support Subversion’s binary protocol or running Subversion over SSH. Obviously, this means that Apache is in play. A version of Apache is included in the distribution. Initial configuration of the server is very easy, the setup instructions describe the extent of it. As I blogged about previously, this changes a bit if you try to get Apache and IIS to run side-by-side. In this case, you need to be very explicit and tell the very greedy IIS to stop listening on other IP addresses so that port 80 can be shared by IIS and Apache. I included links to the Microsoft article in my earlier post. In this case, you’ll want to use httpcfg delete iplisten -i xxx.xxx.xxx.xxx to stop IIS from listening on the port Apache is running on.

    The folks who designed VisualSVN added some cool management functionality that shields the administrator from lower level Subversion commands. Implemented as a Windows MMC snap-in, Subversion repository administration be performed right alongside other server management tasks. The MMC enables one step creation of repositories (with or without the standard Subversion folder structure), creation of users and groups, and assignment of user privileges to repository actions.

* **VisualSVN Visual Studio Plugin** – As useful as the server is, the real product is the VisualStudio plugin. The most recent version of this plugin works on VisualStudio 2008 so I thought I’d install it and give it a whirl. Installation is fairly easy. Both TortoiseSVN and the VisualSVN plugin must be installed. I don’t know exactly how VisualSVN communicates with Tortoise but it seems to make sense to leverage an existing Windows Subversion library rather than building everything from scratch. Using both the plugin and Tortoise gives you two ways to work with Subversion. In my experience with other Java IDE plugins (Netbeans and Elcipse), this is sometimes necessary to get around the shortcomings of the browser plugin.

    Adding a project to VisualSVN using the plugin is, as it well should be, a relatively easy task. VisualSVN has some intelligence built in above and beyond the basicTortoiseSVN libraries. In my case, the plugin didn’t add my Visual Studio settings, binaries, or a bunch of MP3 and JPEG photos that represent content and really didn’t belong under source control. Other than that, a lot of the processing is just handed over to TortoiseSVN. The SVN UI presented by the plugin should all be pretty familiar to you if you’ve ever used TortoiseSVN before.

This looks to be my keeper for Subversion hosting. Now I need to port over my existing repositories into the VisualSVN server.

