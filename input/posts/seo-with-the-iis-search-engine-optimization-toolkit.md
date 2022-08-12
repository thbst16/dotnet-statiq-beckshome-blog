Title: SEO with the IIS Search Engine Optimization Toolkit
Published: 7/7/2009
Tags:
    - New Technology
---
When ScottGu puts the time into creating a mini-tutorial for a new technology, it’s usually something worth investigating. After seeing [his tutorial / overview](https://weblogs.asp.net/scottgu/iis-search-engine-optimization-toolkit) of the new IIS Search Engine Optimization Toolkit, I decided I ought to give it a look. With the new blog running WordPress on IIS, this seemed especially timely and relevant.

As Scott mentions in his blog, a prerequisite to getting the IIS SEO Toolkit up and running is the installation of the [Microsoft Web Platform Installer](https://weblogs.asp.net/scottgu/microsoft-web-platform-installer). I was surprised how easy this installation went. When the installation is complete, you’ll have a new icon on your desktop and a new “Management” section within the IIS admin tool. The Installer looks like a great tool although I’m sure that some (myself included) will be leery about Microsoft installing server-related software on their machines.

![IIS SEO - New Admin Features](http://s3.beckshome.com/20090707-IIS-SEO-New-Admin-Features.png)

I followed ScottGu’s recommendations for installing and running the tool. After running it both against Scott’s site and then performing some follow-up analysis, there were several things that I felt warranted a bit further explanation:

1. The scan of my blog took a lot longer to run. This was on the order of 8 minutes for my blog versus the 13 seconds Scott quotes. My suspicion is that, especially as your site’s link depth increases and you point towards more external media, the scan takes longer to run and pseudo-index it all. In short, the IIS SEO Toolkit is doing a full spidering of your web site and the time to do so will vary according to the size and complexity of your site.
2. Scott mentioned but didn’t go into a lot of detail on the robots exclusion and sitemap / site indexing tools. I was hoping that there would have been a bit more automation that would occur after the initial site analysis was run but was disappointed to find out that this was not the case. These tools look to be little more than editors slapped on top of these files.
3. On the positive side, there’s a lot more that this tool can do than was covered in ScottGu’s brief post. In short, the analysis provides four information groupings: violations, content, performance, and links. Of these, ScottGu only covers one, Violations. I offer some more information on the other capabilities and features below.

**Site Analysis Trending Capabilities**

The IIS SEO Toolkit stores historical analysis metadata and details. This effectively affords you the capability to perform analysis and trending of your site’s SEO and other critical metrics over time. You can see below how my site changed between two different analysis runs.

![IIS SEO Analysis History](http://s3.beckshome.com/20090707-IIS-SEO-Analysis-History.png)

**Content Summary**

The content summary offers an abundance of information on content types, hosts, link, files, titles, and keywords. This information is useful for SEO and other site maintenance activities. The image below illustrates one example of the content summary – the pages with broken links summary.

![IIS SEO Content Summary](http://s3.beckshome.com/20090707-IIS-SEO-Content-Summary.png)

**Performance Summary**

The performance summary section provides information on slow pages, pages with a large amount of resources, and page performance metrics by content and directory type. These statistics require a bit of interpretation. The image below is of performance by content type. This report allows further investigation as to why some content types categorically take longer to render than others do.

![IIS SEO - Performance Summary](http://s3.beckshome.com/20090707-IIS-SEO-Performance-Summary.png)

**Query Capabilities**

All of the canned reports in the IIS SEO Toolkit are backed by a query engine. The ability to directly query the data is also provided using a simple query builder. As of this release, it looks as if queries are restricted to a single analysis run. It would be nice in the future if the queries could be expanded to span multiple analysis runs and provide a longitudinal picture of a site’s evolution.

![IIS SEO - Query Capabilities](http://s3.beckshome.com/20090707-IIS-SEO-Query-Capabilities.png)

