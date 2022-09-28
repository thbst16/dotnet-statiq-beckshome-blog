Title: Logging to SQLite with NLog
Published: 3/13/2010
Tags:
    - .NET
---
This is one of those seemingly trite blog entries – unless you’re actually trying to integrate System.Data.SQLite with [NLog](https://github.com/nlog/nlog/wiki), in which case it’s invaluable. SQLite and NLog really are the perfect combination for lightweight logging. You avoid the sprawl of file-based logs over time, can execute SQL queries against your logs and have an absolutely minimal database footprint to deal with. If only you can get the configuration correct…

The NLog documentation provides some hints to get you going in the right direction for database-based logging. However, no matter how much spelunking I did around the Net, I couldn’t find a definitive answer on how to configure NLog to use SQLite. The configuration file that worked for me can be found below. Exact mileage may vary based upon your project setup. This should get you 95% of the way there though. Happy logging!

<pre data-enlighter-language="xml">
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <targets>
        <target name="File" xsi:type="File" fileName="C:Logfiles${shortdate}.nlog.txt"/>
        <target name="Database" xsi:type="Database" keepConnection="false" 
            useTransactions="false"
            dbProvider="System.Data.SQLite.SQLiteConnection, System.Data.SQLite, Version=1.0.65.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139"
            connectionString="Data Source=C:ProjectsDatabasesLogging.s3db;Version=3;"
            commandText="INSERT into LOGTABLE(Timestamp, Loglevel, Logger, Callsite, Message) values(@Timestamp, @Loglevel, @Logger, @Callsite, @Message)">
            <parameter name="@Timestamp" layout="${longdate}"/>
            <parameter name="@Loglevel" layout="${level:uppercase=true}"/>
            <parameter name="@Logger" layout="${logger}"/>
            <parameter name="@Callsite" layout="${callsite:filename=true}"/>
            <parameter name="@Message" layout="${message}"/>
        </target>
    </targets>
    <rules>
        <logger name="*" minlevel="Debug" writeTo="Database" />
    </rules>
</nlog>
</pre>