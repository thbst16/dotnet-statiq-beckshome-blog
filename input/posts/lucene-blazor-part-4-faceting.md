Title: "Lucene + Blazor, Part 4: Faceting"
Published: 11/20/2022
Tags:
    - Lucene.NET
    - .NET
---
In this fourth installment of my Blazor + Lucene.Net series, we'll make the most significant updates to the classes, search function and UI to date. Facets enrich the query responses, enabling users to further tune the search results along specific, pre-defined vectors. The implementation of facets uses the Lucene.Net.Facet library to implement an additional faceted index over two attributes -- Scholars and Universities -- which are applied on top of the WaffleText class and data. 

The code and code narrative below reflects the changes that have been made on top of the first 3 posts. All [source code is available online](https://github.com/thbst16/dotnet-lucene-search/tree/main/4-Faceting) for this results paging post.

**Sample App**

The sample application let's you search over 3,000 waffle text entries, returning paginated search results. Auto-complete functionality provides suggestion for the most relevant search terms in the waffle text index. On top of the search results, two attributes (Scholars and Universities) are available as facets. These facets can be drilled into or removed, shaping the results of the query appropriately. The site is available online at https://dotnet-lucene-search.azurewebsites.net/

![Faceting](https://s3.amazonaws.com/s3.beckshome.com/20221120-dotnet-lucene-faceting.jpeg)

**Faceted Search**

There are three significant changes to the SearchEngine.cs class:

1) Modification of the GetData() data generation function to generate Scholar and University facets for each of the WaffleText entries. The classes to support these facets are covered in subsequent sections.
2) Changes to the Index() method to add a new index for the facets and index the new facet fields.
3) Replacement of the default Search() method with a more advanced FacetedSearch() method that supports drill-down queries into each of the facets.

The heart of the search ahead function is included in a new method (SearchAhead) in the SearchEngine.cs file. The function creates a dictionary of terms on top of the search index and then searches that for the input text, returning an ordered set of results of words starting with the typed letters. To get this to work, I had to add a new field to the index (HeadBody) because there doesn't seem to be a way to apply a MultiFieldQueryParser over the LuceneDictionary of terms. 

