Title: Netbeans 6.0 as a Rails IDE
Published: 10/7/2007
Tags:
    - New Technology
---
I’ve posted about how impressed I was with NetBeans as a Java IDE and the incredible progress this product has made in the last couple of years. I knew for a while that Ruby on Rails and JRuby support was coming for the next major Netbeans release (v 6.0), but I hesitated moving from RadRails to NetBeans until the feature set had stabilized. Last week, the Netbeans 6.0 beta was released and, with RadRails stagnating somewhat under the Aptana brand, I caved in and made the switch.

George Cook does an excellent Job of running through the new features with lots of nice pretty screenshots. If you’re looking at moving to Netbeans as a Rails IDE, it’s the first place I suggest that you go. Some of my favorite features of Netbeans (with screens shamelessly stolen from George’s site) include code completion

![Netbeans 6.0 Code Completion](https://s3.amazonaws.com/s3.beckshome.com/20071001-Netbeans-6-0-Code-Completion.png)

…and debugging

![Netbeans 6.0 Debugging](https://s3.amazonaws.com/s3.beckshome.com/20071001-Netbeans-6-0-Debugging.png)

There are several features from RadRails that I miss and that I hope the NetBeans team will consider integrating over time. These include the ability to import a project directly from Subversion and the test window that allows you to visually check the status of your tests and select particular tests to run. Those features aside, I don’t plan on going back to RadRails. NetBeans has made so much progress so quickly, I can only imagine that it’s going to put significant distance between itself and RadRails in the near future.

You can [get Netbeans 6.0 here(https://netbeans.apache.org/), available as a skinnied-down Ruby only version if you want. Finally, since Netbeans uses JRuby as the default interpreter and expects the Derby Java database, this article on wiring NetBeans for InstantRails should get you up and moving with the standard Ruby interpreter and MySQL database configuration, regardless of whether you’re using InstantRails or not.

Final note if you’re brand new to Ruby on Rails and reading this post. Skip right to [Rails 2.0, which is now in preview mode](https://rubyonrails.org/2007/9/30/rails-2-0-0-preview-release), to avoid dealing with Rails 1.2.x deprecations and to benefit from some of the new defaults. Enjoy!