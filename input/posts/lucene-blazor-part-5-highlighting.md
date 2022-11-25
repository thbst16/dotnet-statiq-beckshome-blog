Title: "Lucene + Blazor, Part 5: Highlighting"
Published: 11/25/2022
Tags:
    - Lucene.NET
    - .NET
---
In this final installment of my Blazor + Lucene.Net series, we'll be adding highlights for the search terms found in the header and body text of each of our results. The implementation of highlighting makes use of the Lucene.Net.Highlighter library, plugging this library into a simple method that can be used as a filter for search results to highlight key terms. 

The code and code narrative below reflects the changes that have been made on top of the first 4 posts. All [source code is available online](https://github.com/thbst16/dotnet-lucene-search/tree/main/5-Highlighting) for this highlighting post.

**Sample App**

The sample application let's you search over 3,000 waffle text entries, returning paginated search results. Auto-complete functionality provides suggestion for the most relevant search terms in the waffle text index. On top of the search results, two attributes (Scholars and Universities) are available as facets. Finally, search results in the header and body of the waffle text are highlighted. The site is available online at https://dotnet-lucene-search.azurewebsites.net/

![Highlighting](https://s3.amazonaws.com/s3.beckshome.com/20221122-dotnet-lucene-highlighting.jpeg)

**Highlighted Search Terms**

There are two modifications to the search engine (SearchEngine.cs) to enable search highlighting:

1) A new static method is added (GenerateHighlightedText()), which takes in the components needed by the Lucene.Net.Highlighter library's GetBestFragments() method to surround keywords with the HTML Mark and Strong tags for highlighting.
2) The existing FacetedSearch method is modified to pass the WaffleHead and WaffleBody text through the GenerateHighlightedText() method to highlight text in these two fields. Lines 135 and 137 below.

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
using Lucene.Net.Search.Highlight;
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
                    _waffle.WaffleHead = GenerateHighlightedText(a, query, doc.Get("WaffleHead"), "WaffleHead");
                        if (_waffle.WaffleHead == string.Empty) {_waffle.WaffleHead = doc.Get("WaffleHead");}
                    _waffle.WaffleBody = GenerateHighlightedText(a, query, doc.Get("WaffleBody"), "WaffleBody");
                        if (_waffle.WaffleBody == string.Empty) {_waffle.WaffleBody = doc.Get("WaffleBody");}
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
            input = Regex.Replace(input, @"\"""", " ");
            input = Regex.Replace(input, @"\~", " ");
            input = Regex.Replace(input, @"\*", " ");
            input = Regex.Replace(input, @"\?", " ");
            input = Regex.Replace(input, @"\:", " ");
            input = Regex.Replace(input, @"\\", " ");
            return input;
        }

         public static string GenerateHighlightedText(Analyzer a, Query q, string docPart, string fieldName)
        {
            QueryScorer scorer = new QueryScorer(q, fieldName);
            SimpleHTMLFormatter formatter = new SimpleHTMLFormatter("<mark><strong>", "</strong></mark>");
            Highlighter highlighter = new Highlighter(formatter, scorer);
            highlighter.TextFragmenter = (new SimpleFragmenter(int.MaxValue));
            TokenStream stream = a.GetTokenStream(fieldName, docPart);
            return highlighter.GetBestFragments(stream, docPart, 10, "...");
        }
    }
}
</pre>

**Highlighting in the UI**

The changes to Index.razor to support highlighted text are very minimal. Since the Waffle Head and Waffle Body are being displayed in the search results already, the highlighting comes through in the HTML tags added by the GenerateHighlightedText() method. The only thing that needs to be done is to cast the text to markup using(MarkupString) so that the HTML tags aren't rendered literally. You can find this in lines 131 and 137 of the Index.razor code below.

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
                                        <h5 class="mb-1">@((MarkupString)@result.WaffleHead)</h5>
                                    </div>
                                    <div>
                                        <BSBadge Color="BSColor.Primary">@result.WaffleScholar</BSBadge>
                                        <BSBadge Color="BSColor.Secondary">@result.WaffleUniversity</BSBadge>
                                    </div>
                                    <p class="mb-1">@((MarkupString)@result.WaffleBody)</p>
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

**Highlighting - Enablement**

Finally, the Lucene.Net.Hihglighter library is added to the project in the .csproj file.

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
    <PackageReference Include="Lucene.Net.Highlighter" Version="4.8.0-beta00016" />
    <PackageReference Include="Lucene.Net.QueryParser" Version="4.8.0-beta00016" />
    <PackageReference Include="Lucene.Net.Suggest" Version="4.8.0-beta00016" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="6.0.1" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="6.0.0" />
    <PackageReference Include="WaffleGenerator.Bogus" Version="4.2.1" />
  </ItemGroup>

</Project>
</pre>