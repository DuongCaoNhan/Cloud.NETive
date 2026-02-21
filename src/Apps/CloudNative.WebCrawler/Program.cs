using CloudNative.Core.Models;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new() { Title = "CloudNative Web Crawler", Version = "v1" }));

var app = builder.Build();
app.MapDefaultEndpoints();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection();

var api = app.MapGroup("/api/v1/crawl");

api.MapPost("/", (CrawlRequest req) =>
{
    // TODO: implement HtmlAgilityPack DOM crawling pipeline
    return ApiResponse<CrawlResult>.Ok(new(req.Url, [], DateTime.UtcNow), "Crawl queued");
})
.WithName("Crawl")
.WithSummary("Submit a URL for crawling");

app.Run();

record CrawlRequest(string Url, int MaxDepth = 1, bool FollowLinks = false);
record CrawlResult(string Url, IEnumerable<string> ExtractedLinks, DateTime CrawledAt);
