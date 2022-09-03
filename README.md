# dotnet-statiq-beckshome-blog

![Uptime Robot ratio (7 days)](https://img.shields.io/uptimerobot/ratio/7/m792586859-9634d4aa6352cf540b960a54?logo=http)

The dotnet-statiq-beckshome-blog solution is the fourth version of my blog's (https://blog.beckshome.com) hosting engine. I have moved from the original Das Blog to Wordpress (hosted and then static) to a .NET-based static site generator ([Statiq](https://www.statiq.dev/web)). As part of this port, all of my content was converted to relatively standard markdown format and I kept the external S3 blob storage for images. This standardization of content input will enable me to relatively easily import my blog content to other markdown-based static site generators in the future, if required.

# Solution Highlights

It took a couple of iterations to get all the features right that were needed to replicate the existing Beckshome.com blog functionality. I've included below specific features and functions that are instrumental to the functionality of the blog:

* Statiq web framework - https://www.statiq.dev/web
* Statiq clean blogging theme - https://github.com/statiqdev/CleanBlog
* Giscus for commenting - https://giscus.app
* Azure App Service deployment - https://www.statiq.dev/guide/deployment/azure-app-service
* Setting the post destination path for REST'ful URLs - https://github.com/statiqdev/CleanBlog/blob/main/README.md#post-destination-path

# Motivation and Credits

There were two specific reference sites that were particularly helpful in my Statiq journey:

* [Migrating to Statiq](https://www.techwatching.dev/posts/migrating-blog) - Awesome blog post on getting Statiq set up, using a theme and deploying giscus. The source code for the site is on GitHub and is a very useful datapoint.
* [Continuous Deployment of Statiq to Azure](https://www.developmomentum.com/blog/continuously_deploy_a_static_website_with_azure_pipelines.html) - Although I rolled back my deployment from Azure Blob hosting to Azure Web Apps, this article helped me get the Blog hosting deployment pipeline setup in Azure DevOps. As a caveat, there's some additional work to get REST urls working on static hosting that made this approach not immediately worth it for me.