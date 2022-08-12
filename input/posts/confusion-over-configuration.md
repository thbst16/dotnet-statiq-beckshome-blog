Title: Confusion over Configuration
Published: 12/16/2006
Tags:
    - Legacy Blog
---
My initial experiences with Oracle’s TopLink object-relational mapping tool have been less than pleasant. TopLink is the default Java Persistence API provider when creating persistence units for EJB3 style beans in NetBeans. When using TopLink out of the box with NetBeans, the tool’s default behavior is to behave in a case-sensitive fashion with respect to table and column names. This results in awful “Table XXXXX does not exist” errors, where XXXXX is, of course, the capitalized table name.

![Confusion over Configuration](http://s3.beckshome.com/20061216-Confusion-Over-Configuration.gif)

This posture represents the absolute antithesis to the recent, Ruby on Rails-driven trend towards “Convention over Configuration”; instead reverting to the longstanding software engineering tradition of confusion over configuration. Why wouldn’t you just set the default behavior to respect case insensitivity? I’m assuming that this can be set in a config file but I’m too mad to go thumbing through the documentation to find out where that is. The TopLink API has a setShouldForceFieldsToUpperCase() method. I don’t want to call that either. Why should I have to?

Hmm… if I remember correctly, were I to open a SQL*Plus command prompt and execute the statement CREATE TABLE foo, Oracle would create for me a table named FOO. Sounds like Oracle convention over ease of configuration. As far as I can see, EJB-QL is case insensitive and the database I’m using, Derby, is also case insensitive. My primitive troubleshooting points to TopLink as the culprit here. I’m going to swap in Hibernate and see if I’m right or wrong. Is anyone else experiencing similar problems?