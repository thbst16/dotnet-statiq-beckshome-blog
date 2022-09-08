Title: Beckshome on Statiq
Published: 9/8/2022
Tags:
    - Blog
    - Statiq
---
After 7 years of being dormant, the beckshome.com blog hummed back to life over the past month. What started with [a full .NET hosted blog](/2006/06/das-blog-installation), [moved to Wordpress](/2011/09/web-hosting-provider-cutover) and then was exported from Wordpress to create static content in 2015, which is where things stood for the past 7 years. Once you get beyond the Hello World examples of getting Statiq Web stood up, there's a lot of work to be done to get a blog ported over. Some of this work is brute force conversion of years of blog posts into Markdown format. Some of it lies in the details of Statiq configuration. Rather than reinventing the wheel on this latter topic, I point to a [great blog post on migrating to Statiq](https://www.techwatching.dev/posts/migrating-blog) along with two GitHub repositories containing Open Sourced versions of live Statiq sites -- from [Alexandre Nédélec](https://github.com/techwatching/techwatching.dev) and from [David Glick](https://github.com/daveaglick/daveaglick), the creator of Statiq.

There are several lessons I learned beyond what is in the aforementioned blogs. I have highlighted and documented these items below:

<h3>Post Destination Paths</h3> 

The Clean Blog site theme documentation [provides an example](https://github.com/statiqdev/CleanBlog#post-destination-path) of using the _directory.yml file in the posts folder to output posts with computed, date-specific paths. The provided example does the trick for date-specific paths but still handles .MD path extensions. My changes below take care of both the date-based paths and .MD extension handling.

<pre data-enlighter-language="csharp">
DestinationPath: => $"{Document.GetDateTime("Published").ToString("yyyy/MM")}" + "/" + $"{Document.Destination.FileName}".Replace(".md", "") + ".html"
</pre>

<h3> Rewrite Rules for Hosting on Azure App Services</h3>

To get the Restful Url routing working and mask the HTML file extension required a custom rewrite rule in a web.config file. Surprisingly, I needed to put in a rule and a custom MIME extension for RSS as well. Very App Service / IIS specific but necessary if you're using these platforms.

<pre data-enlighter-language="xml">
<configuration>
  <system.webServer>
    <staticContent>
      <mimeMap fileExtension=".rss" mimeType="application/rss+xml" />
    </staticContent>
    <rewrite>
      <rules>
        <rule name="rss" stopProcessing="true">
          <match url="^feed.rss$" />
          <action type="None" />
        </rule>
        <rule name="html">
          <match url="(.*)" />
          <conditions>
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
          </conditions>
          <action type="Rewrite" url="{R:1}.html" />
        </rule>
      </rules>
    </rewrite>
  </system.webServer>
</configuration>
</pre>

<h3>Google Analytics</h3>

If you're looking to add Google Analytics, or other web tracking products, this is really simple in Statiq. Add the Google Analytics javascrip to the _head.cshtml file, which should be blank, and go to town.

<pre data-enlighter-language="js">
<!-- Google tag (gtag.js) -->
<script async src="https://www.googletagmanager.com/gtag/js?id=G-YOUR-CODE-HERE"></script>
<script>
  window.dataLayer = window.dataLayer || [];
  function gtag(){dataLayer.push(arguments);}
  gtag('js', new Date());

  gtag('config', 'G-YOUR-CODE-HERE');
</script>
</pre>

* Some lessons learned
    * enlighterer in _layout.cshtml
    * _post-footer.cshtml for giscus
    * _sidebar.cshmlt and _social_links.cshtml for social links (could do other like Twitter)
