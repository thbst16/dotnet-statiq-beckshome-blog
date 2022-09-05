Title: Podcast Creation from Rails
Published: 7/8/2007
Tags:
    - Technology Guides
---
I’ve been putting a good deal of time recently into converting GeoGlue from .NET to Rails. One of the things that I’m looking to get into the alpha release is the dynamic creation of podcasts. This is really nothing special since a podcast is little more than a special case of an RSS feed that points at external media files (i.e. audio or video).

![Podcast Creation From Rails](https://s3.amazonaws.com/s3.beckshome.com/20070708-Podcast-Creation-From-Rails.jpg)

I plan on covering the audio/video entry in an upcoming post about the nuances of the Attachment_Fu plugin on Windows. In this post, I’m going to just lay out the code for the podcast creation, since this is nothing more that a simple rxml file. I’ve sprinkled in comments liberally but most of the code should be fairly self explanatory to those familiar with Ruby and RSS feeds.

<pre data-enlighter-language="ruby">
xml.instruct! ::xml, :version=>"1.0", :encoding=>"UTF-8"
xml.rss('version' => '2.0') do
    xml.channel do
        xml.title @podcast.name
        # Self-referencing link
        xml.link url_for(:only_path => false)
        # Important --> RFC-822 compliant datetime
        xml.pubDate(Time.now.strftime("%a, %d %b %Y %H:%M:%S %Z"))
        xml.language "en-us";
        xml.ttl "40"
        # User who caused the feed to be generated
        xml.generator User.find(:first, session[:user_id]).name
        xml.description @podcast.description
        # 'public_filename' is a method from the Attachment_Fu plugin
        xml.image do
            xml.url url_for(:controller => @podcast.images[0].public_filename, ::only_path => false)
            xml.link url_for(:only_path => false)
            xml.title @podcast.name
            xml.width @podcast.images[0].width
            xml.height @podcast.images[0].height
        end
        @podcast.entries.each do |entry|
            xml.item do
                xml.title(entry.title)
                xml.link(url_for(:controller =>; entry.audios[0].public_filename, ::only_path => false))
                # User who actually generated the media (i.e. audio)
                xml.author(entry.user.name)
                xml.category "Uncategorized"
                xml.guid(url_for(:controller => entry.audios[0].public_filename, ::only_path => false))
                xml.description(entry.description)
                # Simplification, you should pull from updated_at/updated_on
                xml.pubDate(Time.now.strftime("%a, %d %b %Y %H:%M:%S %Z"))
                # The enclosure is very important!!
                # If you use Attachment_Fu, everything you need is included in the model
                xml.enclosure(:type=>entry.audios[0].content_type,
                :length=>entry.audios[0].size.to_s,
                :url=>url_for(:controller => entry.audios[0].public_filename, ::only_path => false)
            )
            end
        end
    end
end
</pre>
A couple of lessons learned from my experience. Firstly, Apple provides some good resources on generating podcasts. This is especially important since the iTunes crowd is a large and important contingent of the feed consuming world. There are iTunes-specific tags (and a schema) available. These tags are not mandatory (I didn’t use them here) but they will help you produce a richer feed for consumption within iTunes. Secondly, since the RXML file is just another view, make sure to turn off any default layouts that you might have applied to your other views. I’ve included a snippet below to demonstrate how to do this. Check your version of Rails, mileage may vary with exempt_from_layout based upon your release.

<pre data-enlighter-language="ruby">
class ApplicationController < ActionController::Base  
	 
    # Pick a unique cookie name to distinguish our session data from others
    session :session_key => '_trunk_session_id'
    layout 'default'
    exempt_from_layout :rxml
    ...
 
end
</pre>
My final caveat is not to apply forms-based authentication to your podcast (RXML view). Either make the view public or, if you wish to protect it, do so using HTTP Basic authentication instead. If you’re using both forms-based and HTTP Basic authentication, you’ll probably need to sync the two by using a single LDAP repository. That’s fodder for a completely different post.