Title: Podcast Creation from Rails
Published: 7/8/2007
Tags:
    - Technology Guides
---
I’ve been putting a good deal of time recently into converting GeoGlue from .NET to Rails. One of the things that I’m looking to get into the alpha release is the dynamic creation of podcasts. This is really nothing special since a podcast is little more than a special case of an RSS feed that points at external media files (i.e. audio or video).

![Podcast Creation From Rails](https://s3.amazonaws.com/s3.beckshome.com/20070708-Podcast-Creation-From-Rails.jpg)

I plan on covering the audio/video entry in an upcoming post about the nuances of the Attachment_Fu plugin on Windows. In this post, I’m going to just lay out the code for the podcast creation, since this is nothing more that a simple rxml file. I’ve sprinkled in comments liberally but most of the code should be fairly self explanatory to those familiar with Ruby and RSS feeds.

```ruby
1	xml.instruct! ::xml, :version=>"1.0", :encoding=>"UTF-8"
2	xml.rss('version' => '2.0') do
3	xml.channel do
4	xml.title @podcast.name
5	# Self-referencing link
6	xml.link url_for(:only_path => false)
7	# Important --&gt; RFC-822 compliant datetime
8	xml.pubDate(Time.now.strftime("%a, %d %b %Y %H:%M:%S %Z"))
9	xml.language "en-us";
10	xml.ttl "40"
11	# User who caused the feed to be generated
12	xml.generator User.find(:first, session[:user_id]).name
13	xml.description @podcast.description
14	# 'public_filename' is a method from the Attachment_Fu plugin
15	xml.image do
16	xml.url url_for(:controller => @podcast.images[0].public_filename, ::only_path => false)
17	xml.link url_for(:only_path => false)
18	xml.title @podcast.name
19	xml.width @podcast.images[0].width
20	xml.height @podcast.images[0].height
21	end
22	@podcast.entries.each do |entry|
23	xml.item do
24	xml.title(entry.title)
25	xml.link(url_for(:controller =>; entry.audios[0].public_filename, ::only_path => false))
26	# User who actually generated the media (i.e. audio)
27	xml.author(entry.user.name)
28	xml.category "Uncategorized"
29	xml.guid(url_for(:controller => entry.audios[0].public_filename, ::only_path => false))
30	xml.description(entry.description)
31	# Simplification, you should pull from updated_at/updated_on
32	xml.pubDate(Time.now.strftime("%a, %d %b %Y %H:%M:%S %Z"))
33	# The enclosure is very important!!
34	# If you use Attachment_Fu, everything you need is included in the model
35	xml.enclosure(:type=>entry.audios[0].content_type,
36	:length=>entry.audios[0].size.to_s,
37	:url=>url_for(:controller => entry.audios[0].public_filename, ::only_path => false)
38	)
39	end
40	end
41	end
42	end
```
A couple of lessons learned from my experience. Firstly, Apple provides some good resources on generating podcasts. This is especially important since the iTunes crowd is a large and important contingent of the feed consuming world. There are iTunes-specific tags (and a schema) available. These tags are not mandatory (I didn’t use them here) but they will help you produce a richer feed for consumption within iTunes. Secondly, since the RXML file is just another view, make sure to turn off any default layouts that you might have applied to your other views. I’ve included a snippet below to demonstrate how to do this. Check your version of Rails, mileage may vary with exempt_from_layout based upon your release.

```ruby
1	class ApplicationController < ActionController::Base  
2	 
3	# Pick a unique cookie name to distinguish our session data from others
4	session :session_key => '_trunk_session_id'
5	layout 'default'
6	exempt_from_layout :rxml
7	...
8	 
9	end
```
My final caveat is not to apply forms-based authentication to your podcast (RXML view). Either make the view public or, if you wish to protect it, do so using HTTP Basic authentication instead. If you’re using both forms-based and HTTP Basic authentication, you’ll probably need to sync the two by using a single LDAP repository. That’s fodder for a completely different post.