using CloudNative.Core.Models;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new() { Title = "CloudNative Translation API", Version = "v1" }));

var app = builder.Build();
app.MapDefaultEndpoints();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection();

var api = app.MapGroup("/api/v1/translate");

api.MapPost("/", (TranslateRequest req) =>
{
    // TODO: plug in real translation provider (Azure AI Translator, DeepL, etc.)
    return ApiResponse<TranslateResponse>.Ok(new(req.Text, req.TargetLanguage, req.Text, "mock"));
})
.WithName("Translate")
.WithSummary("Translate text to target language");

app.Run();

record TranslateRequest(string Text, string TargetLanguage, string? SourceLanguage = null);
record TranslateResponse(string Original, string TargetLanguage, string Translated, string Provider);
