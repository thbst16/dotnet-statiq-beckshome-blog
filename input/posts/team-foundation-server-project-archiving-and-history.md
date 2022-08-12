Title: Team Foundation Server Project Archiving and History
Published: 5/2/2010
Tags:
    - .NET Application Architecture
---
One of the things I was really eager to do was help one of our clients manage the archival and history of projects within their TFS repository. Historically, VSS volumes sizes have gotten out of control over time, resulting in commensurately poor performance. Obviously, a SQL Server backing database offers lots of advantages over the Jet database engine but even SQL Server performance will degrade over time as the history volume in long-running projects backs up.

I was hoping that TFS 2008 had built in functionality to manage project archiving and history management. Not only does the TFS 2008 not have such a function but the co-mingling of data (all the projects on a server write to the same database) means that it’s nearly impossible to break out what data belongs to what project and apply different types of information lifecycle management rules such as modifying the type of storage used, applying specific backup criteria to different projects, or taking a project completely offline so that it no longer impacts the performance of the TFS database but can still be retained for historical purposes.

The good news is that, if you’re willing to make the move, TFS 2010 has functionality to explicitly address the requirement for TFS archiving and history management. TFS 2010 Team Project Collections allow you to organize similar projects into collections and, most importantly for our needs, allocate a different set of hardware resources for each team project collection. The benefit of this setup and applicability to the intent of this blog post should be immediately obvious. The downside of this approach is that you can’t work (link work items, branch & merge, etc.) across project collections. An annotated version of a diagram from the [MSDN Team Project Collections documentation](https://docs.microsoft.com/en-us/azure/devops/server/admin/manage-project-collections?view=azure-devops-2020&viewFallbackFrom=azure-devops) can be found below.

![Team Project Collections](http://s3.beckshome.com/20100502-Team-Project-Collections.png)