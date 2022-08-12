Title: Simple_Captcha on Windows
Published: 10/7/2007
Tags:
    - Legacy Blog
---
Another in the installment of Rails on Windows “gotchas”, there are some things to be wary of when working with the Simple_Captcha plugin in the Windows environment. In terms of basic background, the Simple_Captcha plugin facilitates the integration of [CAPTCHA](https://en.wikipedia.org/wiki/CAPTCHA) (Computer Automated Public Turing test to tell Computers and Humans Apart) image recognition tests, like the example below, into a Rails application. Facilitates is perhaps not a strong enough term. The plugin makes CAPTCHA integration dirt simple.

![Simple_Captcha On Windows](http://s3.beckshome.com/20071007-Simple-Captcha.PNG)

The Simple_Captcha plugin uses RMagick for generation of the CAPTCHA recognition images, allowing for various image styles and distortion levels. The CAPTCHA can be integrated via the controller (this one is dirt simple) or via the model (this one is just silly simple). You can find out more about these and various other integration options on the plugin’s page.

If you’re doing Rails development on the Windows platform and are not feeling especially masochistic, the rmagick-win32 gem, which is bundled with a copy of the ImageMagick Windows installer, is really the only way to use RMagick. For a long while, the 1.13.0 rmagick-win32 gem was the standard. However, this gem is likely to cause you issues and you should really upgrade your gem to the 1.14.1 gem or greater. These gems are fixed to work with RubyGems 0.9.4, which is the most recent version of this gem as of this blog post. If you don’t perform this update, you’re likely to see ImageMagick issues bubble up at runtime.

On Windows, these runtime errors frequently manifest themselves as ‘cur_image’ issues. Several of these issues have been reported on the plugin’s page. My post on 10/6 covered fixing these issues by upgrading your RMagick gem. Please don’t downgrade other gems, as suggested in some other posts; this will only make your life more miserable in the future.

All-in-all, the RMagick Windows gem is an excellent way to make powerful image processing capabilities available to all, including those unfortunate enough to be stuck on a Rails on Windows development platform. The plugins built on top of RMagick such as Simple_Captcha and Attachment_Fu are incredibly powerful and remain very simple by leveraging RMagick’s capabilities. Just beware if you’re developing on Windows, a little bit of tweaking and debugging may be necessary to get these plugins to work as advertised.

