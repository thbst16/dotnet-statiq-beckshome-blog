Title: "Lucene + Blazor, Part 3: Auto Complete"
Published: 11/12/2022
Tags:
    - Lucene.NET
    - .NET
---
In this third installment of my Blazor + Lucene.Net series, I'll start tackling some advanced Lucene functionality, namely auto-complete. For advanced Lucene work, the most important lessons is <b>don't roll your own</b> functionality. If you go to the [docs for the Lucene.Net API](https://lucenenet.apache.org/docs/4.8.0-beta00007/api/Lucene.Net/overview.html), you'll see that a ton of additional functionality is built into Lucene via modules. Modules exist for faceting, highlighting, spatial search and autosuggest, amongst others. Lots of the examples and StackOverflow answers are roll-your-own solutions -- don't do it!!

Specific to this installment around Auto Complete, I employed two specific libraries:

1) [Lucene.Net Suggest](https://lucenenet.apache.org/docs/4.8.0-beta00007/api/Lucene.Net.Suggest/overview.html) - The auto complete / auto suggest library includes the methods to index the data for autosuggest and then a number of suggester algorithms to query the index.
2) [Blazored.Typeahead](https://github.com/Blazored/Typeahead) - A drop-in Blazor control for type-ahead that accomodates things like debouncing time before executing searches.

**Sample App**

The sample application let's you search over 3,000 waffle text entries, returning paginated search results. Auto-complete functionality provides suggestion for the most relavent search terms in the waffle text index.

![Auto Complete](https://s3.amazonaws.com/s3.beckshome.com/20221111-dotnet-lucene-auto-complete.jpeg)