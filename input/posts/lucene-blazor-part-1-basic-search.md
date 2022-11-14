Title: "Lucene + Blazor, Part 1: Basic Search"
Published: 10/30/2022
Tags:
    - Lucene.NET
    - .NET
---
Lucene.NET is a C# port of the Java Lucene search engine library. Lucene.NET provides robust search and index capabilities enhanced by a wide array of support packages (e.g. auto-suggest, faceting) that enable the creation of robust search facilities within .NET applications. 

While [useful and recent tutorials](https://code-maze.com/how-to-implement-lucene-dotnet/) exist on using Lucene.NET in a command line context, there is a dearth of tutorials on using Lucene.NET in a web context, especially with the current Blazor framework. This tutorial fills that gap, with this existing article being the first in a series that will illustrate the buildout of a Blazor-based search site. 

**Sample App**

The sample application generates sample waffle text, indexes this text and provides a web search front end. The image below illustrates the basic user interface. This site is available online at https://dotnet-lucene-search.azurewebsites.net/ with the source code for this tutorial at https://github.com/thbst16/dotnet-lucene-search/tree/main/1-BasicSearch. 

![BlazorCrud Home Page](https://s3.amazonaws.com/s3.beckshome.com/20221029-dotnet-lucene-search-basic.jpeg)

**Required Libraries**

Several NuGet packages are required to run the application, as illustrated in the search.csproj file below. These packages include the latest pre-releases of the Lucene.Net framework and supporting packages for the search engine, BlazorStrap for Blazor-based UI components and the Bogus data generation library. 

<pre data-enlighter-language="xml">
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="BlazorStrap" Version="5.0.106" />
    <PackageReference Include="Bogus" Version="34.0.2" />
    <PackageReference Include="Lucene.Net" Version="4.8.0-beta00016" />
    <PackageReference Include="Lucene.Net.Analysis.Common" Version="4.8.0-beta00016" />
    <PackageReference Include="Lucene.Net.QueryParser" Version="4.8.0-beta00016" />
    <PackageReference Include="WaffleGenerator.Bogus" Version="4.2.1" />
  </ItemGroup>

</Project>
</pre> 

**Waffle Text**

The search engine app will be generating and searching on [waffle text](https://www.red-gate.com/simple-talk/development/dotnet-development/the-waffle-generator/). Support for waffle text generation is provided by an ancillary Bogus library. The definition of the waffle text class is provided in WaffleText.cs.

<pre data-enlighter-language="csharp">
namespace search.Shared
{
    public class WaffleText
    {
        public string? GUID { get; set; }
        public string? WaffleHead { get; set; }
        public string? WaffleBody { get; set; }

        public WaffleText() {}
        public WaffleText(string _guid, string _waffleHead, string _waffleBody)
        {
            GUID = _guid;
            WaffleHead = _waffleHead;
            WaffleBody = _waffleBody;
        }
    }
}
</pre>

**Search Model**

The model for handling the search inputs, results count and collection of waffles is defined in the SearchModel.cs class.

<pre data-enlighter-language="csharp">
using System.ComponentModel.DataAnnotations;

namespace search.Shared
{
    public class SearchModel{
        [Required]
        public string SearchText {get; set;}
        public int ResultsCount {get; set;}
        public List<WaffleText> SearchResults {get; set;}
    }
}
</pre>

**Search Engine**

The magic of the app is handled in the SearchEngine.cs class. This class interacts with the Lucene.NET search engine library and the Bogus library to facilitate the generation, indexing and search of waffle data. The SearchEngine.cs class has 3 major methods:

1. <b>GetData.</b> Uses the Bogus data generation library to generate 50 pseudo-random waffle text items.
2. <b>Index.</b> Uses the Lucene.NET search engine library to index the generated waffle text items for search.
3. <b>Search.</b> Provides the search function that searches over the indexed waffle text using a scored search.

Both the GetData and Index methods are called during program startup (from the Program.cs file). The Search method is invoked from the Blazor UI with the search text passed in from the user's input.

<pre data-enlighter-language="csharp">
using Bogus;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace search.Shared
{
    public class SearchEngine{
        public static List<WaffleText> Data {get; set;}
        private static RAMDirectory _directory;
        public static IndexWriter Writer { get; set; }

        public static void GetData()
        {
            Randomizer.Seed = new Random(11784);
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
            var waffles = testWaffles.Generate(50);
            
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

            var d = new Document()
            {
                guidField,
                headField,
                bodyField
            };

            foreach (WaffleText wt in Data)
            {
                guidField.SetStringValue(wt.GUID);
                headField.SetStringValue(wt.WaffleHead);
                bodyField.SetStringValue(wt.WaffleBody);
                Writer.AddDocument(d);
            }
            Writer.Commit();
        }

        public static void Dispose()
        {
            Writer.Dispose();
            _directory.Dispose();
        }

        public static List<WaffleText> Search(string input)
        {
            const LuceneVersion lv = LuceneVersion.LUCENE_48;
            Analyzer a = new StandardAnalyzer(lv);
            var dirReader = DirectoryReader.Open(_directory);
            var searcher = new IndexSearcher(dirReader);

            string[] waffles = { "GUID", "WaffleHead", "WaffleBody" };
            var multiFieldQP = new MultiFieldQueryParser(lv, waffles, a);
            Query query = multiFieldQP.Parse(input.Trim());

            ScoreDoc[] docs = searcher.Search(query, null, 1000).ScoreDocs;

            var results = new List<WaffleText>();
            for (int i = 0; i < docs.Length; i++)
            {
                Document d = searcher.Doc(docs[i].Doc);
                WaffleText _localWaffle = new WaffleText();
                _localWaffle.GUID = d.Get("GUID");
                _localWaffle.WaffleHead = d.Get("WaffleHead");
                _localWaffle.WaffleBody = d.Get("WaffleBody");
                results.Add(_localWaffle);
            }

            dirReader.Dispose();
            return results;
        }
    }
}
</pre>

**Search Page**

The last piece of the puzzle is the user-facing Blazor search page that takes the user search input, invokes the engine and returns the search results. All of this functionality is contained in the Blazor page Index.razor.

<pre data-enlighter-language="csharp">
@page "/"

<PageTitle>Prose Search</PageTitle>

<BSRow>
    <BSCol Column="12" Padding="Padding.Small">    
    </BSCol>
</BSRow>

<EditForm Model="@searchModel" OnValidSubmit="@HandleSearch">
    <DataAnnotationsValidator />
    <BSRow>
        <BSCol Column="6" Padding="Padding.Small">
            <div class="input-group mb-3">
                <InputText class="form-control" placeholder="Enter Prose Text" @bind-Value="searchModel.SearchText" />
                <BSButton type="Submit" Color="BSColor.Primary">Search</BSButton>
            </div>
        </BSCol>
    </BSRow>
</EditForm>

@if(@SearchText!=String.Empty)
{
    <BSRow>
        <BSCol Column="12">
            <div class="mb-12">
                @if(@SearchResultsCount==1)
                {
                    <div>@SearchResultsCount Result</div>
                }
                else
                {
                    <div>@SearchResultsCount Results</div>
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
                    @foreach (var result in @searchModel.SearchResults)
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
        
    <BSRow>
        <BSCol Column="12" Padding="Padding.Large">    
        </BSCol>
    </BSRow>
}

@code {
    private SearchModel searchModel = new SearchModel();
    [Parameter]
    public string SearchText {get; set;} = string.Empty; 
    [Parameter]
    public int SearchResultsCount {get; set;} = 0;

    private void HandleSearch()
    {
        searchModel.SearchResults = SearchEngine.Search(searchModel.SearchText);
        searchModel.ResultsCount = searchModel.SearchResults.Count;
        SearchResultsCount = searchModel.ResultsCount;
        SearchText = searchModel.SearchText;
    }
}
</pre>