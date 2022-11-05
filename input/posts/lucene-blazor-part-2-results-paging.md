Title: "Lucene + Blazor, Part 2: Results Paging"
Published: 11/5/2022
Tags:
    - Lucene.NET
    - .NET
---
In the [first installment of this series](https://beckshome.com/2022/10/lucene-blazor-part-1-basic-search), we looked at returning results from a limited pool of items in a Lucene full text index. In this second installment, we significantly increase the number of generated items (3,000, by default) and add a numbered paging system, as used by the main commercial search engines and search sites.

The code and code narrative below reflects the changes that have been made since the first post. All [source code is available online](https://github.com/thbst16/dotnet-lucene-search/tree/main/2-ResultsPaging) for this results paging post.

**Sample App**

The sample application generates 3,000 waffle text records with the exact count being configurable and stored in the appsettings.json file. These waffle items can be searched and return in paginated form with a default page size of 5 records (not configurable). Additional character escaping / nulling has been added to remove characters from searches prior to passing them to the search engine. The site is avaialble online at https://dotnet-lucene-search.azurewebsites.net/

![Results Paging](https://s3.amazonaws.com/s3.beckshome.com/20221104-dotnet-lucene-search-pagination.jpeg)

