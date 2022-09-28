Title: "SQLite and .NET - Agility Tips and Tricks"
Published: 3/30/2010
Tags:
    - .NET
---
For quick and easy prototypes, you’ve got to admire ASP.NET MVC and WCF RIA Services. These approaches may not be perfect out-of-the-box but they’re structured much better than the old “bind a dataset to a grid and let it fly” approach of 2003. As easy as these approaches are, I’m always looking for ways to make things easier. I get a lot of bang for my buck by using SQLite as an in-memory database whenever I create a new MVC or RIA Services solution. In fact, I create 4 SQLite databases with each new solution: one each for application data, test data, membership/role data, and logging/tracing data. Below I’ve described the techniques I make use of to utilize each of these databases.

**System.Data.Sqlite + ORM of Choice**

If you’ve never used SQLite with .NET before, you’ll be happy to know that it’s as easy as can be. The System.Data.SQLite open source ADO.NET provider gives you everything you need. The provider is a complete ADO.NET implementation, including full support for the ADO.NET Entity Framework and Visual Studio design-time support – all in a 900 KB assembly. Need support for VisualStudio 2010? Ion123 includes a library compatible with 2010 in this post. So whether you use Entity Framework or NHibernate, just drop in the System.Data.Sqlite DLL, create a database, wire up your objects to the ORM and go to town. Data access simply could not be easier.

**SQLite-Backed Testing**

There are lots of good reasons to implement proper interfaces and mock objects or stubs for the purposes of testing. Sometimes it’s just easier not to have to deal with it. SQLite-backed testing provides the perfect alternative. You can still create your unit tests, even exercising framework elements and third party libraries that aren’t always the easiest to cover with traditional mocking frameworks. Just plug in a temporary SQLite test database, write your test code just as you’d write your application code and use one of several mechanisms to purge the data between tests. As usual, [Ayende provides the definitive reference](https://ayende.com/blog/1772/unit-testing-with-nhibernate-active-record) on how to do this for NHibernate. I’ve provided code below from my experiences doing this with file backed databases for Castle ActiveRecord. Find your way to Google for references on how to accomplish this with the Entity Framework.

<pre data-enlighter-language="csharp">
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Config;
using Gallio.Framework;
using Gallio.Model;
using MbUnit.Framework;
using MyNameSpace.Models;
using System;

1namespace MyNameSpace.Tests
{
    public abstract class AbstractBaseTest
    {
        protected SessionScope scope;
	 
        [FixtureSetUp]
        public void InitializeAR()
        {
	        ActiveRecordStarter.ResetInitializationFlag();
	        IConfigurationSource source = new XmlConfigurationSource("TestConfig.xml");
	        ActiveRecordStarter.Initialize(source, typeof(Object1), typeof(Object2));
        }

	    [SetUp]
        public virtual void Setup()
        {
            Object2.DeleteAll();
            Object1.DeleteAll();
            scope = new SessionScope();
        }
	 
        [TearDown]
        public virtual void TearDown()
        {
            scope.Dispose();
        }
	 
        public void Flush()
        {
            scope.Flush();
            scope.Dispose();
            scope = new SessionScope();
        }
    }
}
</pre>

**SQLite as a Membership and Role Provider**

Both ASP.NET MVC and WCF RIA Services use SQL Server ASP.NET Membership and Role Providers by default. Take SQL Server out of the equation and swap in the [custom SQLite Membership and Role Providers](http://www.nullskull.com/articles/20051119.asp) and you can use SQLite for your security data as well. Configuration of the custom provider can all be done right in the web.config file, as illustrated below.

<pre data-enlighter-language="xml">
<configuration>
    <connectionStrings>
        <add name="MembershipConnection" connectionString="Data Source=C:ProjectsDatabasesMyApp_Membership.s3db;Version=3;"/>
    </connectionStrings>
    <system.web>
        <authentication mode="Forms">
            <forms loginUrl="~/Account/LogOn"/>
	    </authentication>
	    <membership defaultProvider="SQLiteMembershipProvider" userIsOnlineTimeWindow="15">
	        <providers>
                <clear/>
                <add name="SQLiteMembershipProvider" type="MyNameSpace.Web.Helpers.SqliteMembershipProvider" connectionStringName="MembershipConnection" applicationName="MyApplication" enablePasswordRetrieval="false" enablePasswordReset="true" requiresQuestionAndAnswer="false" requiresUniqueEmail="true" passwordFormat="Hashed" writeExceptionsToEventLog="true"/>
            </providers>
        </membership>
        <roleManager defaultProvider="SQLiteRoleProvider" enabled="true" cacheRolesInCookie="true" cookieName=".ASPROLES" cookieTimeout="30" cookiePath="/" cookieRequireSSL="false" cookieSlidingExpiration="true" cookieProtection="All">
            <providers>
	            <clear/>
	            <add name="SQLiteRoleProvider" type="MyNameSpace.Web.Helpers.SQLiteRoleProvider" connectionStringName="MembershipConnection" applicationName="MyApplication" writeExceptionsToEventLog="true"/>
            </providers>
        </roleManager>
    </system.web>
</configuration>
</pre>
**SQLite Logging and Tracing with NLog**

I [recently covered](/2010/03/logging-to-sqlite-with-nlog) the integration of [NLog](http://nlog-project.org/archives/) with SQLite. A simple configuration file entry and all of your log and trace output can go into a single SQLite database.