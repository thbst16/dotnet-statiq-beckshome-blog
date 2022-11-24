Title: "Lucene + Blazor, Part 5: Highlighting"
Published: 11/24/2022
Tags:
    - Lucene.NET
    - .NET
---
In this fourth installment of my Blazor + Lucene.Net series, we'll make the most significant updates to the classes, search function and UI to date. Facets enrich the query responses, enabling users to further tune the search results along specific, pre-defined vectors. The implementation of facets uses the Lucene.Net.Facet library to implement an additional