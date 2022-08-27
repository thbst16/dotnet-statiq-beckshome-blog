Title: "The Innovation Engine at Yahoo - Pipes, OmniFind and TagMaps"
Published: 2/21/2007
Tags:
    - Legacy Blog
---
The innovation engine at Yahoo is heating up, looking to get Yahoo back in the race with the “Big Boys”, rivals Google and Microsoft. In an environment categorized by copycat service offerings and one-upmanship, Yahoo’s offerings are refreshingly unique. I cover three of the most recent services that I’ve been playing around with and that I think will prove entertaining to my readers as well – Pipes, OmniFind Yahoo! Edition, and TagMaps.

* **Pipes** – No less a luminary than Tim O’Reilly called Pipes [“A milestone in the history of the Internet”](http://radar.oreilly.com/archives/2007/02/pipes-and-filte.html). Pipes is a browser-based visual editor that allows you to take input from one source and pipe it (in UNIX parlance) to another source. Along the way, you can apply a series of filters and transformations to manipulate the data. The data sources start and end as common feeds (RSS, RDF, etc). What you do with the data between its input and output is constrained mostly by your imagination.

    ![Yahoo Pipes](https://s3.amazonaws.com/s3.beckshome.com/20070221-Yahoo-Pipes.gif)

    Yahoo! was certainly not first on the scene with this idea. Dapper and others have preceded them in this regard. What Pipes brings to the party that no one else does is a really cool visual environment that allows you to trace the path of the data through the transformations and filers, interactively debugging along the way based upon the value of the successive outputs. It also has this cool reuse flavor to it, where you can experiment with, tweak, learn from, and potentially improve or fork off new versions of other peoples pipes or just reuse them in a black box sense.

    After you’ve read Tim O’Reilly’s introduction, I encourage you to play around with Pipes. Although seeing is believing, you’ll learn best by actually doing.

* **OmniFind Yahoo! Edition** – Product of a nifty partnership with IBM, the OmniFind Yahoo! Edition is an enterprisey search solution that is the baby brother to IBM’s commercial OmniFind enterprise product. Built on top of the open source Apache Lucene search engine, OmniFind has the solid lineage necessary to be considered worthy of the task.

    ![Yahoo OmniFind](https://s3.amazonaws.com/s3.beckshome.com/20070221-Yahoo-Omni-Find.png)

    The product is a very easy install, whether on Windows or Linux, requiring very few steps to get the product up and running. OmniFind returns search results against locally indexed documents and the Internet, with the results being returned in the familiar Yahoo! look and feel. For those interested, the UI can be styled to match a particular site’s look and feel or you can go the direct route and work with the exposed REST APIs.

    With the pricetag (free) and support for a couple hundred file types, there’s little not to like about OmniFind. Search performance has proven to be very fast with a few thousand documents. Indications are that the tool scales pretty well although the indexing process can be quite processor intensive and there are a couple of known issues with cleaning up very large temp files that could eat into your available disk space.

* **TagMaps** – I stumbled onto this product a couple of weeks ago while looking for some information about creating GeoRSS feeds. TagMaps is another way of visualizing data (tags in this case) on maps. I must confess that seeing tags on a map takes a bit of getting used to. I found that the best way to indoctrinate myself was by using Trip Explorer

    ![Yahoo Tag Maps](https://s3.amazonaws.com/s3.beckshome.com/20070221-Yahoo-Tag-Maps.jpg)

    Trip Explorer is a mashup of TagMaps and Yahoo! Travel users’ public Trip Plans. What’s cool about Trip Explorer is that the clustering of tags reveals hidden tour gems that you might not otherwise find on a traditional map mashup. These gems become more evident (and detailed) as you progressively zoom in.

    TagMaps is built upon Yahoo’s Flash maps, which are very interesting in their own right and need to be experienced if you haven’t yet had the chance. Aside from using Yahoo’s canned Explorer TagMaps, of which Trip Explorer is one, you can create your own TagMaps mashups. Simply create a GeoRSS feed or select an existing GeoRSS feed that returns a set of weighted tags for a given lat/lon bounded box. Easier said than done, I know. I’ll be writing more about how to do this in a coming blog entry. Until then, give this a look.