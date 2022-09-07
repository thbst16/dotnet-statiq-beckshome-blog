Title: Beckshome on Statiq
Published: 9/7/2022
Tags:
    - Blog
    - Statiq
---
After 7 years of being dormant, the beckshome.com blog hummed back to life over the past month. What started with [a full .NET hosted blog](/2006/06/das-blog-installation), [moved to Wordpress](/2011/09/web-hosting-provider-cutover) and then was exported from Wordpress to create static content in 2015, which is where things stood for the past 7 years. Once you get beyond the Hello World examples of getting Statiq Web stood up, there's a lot of work to be done to get a blog ported over. Some of this work is brute force conversion of years of blog posts into Markdown format. Some of it lies in the details of Statiq configuration. Rather than reinventing the wheel on this latter topic, I point to a [great blog post on migrating to Statiq](https://www.techwatching.dev/posts/migrating-blog) along with two GitHub repositories containing full Open Sourced versions of live Staiq sites -- from [Alexandre Nédélec](https://github.com/techwatching/techwatching.dev) and  

* Port to statiq - backreference to Das Blog
* On github, other great links on how to do it right...
* Some lessons learned
    * _directory.yml for URLs
    * web.confi for hosting on Azure App Services / IIS
    * google analytics from _head.cshtml
    * enlighterer in _layout.cshtml
    * _post-footer.cshtml for giscus
    * _sidebar.cshmlt and _social_links.cshtml for social links (could do other like Twitter)
