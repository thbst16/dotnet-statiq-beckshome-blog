Title: Web Hosting Provider Cutover
Published: 9/25/2011
Tags:
    - Technology Guides
---
I feel like I’m in the homestretch of my migration off of my current hosting provider – [FullControl](https://www.fullcontrol.net/). Nothing against these guys; they’ve been an absolute stellar service provider. I just don’t need the dedicated virtual server I was paying for with them. It’s a short story that came down to rightsizing my hosting provider to align with my current needs. I’ll tell the somewhat longer version of the story in this blog post though since there are a couple of interesting corollaries along the way.

I have three requirements of a hosting provider. Once they could fulfill these 3 requirements, I am looking to optimize on costs. My three requirements are:

* Host WordPress blogs.
* Provide Subversion source control services.
* Support [OSQA](https://www.osqa.net/), which essentially means running Python and Django

![Dreamhost Cutover](https://s3.amazonaws.com/s3.beckshome.com/20110925-Dreamhost-Cutover.jpg)

Both my ex-provider and my new provider met these three requirements – my ex-provider at the high cost side and my new provider at the low cost side. At both ends of the price extremes, they still have similar architectures though – a single server that can host PHP, Python and MySQL. Despite the fact that one is Windows and one is Linux, they’re still both standard hosting stacks.

When I first started thinking about moving hosting providers, I considered some slightly more esoteric approaches, especially as they relate to blog hosting. I did a bit of probing and they all fell short in one area or another but are worth mentioning just due to the irregular architectures they embody.

1. **WordPress on Windows Azure.** You can most certainly host WordPress on Windows Azure and SQL Server Azure. Zach Owens is an evangelist for Microsoft who is supporting this and [blogs all about it](http://wordpress.visitmix.com/development/migrating-a-wordpress-site-from-mysql-to-sql-serversql-azure). It sounds interesting but I get the sense that this is just some sort of Microsoft pet project and the floor on it could drop out at any time.
2. **WordPress on Amazon EC2 Micro Instances.** I loved the ideas Ashley Schroder presented in his [blog post on clustering WordPress on EC2 micro instances](http://www.aschroder.com/2011/06/clustering-wordpress-on-amazon-ec2-micro-instances/). His approach and experiences are worth reading about and will cause you to think about and investigate EC2 spot instance pricing structure, if nothing else.
3. **BlogEngine on EC2 Micro Instances using SQL Azure.** A radical extension of Ashley’s ideas onto the Microsoft platform: host [BlogEngine.NET](http://www.dotnetblogengine.net/) on EC2 Micro Instances and talk to SQL Azure on the back end. This fell apart on BlogEngine’s architecture, which many posts indicate doesn’t scale out at all due to architectural limitations in the DAL and caching layers.

The more I thought about it, the more I just want a stack that just works for my personal web apps. As exciting as the above options were, they sounded like massive black holes that would suck in my free time. I ultimately decided to go with a simple solution: the tried-and-proven Dreamhost (http://www.dreamhost.com), a Linux provider, for my needs. I get what I need for less than $10 US per month and I can ramp up Amazon EC2 spot instances when I need a throw-away playground. The move over was a lot easier than I expected, consisting of the following three steps:

1. [Export WordPress content](https://codex.wordpress.org/Tools_Export_Screen) from my old provider wordpress site into my new site.
2. Flip over DNS to point at my WordPress blog on my new provider’s site. This included flipping over DNS on all of my binary content (e.g. images) that I host on Amazon S3 and redirect to with a CNAME entry from a beckshome subdomain.
3. Flip the switch on the DNS routing for Google Apps after I noticed my beckshome.com email dried up for a couple of days.