<pre data-enlighter-language="csharp">
using Bogus;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Documents;
using Lucene.Net.Facet;
using Lucene.Net.Facet.Taxonomy;
using Lucene.Net.Facet.Taxonomy.Directory;
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
        private static RAMDirectory _indexDirectory;
        private static RAMDirectory _facetDirectory;
        public static IndexWriter indexWriter { get; set; }
        public static DirectoryTaxonomyWriter taxoWriter { get; set; }
        private static FacetsConfig facetConfig = new FacetsConfig();


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
                        includeHeading: false))
                .RuleFor(wt => wt.WaffleScholar, f => f.PickRandom<WaffleScholar>())
                .RuleFor(wt => wt.WaffleUniversity, f => f.PickRandom<WaffleUniversity>());
            
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
            _indexDirectory = new RAMDirectory();
            _facetDirectory = new RAMDirectory();
            var config = new IndexWriterConfig(lv, a);

            indexWriter = new IndexWriter(_indexDirectory, config);
            taxoWriter = new DirectoryTaxonomyWriter(_facetDirectory);

            var doc = new Document();

            foreach (WaffleText wt in Data)
            {
                doc = new Document();
                doc.Add(new StringField("GUID", wt.GUID, Field.Store.YES));
                doc.Add(new TextField("WaffleHead", wt.WaffleHead, Field.Store.YES));
                doc.Add(new TextField("WaffleBody", wt.WaffleBody, Field.Store.YES));
                doc.Add(new TextField("HeadBody", wt.WaffleHead + " " + wt.WaffleBody, Field.Store.YES));
                doc.Add(new TextField("WaffleScholarTxt", wt.WaffleScholar.ToString(), Field.Store.YES));
                doc.Add(new TextField("WaffleUniversityTxt", wt.WaffleUniversity.ToString(), Field.Store.YES));
                doc.Add(new FacetField("WaffleScholar", wt.WaffleScholar.ToString()));
                doc.Add(new FacetField("WaffleUniversity", wt.WaffleUniversity.ToString()));

                indexWriter.AddDocument(facetConfig.Build(taxoWriter, doc));
            }
            indexWriter.Commit();
            taxoWriter.Commit();
        }

        public static void Dispose()
        {
            indexWriter.Dispose();
            taxoWriter.Dispose();
            _indexDirectory.Dispose();
            _facetDirectory.Dispose();
        }

        public static SearchModel FacetedSearch(string input, int page, List<string> scholarDrillDowns = null, List<string> universityDrillDowns = null)
        {
            const LuceneVersion lv = LuceneVersion.LUCENE_48;
            Analyzer a = new StandardAnalyzer(lv);
            
            string[] fnames = { "GUID", "WaffleHead", "WaffleBody" };
            var multiFieldQP = new MultiFieldQueryParser(lv, fnames, a);
            string _input = EscapeSearchTerm(input.Trim());
            Query query = multiFieldQP.Parse(_input);

            // Add drill down query
            DrillDownQuery ddq = new DrillDownQuery(facetConfig, query);
            if (scholarDrillDowns is not null)
            {
                foreach (string scholar in scholarDrillDowns)
                {
                    ddq.Add("WaffleScholar", scholar);
                }
            }
            if (universityDrillDowns is not null)
            {
                foreach (string university in universityDrillDowns)
                {
                    ddq.Add("WaffleUniversity", university);
                }
            }
            
            using (DirectoryReader indexReader = DirectoryReader.Open(_indexDirectory))
            using (TaxonomyReader taxoReader = new DirectoryTaxonomyReader(_facetDirectory))
            {
                IndexSearcher searcher = new IndexSearcher(indexReader);

                // Execute document search and return collection of WaffleText class
                ScoreDoc[] docs = searcher.Search(ddq, null, 1000).ScoreDocs;
                var waffles = new List<WaffleText>();
                int first = (page-1)*5;
                int last = first + 5;
                for (int i = first; i < last && i < docs.Length; i++)
                {
                    Document doc = searcher.Doc(docs[i].Doc);
                    WaffleText _waffle = new WaffleText();
                    _waffle.GUID = doc.Get("GUID");
                    _waffle.WaffleHead = doc.Get("WaffleHead");
                    _waffle.WaffleBody = doc.Get("WaffleBody");
                    _waffle.WaffleScholar = (WaffleScholar)Enum.Parse(typeof(WaffleScholar), doc.Get("WaffleScholarTxt"));
                    _waffle.WaffleUniversity = (WaffleUniversity)Enum.Parse(typeof(WaffleUniversity), doc.Get("WaffleUniversityTxt"));
                    waffles.Add(_waffle);
                }

                var returnModel = new SearchModel();
                returnModel.CurrentPageSearchResults = waffles;
                returnModel.SearchText = _input;
                returnModel.ResultsCount = docs.Length;
                returnModel.PageCount = (int)Math.Ceiling(docs.Length/5.0);
                returnModel.CurrentPage = page;

                // Execute facets search and return collection of FacetResults class
                FacetsCollector fc = new FacetsCollector();
                FacetsCollector.Search(searcher, ddq, 100, fc);
                IList<FacetResult> results = new List<FacetResult>();
                Facets facets = new FastTaxonomyFacetCounts(taxoReader, facetConfig, fc);
                results.Add(facets.GetTopChildren(100, "WaffleScholar"));
                results.Add(facets.GetTopChildren(100, "WaffleUniversity"));

                returnModel.FacetResults = results;

                return returnModel;
            }
        }

        public static List<string> SearchAhead(string input)
        {
            const LuceneVersion lv = LuceneVersion.LUCENE_48;
            Analyzer a = new StandardAnalyzer(lv);
            var dirReader = DirectoryReader.Open(_indexDirectory);

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

        // Lucene supports escaping the following chars: + - && || ! ( ) { } [ ] ^ " ~ * ? : \
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
            input = Regex.Replace(input, @"\"""", " ");
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

**WaffleText Support for Facets**

The base WaffleText class in WaffleText.cs changes to add support for the WaffleScholar and WaffleUniversity facets. Both of these are implemented as separate Enums. 

<pre data-enlighter-language="csharp">
namespace search.Shared
{
    public class WaffleText
    {
        public string? GUID { get; set; }
        public string? WaffleHead { get; set; }
        public string? WaffleBody { get; set; }
        public WaffleScholar? WaffleScholar { get; set; }
        public WaffleUniversity? WaffleUniversity { get; set; }

        public WaffleText() {}
        public WaffleText(string _guid, string _waffleHead, string _waffleBody, WaffleScholar _waffleScholar, WaffleUniversity _waffleUniversity)
        {
            GUID = _guid;
            WaffleHead = _waffleHead;
            WaffleBody = _waffleBody;
            WaffleScholar = _waffleScholar;
            WaffleUniversity = _waffleUniversity;
        }
    }
}
</pre>

**Addition of Enums to Support Faceting**

The WaffleScholar and WaffleUniversity enums were both added to support the new facets.

<pre data-enlighter-language="csharp">
namespace search.Shared
{
    public enum WaffleScholar
    {
       Freud,
       Copernicus,
       Erasmus,
       Descartes,
       Einstein,
       Newton,
       Goethe,
       Confucius
    }
}
</pre>

<pre data-enlighter-language="csharp">
namespace search.Shared
{
    public enum WaffleUniversity
    {
       MIT,
       Cambridge,
       Stanford,
       Oxford,
       Harvard,
       Caltech,
       UCL,
       Penn,
       Edinburgh,
       Princeton
    }
}
</pre>

**Faceted User Interface**

The changes to Index.razor to support a faceted UI have been the most significant changes to the UI to date. Try it out and see how the search results, pagination and search result summary are all dynamically updated based upon the selection / un-selection of individual facets.

The following changes were necessary:

* Changes to the search result wording, showing the number of results returned and displayed out of the total results.
* Addition of a new display area housing the facets. This display area iterates over the facets returned from a search and displays them. It also makes calls to the new ScholarFilter() / ScholarRemove() and UniversityFilter() / UniversityRemove() methods to apply the facets and update the search results. 
* Addition of facet badges for each of the search results to visually show the facets associated with each of the individual results of the search.

<pre data-enlighter-language="csharp">
@page "/"
@inject NavigationManager NavManager

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
                <div>Showing <b>@((Page*5)-4) - @(Math.Min(Page*5, SearchResultsCount))</b> out of <b>@SearchResultsCount</b>  for: <i>@SearchText</i> </div>
            </div>
        </BSCol>
    </BSRow>
    <BSRow>
        <BSCol Column="12" Padding="Padding.Small">    
        </BSCol>
    </BSRow>
}

