Title: "Adding Flickr's Flash Photo Viewer to Your Blog"
Published: 2/27/2007
Tags:
    - Technology Guides
---
One of my original intents of registering the beckshome.com domain name was to publish photos of my new baby son or daughter. That was two years and two daughters ago and, until this weekend, photos were nowhere to be found on my blog. I host my blog on the Windows platform and had no <u>desire/time</u> to do any of the following: (a) buy a separate package for image management; (b) cobble together an ASP.NET solution to manage my photos; (c) switch blogging software to a tool like Community Server that has integrated photo management. Furthermore, I already manage my photos on Flickr and I’m more than happy with the service, user experience, and the cost-benefit. What I really needed was a way to integrate my existing Flickr photos into my current .NET-based blog (DasBlog). The pursuit of this goal is what this blog entry is all about.

Being a regular blog reader, what I’ve seen a lot of out there are the Flickr badges. These badges, available in either HTML or Flash versions (like the one in the sidebar of this blog), are pretty slick and can be found pretty much everywhere on the Web. The problem with these badges is that they only offer the opportunity for shallow integration. Click on the badge and bye-bye blog, you’re zipped off to Flickr’s site to look at the photos. Since I aspired to achieve a bit deeper integration, I needed a different approach.

Next thing that I looked into was programmatic access to the Flickr API or a pre-existing solution that I could use wholesale or reproduce with little effort on my part. The [Great Flickr Tools Collection](https://www.quickonlinetips.com/archives/2005/03/great-flickr-tools-collection/) has a vast assortment of very interesting tools – none of which quite seemed to meet my needs. I checked out the Flickr.NET API Library, which was written about in a Coding4Fun post and can be found for download here. It’s very well done and, although it probably won’t be the last time I mention this API in my blog, it will be the last time I mention it in this posting.

What I eventually stumbled on was a simple and elegant solution that got me exactly what I wanted by embedding the Flickr slideshow viewer into a custom page on my existing blog. [Paul Stamatiou has an excellent post](https://paulstamatiou.com/how-to-quickie-embedded-flickr-slideshows/) on his blog on how to do just that. By using an iframe and setting some API attributes you can get this up and running very quickly; qualifying this as a super easy hack that just works. Note that only photos marked as public will be displayed.

With the Flickr slideshow viewer up and running, I only needed to add the ability to select between multiple photo groups and I was done. With the actual Flickr viewer taking care of all of the real AJAX work, all that was needed was a bit of light JavaScript to tie this all together. Below you will find the code that does all of the lifting.

```js
1	<script type="text/javascript">// <![CDATA[
2	      function changeSlideshow(url, title)
3	      {
4	            document.getElementById("SlideShow").src=url;
5	            document.getElementById("Title").innerHTML=title;
6	            return false;
7	      }
8	// ]]></script>;
```

Clicking on any of the photo group links / thumbnails makes a call to the above function passing the URL for the slideshow in the manner stipulated in Paul’s article for populating the slideshow viewer. The title is also passed so that the title of the page can be updated. You can see this at work on my new photopage. By viewing the page source, you can see the exact mechanism I used to make this work. If you’re have any questions, feel free to drop me a line.