using Statiq.App;
using Statiq.Common;
using Statiq.Web;

namespace StatiqBlog
{
  public class Program
  {
    public static async Task<int> Main(string[] args) =>
      await Bootstrapper
        .Factory
        .CreateWeb(args)
        .DeployToAzureAppService(
          "dotnet-statiq-beckshome-blog",
          "$dotnet-statiq-beckshome-blog",
          Config.FromSetting<string>("AzureAppServicePassword")
        )
        .RunAsync();
  }
}