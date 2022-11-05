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

**Dynamic Configuration - Settings**

The dynamic configuration settings, specifically the size of the waffle text corpus to be geneated and the random seed initializer used for genration, are stored in the appsettings.json file and read at runtime.

<pre data-enlighter-language="json">
{
    "Logging": {
      "LogLevel": {
        "Default": "Information",
        "Microsoft.AspNetCore": "Warning"
      }
    },
    "AllowedHosts": "*",
    "BogusConfig": {
      "Rand": "11784",
      "WaffleCount": "3000"
    }
  }
</pre>

**Dynamic Configuration - Enablement**

The Microsoft.Extensions libraries are added to the project in the .csproj file to enable dynamic, JSON-based configuration.

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
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="6.0.1" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="6.0.0" />
    <PackageReference Include="WaffleGenerator.Bogus" Version="4.2.1" />
  </ItemGroup>

</Project>
</pre>

**Dynamic Configuration - Activation**

The program's dynamic configuration is implemented in the Program.cs file. The configuration file is open, settings are read and then passed dynamically to the GetData() method of the engine, which generates the sample data.

<pre data-enlighter-language="csharp">
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using search.Shared;
using BlazorStrap;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazorStrap();

// Setup configuration
var cfgBuilder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", true, true);
var config = cfgBuilder.Build();

// Search engine setup
SearchEngine.GetData(Int32.Parse(config["BogusConfig:Rand"]), Int32.Parse(config["BogusConfig:WaffleCount"]));
SearchEngine.Index();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
</pre>

**Pagination - Model Enablement**

To enable pagination, additional attributes are added to the SearchModel class to allow for the total count of pages and current page for a specific search.

<pre data-enlighter-language="csharp">
using System.ComponentModel.DataAnnotations;

namespace search.Shared
{
    public class SearchModel{
        [Required]
        public string SearchText {get; set;}
        public int ResultsCount {get; set;}
        public int PageCount {get; set;}
        public int CurrentPage {get; set;}
        public List<WaffleText> CurrentPageSearchResults {get; set;}
    }
}
</pre>

**Pagination - Implementation**

Pagination is implemented on the back end in the SearchEngine.cs class. The Search method signature and method have been changed significantly from the original post to enable paginated searches. Also, an EscapeSearchTerm function has been added to remove specific charachters from the search text. This function is applied to search input within the Search method.

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

        // Lucene supports escapting the following chars: + - && || ! ( ) { } [ ] ^ " ~ * ? : \
        // To make it easier, I remove / replace
        private static string EscapeSearchTerm(string input)
        {
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
            input = Regex.Replace(input, @"\"", " ");
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

**Pagination - User Interface**

Finally, the front-end pagination is added to the Index.razor class. All of this is made much easier through the presence of a very capable [BlazorStrap pagination component](https://blazorstrap.io/V5/components/pagination).

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

    private void UpdatePage()
    {
        searchModel = SearchEngine.Search(searchModel.SearchText, Page);
    }
}
</pre>