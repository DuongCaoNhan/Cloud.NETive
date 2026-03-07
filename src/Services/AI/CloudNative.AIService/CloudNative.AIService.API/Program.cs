using CloudNative.AIService.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddApplicationServices();    // Extensions/ServiceExtensions.cs

var app = builder.Build();
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection();
app.MapAIEndpoints();                // Extensions/WebAppExtensions.cs

app.Run();
