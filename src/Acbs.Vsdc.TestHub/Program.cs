using Acbs.Vsdc.TestHub.Bootstrap;

var builder = WebApplication.CreateBuilder(args);
builder.AddTestHubConfiguration();
builder.Services.AddTestHubServices(builder.Configuration, builder.Environment);

var app = builder.Build();
app.UseTestHubPipeline();
await app.InitializeTestHubAsync();
app.Run();
