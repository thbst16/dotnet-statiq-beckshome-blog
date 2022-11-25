Title: "Lucene + Blazor, Part 5: Highlighting"
Published: 11/25/2022
Tags:
    - Lucene.NET
    - .NET
---
In this final installment of my Blazor + Lucene.Net series, we'll be adding highlights for the search terms found in the header and body text of each of our results. The implementation of highlighting makes use of the Lucene.Net.Highlighter library, plugging this library into a simple method that can be used as a filter for search results to highlight key terms. 

The code and code narrative below reflects the changes that have been made on top of the first 4 posts. All [source code is available online](https://github.com/thbst16/dotnet-lucene-search/tree/main/5-Highlighting) for this results paging post.