<BSRow>

    <BSCol Column="3"  PaddingLeftAndRight="Padding.Medium">

        @if(@SearchResultsCount>0)
        {

            <BSRow class="h-100 border rounded">
                <BSCol Column="12" PaddingLeftAndRight="Padding.Medium">
                    <div class="mb-12">
                        <BSRow Padding="Padding.ExtraSmall"></BSRow>
                        <BSRow class="text-muted"><b>Scholars</b></BSRow>
                        <BSRow Padding="Padding.ExtraSmall"></BSRow>

                        @if (@ScholarFacet.Count == 0)
                        {
                            @foreach (var _scholarFacet in @searchModel.FacetResults[0].LabelValues)
                            {
                                <BSRow Padding="Padding.ExtraSmall">
                                    <BSCol Column="10" Align="Align.Start">
                                        <BSLink href="javascript:void(0)" class="link-dark" style="text-decoration: none" OnClick="() => ScholarFilter(_scholarFacet)">@_scholarFacet.Label</BSLink>
                                    </BSCol>
                                    <BSCol Column="2" Align="Align.End">@_scholarFacet.Value</BSCol>
                                </BSRow>
                            }
                        }
                        else
                        {
                            <BSRow Padding="Padding.ExtraSmall">
                                <BSCol Column="12" Align="Align.Start">
                                    <b>@ScholarFacet[0]</b>
                                    (
                                        <BSLink href="javascript:void(0)" class="link-primary" style="text-decoration: none" OnClick="ScholarRemove">Remove</BSLink>
                                    )
                                </BSCol>
                            </BSRow>
                        }

                        <BSRow Padding="Padding.Small"></BSRow>
                        <BSRow class="text-muted"><b>Universities</b></BSRow>
                        <BSRow Padding="Padding.ExtraSmall"></BSRow>

                         @if (@UniversityFacet.Count == 0)
                        {
                            @foreach (var _universityFacet in @searchModel.FacetResults[1].LabelValues)
                            {
                                <BSRow Padding="Padding.ExtraSmall">
                                    <BSCol Column="10" Align="Align.Start">
                                        <BSLink href="javascript:void(0)" class="link-dark" style="text-decoration: none" OnClick="() => UniversityFilter(_universityFacet)">@_universityFacet.Label</BSLink>
                                    </BSCol>
                                    <BSCol Column="2" Align="Align.End">@_universityFacet.Value</BSCol>
                                </BSRow>
                            }
                        }
                        else
                        {
                            <BSRow Padding="Padding.ExtraSmall">
                                <BSCol Column="12" Align="Align.Start">
                                    <b>@UniversityFacet[0]</b>
                                    (
                                        <BSLink href="javascript:void(0)" class="link-primary" style="text-decoration: none" OnClick="UniversityRemove">Remove</BSLink>
                                    )
                                </BSCol>
                            </BSRow>
                        }

                    </div>
                </BSCol>
            </BSRow>

        }

    </BSCol>

    <BSCol Column="9">

        @if(@SearchResultsCount>0)
        {
            <BSRow>
                <BSCol Column="12">
                    <div class="mb-12">
                        <BSListGroup>
                            @foreach (var result in @searchModel.CurrentPageSearchResults)
                            {
                                <BSListGroupItem>
                                    <div class="d-flex w-100 justify-content-between">
                                        <h5 class="mb-1">@result.WaffleHead</h5>
                                    </div>
                                    <div>
                                        <BSBadge Color="BSColor.Primary">@result.WaffleScholar</BSBadge>
                                        <BSBadge Color="BSColor.Secondary">@result.WaffleUniversity</BSBadge>
                                    </div>
                                    <p class="mb-1">@result.WaffleBody</p>
                                </BSListGroupItem>
                            }
                        </BSListGroup>
                    </div>
                </BSCol>
            </BSRow>
        }
    </BSCol>

