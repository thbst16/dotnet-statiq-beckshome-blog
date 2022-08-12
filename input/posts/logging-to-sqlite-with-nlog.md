Title: Logging to SQLite with NLog
Published: 3/13/2010
Tags:
    - .NET Application Architecture
---
This is one of those seemingly trite blog entries – unless you’re actually trying to integrate System.Data.SQLite with [NLog](http://nlog-project.org/documentation.html), in which case it’s invaluable. SQLite and NLog really are the perfect combination for lightweight logging. You avoid the sprawl of file-based logs over time, can execute SQL queries against your logs and have an absolutely minimal database footprint to deal with. If only you can get the configuration correct…

The NLog documentation provides some hints to get you going in the right direction for database-based logging. However, no matter how much spelunking I did around the Net, I couldn’t find a definitive answer on how to configure NLog to use SQLite. The configuration file that worked for me can be found below. Exact mileage may vary based upon your project setup. This should get you 95% of the way there though. Happy logging!

```xml
1	<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
2	  <targets>
3	    <target name="File" xsi:type="File" fileName="C:Logfiles${shortdate}.nlog.txt"/>
4	    <target name="Database" xsi:type="Database" keepConnection="false"
5	            useTransactions="false"
6	            dbProvider="System.Data.SQLite.SQLiteConnection, System.Data.SQLite, Version=1.0.65.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139"
7	            connectionString="Data Source=C:ProjectsDatabasesLogging.s3db;Version=3;"
8	            commandText="INSERT into LOGTABLE(Timestamp, Loglevel, Logger, Callsite, Message) values(@Timestamp, @Loglevel, @Logger, @Callsite, @Message)">
9	      <parameter name="@Timestamp" layout="${longdate}"/>
10	      <parameter name="@Loglevel" layout="${level:uppercase=true}"/>
11	      <parameter name="@Logger" layout="${logger}"/>
12	      <parameter name="@Callsite" layout="${callsite:filename=true}"/>
13	      <parameter name="@Message" layout="${message}"/>
14	    </target>
15	  </targets>
16	  <rules>
17	    <logger name="*" minlevel="Debug" writeTo="Database" />
18	  </rules>
19	</nlog>
```