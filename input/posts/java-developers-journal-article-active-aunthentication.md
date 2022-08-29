Title: "Java Developers Journal Article - Active Authentication"
Published: 7/2/2006
Tags:
    - Legacy Blog
---
Since I originally published my article on active authentication in the Java Developer’s Journal a couple of years back, I’ve been receiving a trickle of requests for the source code. It looks like the article is still available online although the accompanying source code seemed to have disappeared. I rummaged through my archives and dug up the WAR file containing the source code in case you’re interested. I can’t vouch for its absolute correctness. I seem to recall recreating the source code for a guy in Switzerland a couple of years back to run on Tomcat 5. I’m not sure if this is the version I’ve posted.

[ActiveAuthentication.war (6.11 KB)](https://s3.amazonaws.com/s3.beckshome.com/20060702-ActiveAuthentication.zip)

![JDJ Article - Active Authentication](https://s3.amazonaws.com/s3.beckshome.com/20060702-JDJ-Active-Authentication.png)

Also, since I’ve been less involved with Java over the past 3 or 4 years, I’m not sure that this solution is even necessary or applicable anymore. Even when the article was written, there were various degrees of vendor support for active authentication through HTTP filters across the various platforms. I most vividly remember the differences between Tomcat and Websphere. Give the code a whirl and let me know if it works or needs a bit of improvement.