</BSRow>

<BSRow>

    
        <BSCol Column="12">

            <BSRow>
                <BSCol Column="12" Padding="Padding.Small">  
                </BSCol>
            </BSRow>

            <BSRow>

                <BSCol Column="3">
                </BSCol>

                <BSCol Column="9">
                    
                    <BSRow @onclick="UpdatePage">
                        <BSCol Column="12">
                            <div class="mb-12">
                                @if(@PageCount>1)
                                {
                                    <BSPagination Pages=@PageCount @bind-Value="Page"/>
                                }
                            </div>
                        </BSCol>
                    </BSRow>

                </BSCol>

            </BSRow>

        </BSCol>

</BSRow>

<BSRow>
    <BSCol Column="12" Padding="Padding.Large">     
    </BSCol>
</BSRow>

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
    [Parameter]
    public List<string> ScholarFacet {get; set;} = new List<string>();
    [Parameter]
    public List<string> UniversityFacet {get; set;} = new List<string>();

    private void HandleSearch()
    {
        ScholarFacet.Clear();
        UniversityFacet.Clear();
        Page = 1;
        UpdatePage();
    }

    private async Task<IEnumerable<String>> HandleTypeAhead(string searchText)
    {
        List<String> SResult = SearchEngine.SearchAhead(searchText);
        return await Task.FromResult(SResult.Where(x => x.ToLower().Contains(searchText.ToLower())).ToList());
    }

    private void ScholarFilter(Lucene.Net.Facet.LabelAndValue _scholarFacet)
    {
        ScholarFacet.Clear();
        ScholarFacet.Add(_scholarFacet.Label);
        Page = 1;
        UpdatePage();
    }

    private void ScholarRemove()
    {
        ScholarFacet.Clear();
        Page = 1;
        UpdatePage();
    }

    private void UniversityFilter(Lucene.Net.Facet.LabelAndValue _universityFacet)
    {
        UniversityFacet.Clear();
        UniversityFacet.Add(_universityFacet.Label);
        Page = 1;
        UpdatePage();
    }

    private void UniversityRemove()
    {
        UniversityFacet.Clear();
        Page = 1;
        UpdatePage();
    }
    
    private void UpdatePage()
    {
        if (searchModel.SearchText is not null && searchModel.SearchText.Length > 0)
        {
            searchModel = SearchEngine.FacetedSearch(searchModel.SearchText, Page, ScholarFacet, UniversityFacet);
            SearchResultsCount = searchModel.ResultsCount;
            PageCount = searchModel.PageCount;
            SearchText = searchModel.SearchText;
        }
        else
        {
            NavManager.NavigateTo("/");
        }
    }
}
</pre>

**Facet - Enablement**

Finally, the Lucene.Net.Facet library is added to the project in the .csproj file.

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
    <PackageReference Include="Lucene.Net.Facet" Version="4.8.0-beta00016" />
    <PackageReference Include="Lucene.Net.QueryParser" Version="4.8.0-beta00016" />
    <PackageReference Include="Lucene.Net.Suggest" Version="4.8.0-beta00016" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="6.0.1" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="6.0.0" />
    <PackageReference Include="WaffleGenerator.Bogus" Version="4.2.1" />
  </ItemGroup>

</Project>
</pre>