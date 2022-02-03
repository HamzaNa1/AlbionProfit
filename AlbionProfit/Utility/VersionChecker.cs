using System.Reflection;
using Newtonsoft.Json;

namespace AlbionProfit.Utility;

public class VersionChecker
{
    public async Task<bool> IsUpToDate()
    {
        string? currentVersion = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;
        
        return currentVersion == await GetVersionFromApi();
    }

    public string? GetCurrentVersion()
    {
        return Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;
    }
    
    public async Task<string> GetVersionFromApi()
    {
        using HttpClient client = new HttpClient();

        string response = await client.GetStringAsync("https://api.nuget.org/v3/registration5-semver1/albionprofit/index.json");
        Root root = JsonConvert.DeserializeObject<Root>(response);
        return root.Items[0].Upper;
    }
    
     public partial class Root
    {
        [JsonProperty("@id")]
        public Uri Id { get; set; }

        [JsonProperty("@type")]
        public string[] Type { get; set; }

        [JsonProperty("commitId")]
        public Guid CommitId { get; set; }

        [JsonProperty("commitTimeStamp")]
        public DateTimeOffset CommitTimeStamp { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("items")]
        public VersionItem[] Items { get; set; }

        [JsonProperty("@context")]
        public Context Context { get; set; }
    }

    public partial class Context
    {
        [JsonProperty("@vocab")]
        public Uri Vocab { get; set; }

        [JsonProperty("catalog")]
        public Uri Catalog { get; set; }

        [JsonProperty("xsd")]
        public Uri Xsd { get; set; }

        [JsonProperty("items")]
        public Dependencies Items { get; set; }

        [JsonProperty("commitTimeStamp")]
        public CommitTimeStamp CommitTimeStamp { get; set; }

        [JsonProperty("commitId")]
        public CommitId CommitId { get; set; }

        [JsonProperty("count")]
        public CommitId Count { get; set; }

        [JsonProperty("parent")]
        public CommitTimeStamp Parent { get; set; }

        [JsonProperty("tags")]
        public Dependencies Tags { get; set; }

        [JsonProperty("reasons")]
        public Reasons Reasons { get; set; }

        [JsonProperty("packageTargetFrameworks")]
        public Dependencies PackageTargetFrameworks { get; set; }

        [JsonProperty("dependencyGroups")]
        public Dependencies DependencyGroups { get; set; }

        [JsonProperty("dependencies")]
        public Dependencies Dependencies { get; set; }

        [JsonProperty("packageContent")]
        public PackageContent PackageContent { get; set; }

        [JsonProperty("published")]
        public PackageContent Published { get; set; }

        [JsonProperty("registration")]
        public PackageContent Registration { get; set; }
    }

    public partial class CommitId
    {
        [JsonProperty("@id")]
        public string Id { get; set; }
    }

    public partial class CommitTimeStamp
    {
        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("@type")]
        public string Type { get; set; }
    }

    public partial class Dependencies
    {
        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("@container")]
        public string Container { get; set; }
    }

    public partial class PackageContent
    {
        [JsonProperty("@type")]
        public string Type { get; set; }
    }

    public partial class Reasons
    {
        [JsonProperty("@container")]
        public string Container { get; set; }
    }

    public partial class VersionItem
    {
        [JsonProperty("@id")]
        public Uri Id { get; set; }

        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("commitId")]
        public Guid CommitId { get; set; }

        [JsonProperty("commitTimeStamp")]
        public DateTimeOffset CommitTimeStamp { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("items")]
        public ItemItem[] Items { get; set; }

        [JsonProperty("parent")]
        public Uri Parent { get; set; }

        [JsonProperty("lower")]
        public string Lower { get; set; }

        [JsonProperty("upper")]
        public string Upper { get; set; }
    }

    public partial class ItemItem
    {
        [JsonProperty("@id")]
        public Uri Id { get; set; }

        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("commitId")]
        public Guid CommitId { get; set; }

        [JsonProperty("commitTimeStamp")]
        public DateTimeOffset CommitTimeStamp { get; set; }

        [JsonProperty("catalogEntry")]
        public CatalogEntry CatalogEntry { get; set; }

        [JsonProperty("packageContent")]
        public Uri PackageContent { get; set; }

        [JsonProperty("registration")]
        public Uri Registration { get; set; }
    }

    public partial class CatalogEntry
    {
        [JsonProperty("@id")]
        public Uri Id { get; set; }

        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("authors")]
        public string Authors { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("iconUrl")]
        public string IconUrl { get; set; }

        [JsonProperty("id")]
        public string CatalogEntryId { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("licenseExpression")]
        public string LicenseExpression { get; set; }

        [JsonProperty("licenseUrl")]
        public string LicenseUrl { get; set; }

        [JsonProperty("listed")]
        public bool Listed { get; set; }

        [JsonProperty("minClientVersion")]
        public string MinClientVersion { get; set; }

        [JsonProperty("packageContent")]
        public Uri PackageContent { get; set; }

        [JsonProperty("projectUrl")]
        public string ProjectUrl { get; set; }

        [JsonProperty("published")]
        public DateTimeOffset Published { get; set; }

        [JsonProperty("requireLicenseAcceptance")]
        public bool RequireLicenseAcceptance { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}