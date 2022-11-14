Title: "Lucene + Blazor, Part 3: Auto Complete"
Published: 11/12/2022
Tags:
    - Lucene.NET
    - .NET
---
In this third installment of my Blazor + Lucene.Net series, I'll start tackling some advanced Lucene functionality, namely auto-complete. For advanced Lucene work, the most important lessons is <b>don't roll your own</b> functionality. If you go to the [docs for the Lucene.Net API](https://lucenenet.apache.org/docs/4.8.0-beta00007/api/Lucene.Net/overview.html), you'll see that a ton of additional functionality is built into Lucene via modules. Modules exist for faceting, highlighting, spatial search and autosuggest, amongst others. Lots of the examples and StackOverflow answers are roll-your-own solutions -- don't do it!!

Specific to this installment around Auto Complete, I employed two specific libraries:

1) [Lucene.Net Suggest](https://lucenenet.apache.org/docs/4.8.0-beta00007/api/Lucene.Net.Suggest/overview.html) - The auto complete / auto suggest library includes the methods to index the data for autosuggest and then a number of suggester algorithms to query the index.
2) [Blazored.Typeahead](https://github.com/Blazored/Typeahead) - A drop-in Blazor control for type-ahead that accommodates things like debouncing time before executing searches.

The code and code narrative below reflects the changes that have been made on top of posts 1 and 2. All [source code is available online](https://github.com/thbst16/dotnet-lucene-search/tree/main/3-AutoComplete) for this auto-complete post.

**Sample App**

The sample application let's you search over 3,000 waffle text entries, returning paginated search results. Auto-complete functionality provides suggestion for the most relevant search terms in the waffle text index. The site is available online at https://dotnet-lucene-search.azurewebsites.net/

![Auto Complete](https://s3.amazonaws.com/s3.beckshome.com/20221111-dotnet-lucene-auto-complete.jpeg)

**Auto Complete Function**

The heart of the search ahead function is included in a new method (SearchAhead) in the SearchEngine.cs file. The function creates a dictionary of terms on top of the search index and then searches that for the input text, returning an ordered set of results of words starting with the typed letters. To get this to work, I had to add a new field to the index (HeadBody) because there doesn't seem to be a way to apply a MultiFieldQueryParser over the LuceneDictionary of terms. 

<pre data-enlighter-language="csharp">
using Bogus;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Search.Spell;
using Lucene.Net.Search.Suggest.Analyzing;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System.Text.RegularExpressions;

namespace search.Shared
{
    public class SearchEngine{
        public static List<WaffleText> Data {get; set;}
        private static RAMDirectory _directory;
        public static IndexWriter Writer { get; set; }

        public static void GetData(int Rand, int WaffleCount)
        {
            Randomizer.Seed = new Random(Rand);
            var testWaffles = new Faker<WaffleText>()
                .RuleFor(wt => wt.GUID, f => Guid.NewGuid().ToString())
                .RuleFor(
                    property: wt => wt.WaffleHead,
                    setter: (f, wt) => f.WaffleTitle())
                .RuleFor(
                    property: wt => wt.WaffleBody,
                    setter: (f, wt) => f.WaffleText(
                        paragraphs: 2,
                        includeHeading: false));
            var waffles = testWaffles.Generate(WaffleCount);
            
            Data = new List<WaffleText>();
            foreach(WaffleText wt in waffles)
            {
                Data.Add(wt);
            }
        }

        public static void Index()
        {
            const LuceneVersion lv = LuceneVersion.LUCENE_48;
            Analyzer a = new StandardAnalyzer(lv);
            _directory = new RAMDirectory();
            var config = new IndexWriterConfig(lv, a);
            Writer = new IndexWriter(_directory, config);

            var guidField = new StringField("GUID", "", Field.Store.YES);
            var headField = new TextField("WaffleHead", "", Field.Store.YES);
            var bodyField = new TextField("WaffleBody", "", Field.Store.YES);
            // Hoping for better way to do this than indexing combined field
            var headBody = new TextField("HeadBody", "", Field.Store.YES);

            var d = new Document()
            {
                guidField,
                headField,
                bodyField,
                headBody
            };

            foreach (WaffleText wt in Data)
            {
                guidField.SetStringValue(wt.GUID);
                headField.SetStringValue(wt.WaffleHead);
                bodyField.SetStringValue(wt.WaffleBody);
                headBody.SetStringValue(wt.WaffleHead + " " + wt.WaffleBody);
                Writer.AddDocument(d);
            }
            Writer.Commit();
        }

        public static void Dispose()
        {
            Writer.Dispose();
            _directory.Dispose();
        }

        public static SearchModel Search(string input, int page)
        {
            const LuceneVersion lv = LuceneVersion.LUCENE_48;
            Analyzer a = new StandardAnalyzer(lv);
            var dirReader = DirectoryReader.Open(_directory);
            var searcher = new IndexSearcher(dirReader);

            string[] waffles = { "GUID", "WaffleHead", "WaffleBody" };
            var multiFieldQP = new MultiFieldQueryParser(lv, waffles, a);
            string _input = EscapeSearchTerm(input.Trim());
            Query query = multiFieldQP.Parse(_input);

            ScoreDoc[] docs = searcher.Search(query, null, 1000).ScoreDocs;

            var returnModel = new SearchModel();
            returnModel.CurrentPageSearchResults = new List<WaffleText>();
            returnModel.SearchText = _input;
            returnModel.ResultsCount = docs.Length;
            returnModel.PageCount = (int)Math.Ceiling(docs.Length/5.0);
            returnModel.CurrentPage = page;

            int first = (page-1)*5;
            int last = first + 5;
            for (int i = first; i < last && i < docs.Length; i++)
            {
                Document d = searcher.Doc(docs[i].Doc);
                WaffleText _localWaffle = new WaffleText();
                _localWaffle.GUID = d.Get("GUID");
                _localWaffle.WaffleHead = d.Get("WaffleHead");
                _localWaffle.WaffleBody = d.Get("WaffleBody");
                returnModel.CurrentPageSearchResults.Add(_localWaffle);
            }
            dirReader.Dispose();
            return returnModel;
        }

        public static List<string> SearchAhead(string input)
        {
            const LuceneVersion lv = LuceneVersion.LUCENE_48;
            Analyzer a = new StandardAnalyzer(lv);
            var dirReader = DirectoryReader.Open(_directory);

            LuceneDictionary dictionary = new LuceneDictionary(dirReader, "HeadBody");

            RAMDirectory _d = new RAMDirectory();
            AnalyzingInfixSuggester analyzingSuggester = new AnalyzingInfixSuggester(lv, _d, a);
            analyzingSuggester.Build(dictionary);

            var lookupResultList = analyzingSuggester.DoLookup(input.Trim(), false, 9);

            List<string> returnModel = new List<string>();
            foreach(var result in lookupResultList)
            {
                returnModel.Add(result.Key);
            }

            return returnModel;
            dirReader.Dispose();
        }

        // Lucene supports escapting the following chars: + - && || ! ( ) { } [ ] ^ " ~ * ? : \
        // To make it easier, I remove / replace the text altogether
        // Added bold html tag replacement for type ahead
        private static string EscapeSearchTerm(string input)
        {
            input = Regex.Replace(input, @"<b>", "");
            input = Regex.Replace(input, @"</b>", "");
            input = Regex.Replace(input, @"\+", " ");
            input = Regex.Replace(input, @"\-", " ");
            input = Regex.Replace(input, @"\&", " ");
            input = Regex.Replace(input, @"\|", " ");
            input = Regex.Replace(input, @"\!", " ");
            input = Regex.Replace(input, @"\(", " ");
            input = Regex.Replace(input, @"\)", " ");
            input = Regex.Replace(input, @"\{", " ");
            input = Regex.Replace(input, @"\}", " ");
            input = Regex.Replace(input, @"\[", " ");
            input = Regex.Replace(input, @"\]", " ");
            input = Regex.Replace(input, @"\^", " ");
            input = Regex.Replace(input, @"\""", " ");
            input = Regex.Replace(input, @"\~", " ");
            input = Regex.Replace(input, @"\*", " ");
            input = Regex.Replace(input, @"\?", " ");
            input = Regex.Replace(input, @"\:", " ");
            input = Regex.Replace(input, @"\\", " ");
            return input;
        }
    }
}
</pre>

**Auto Complete User Interface**

The auto complete user interface is the other place where meaningful changes were required to accommodate auto-complete functionality. These changes include the addition of the Blazored.TypeAhead control to the page and the new HandleTypeAhead method that invokes the search function.

<pre data-enlighter-language="csharp">
@page "/"

<PageTitle>Prose Search</PageTitle>

<BSRow>
    <BSCol Column="12" Padding="Padding.Small">    
    </BSCol>
</BSRow>

<EditForm Model="@searchModel" OnValidSubmit="@HandleSearch">
    <DataAnnotationsValidator />
    <BSRow Padding="Padding.None">
        <BSCol Column="4" PaddingStart="Padding.Small">
            <BlazoredTypeahead SearchMethod="HandleTypeAhead"
                @bind-Value="searchModel.SearchText"
                Placeholder="Enter Prose Text...">
                <SelectedTemplate Context="searchText">
                    @((MarkupString)@searchText)
                </SelectedTemplate>
                <ResultTemplate Context="searchText">
                    @((MarkupString)@searchText)
                </ResultTemplate>
            </BlazoredTypeahead>
        </BSCol>
        <BSCol Column="1" Padding="Padding.None">
            <BSButton type="Submit" Color="BSColor.Primary" PaddingTopAndBottom="Padding.Small">Search</BSButton>
        </BSCol>
    </BSRow>
</EditForm>

@if(@SearchText!=String.Empty)
{
    <BSRow>
        <BSCol Column="12" PaddingTop="Padding.Medium">
            <div class="mb-12">
                @if(@SearchResultsCount==1)
                {
                    <div><b>@SearchResultsCount Result</b></div>
                }
                else
                {
                    <div><b>@SearchResultsCount Results</b></div>
                }
            </div>
        </BSCol>
    </BSRow>
}

@if(@SearchResultsCount>0)
{
    <BSRow>
        <BSCol Column="12" Padding="Padding.Small">    
        </BSCol>
    </BSRow>

    <BSRow>
        <BSCol Column="9">
            <div class="mb-9">
                <BSListGroup>
                    @foreach (var result in @searchModel.CurrentPageSearchResults)
                    {
                        <BSListGroupItem>
                            <div class="d-flex w-100 justify-content-between">
                                <h5 class="mb-1">@result.WaffleHead</h5>
                            </div>
                            <p class="mb-1">@result.WaffleBody</p>
                        </BSListGroupItem>
                    }
                </BSListGroup>
            </div>
        </BSCol>
    </BSRow>
    @if(@PageCount>1)
    {
        <BSRow>
            <BSCol Column="12" Padding="Padding.Small">  
            </BSCol>
        </BSRow>

        <BSRow @onclick="UpdatePage">
            <BSCol Column="9">
                <div class="mb-9">
                    <BSPagination Pages=@PageCount @bind-Value="Page"/>
                </div>
            </BSCol>
        </BSRow>
    }
        
    <BSRow>
        <BSCol Column="12" Padding="Padding.Large">    
        </BSCol>
    </BSRow>
}

@code {
    private SearchModel searchModel = new SearchModel();
    
    [Parameter]
    public int Page {get; set;} = 1;
    [Parameter]
    public int PageCount {get; set;} = 0;
    [Parameter]
    public string SearchText {get; set;} = string.Empty; 
    [Parameter]
    public int SearchResultsCount {get; set;} = 0;

    private void HandleSearch()
    {
        searchModel = SearchEngine.Search(searchModel.SearchText, 1);
        SearchResultsCount = searchModel.ResultsCount;
        PageCount = searchModel.PageCount;
        SearchText = searchModel.SearchText;
        Page = 1;
    }

    private async Task<IEnumerable<String>> HandleTypeAhead(string searchText)
    {
        List<String> SResult = SearchEngine.SearchAhead(searchText);
        IEnumerable<String> AResult = new List<String>();
        if (!SResult.Contains(searchText))
        {
            AResult = SResult.Prepend("<b>"+searchText+"</b>");
        }
        else
        {
            AResult = SResult;
        }
        return await Task.FromResult(AResult.Where(x => x.ToLower().Contains(searchText.ToLower())).ToList());
    }

    private void UpdatePage()
    {
        searchModel = SearchEngine.Search(searchModel.SearchText, Page);
    }
}
</pre>

**Type Ahead Control - Front End**

The Blazored.Typeahead control needs to be added to the _Layout.cshtml file, brining along the requisite JS and CSS files. 

<pre data-enlighter-language="html">
@using Microsoft.AspNetCore.Components.Web
@namespace search.Pages
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="~/" />
    <link rel="stylesheet" href="css/bootstrap/bootstrap.min.css" />
    <link href="css/site.css" rel="stylesheet" />
    <link href="search.styles.css" rel="stylesheet" />
    <link href="search.styles.css" rel="stylesheet">
    <link href="_content/Blazored.Typeahead/blazored-typeahead.css" rel="stylesheet" />
    <component type="typeof(HeadOutlet)" render-mode="ServerPrerendered" />
</head>
<body>
    @RenderBody()

    <div id="blazor-error-ui">
        <environment include="Staging,Production">
            An error has occurred. This application may no longer respond until reloaded.
        </environment>
        <environment include="Development">
            An unhandled exception has occurred. See browser dev tools for details.
        </environment>
        <a href="" class="reload">Reload</a>
        <a class="dismiss">🗙</a>
    </div>

    <script src="_content/BlazorStrap/popper.min.js"></script>
    <script src="_content/BlazorStrap/blazorstrap.js"></script>
    <script src="_content/Blazored.Typeahead/blazored-typeahead.js"></script>
    <script src="_framework/blazor.server.js"></script>
</body>
</pre>

And the BlazorStrap library using statement needs to be added to the _Imports.razor file.

<pre data-enlighter-language="csharp">
@using System.Net.Http
@using Blazored.Typeahead
@using BlazorStrap
@using Microsoft.AspNetCore.Authorization
@using Microsoft.AspNetCore.Components.Authorization
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.JSInterop
@using search
@using search.Shared
</pre>

**Type Ahead - Enablement**

Finally, the Blazored.Typeahead and Lucene.Net.Suggest libraries are added to the project in the .csproj file.

<pre data-enlighter-language="xml">
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Blazored.Typeahead" Version="4.7.0" />
    <PackageReference Include="BlazorStrap" Version="5.0.106" />
    <PackageReference Include="Bogus" Version="34.0.2" />
    <PackageReference Include="Lucene.Net" Version="4.8.0-beta00016" />
    <PackageReference Include="Lucene.Net.Analysis.Common" Version="4.8.0-beta00016" />
    <PackageReference Include="Lucene.Net.QueryParser" Version="4.8.0-beta00016" />
    <PackageReference Include="Lucene.Net.Suggest" Version="4.8.0-beta00016" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="6.0.1" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="6.0.0" />
    <PackageReference Include="WaffleGenerator.Bogus" Version="4.2.1" />
  </ItemGroup>

</Project>

</pre>