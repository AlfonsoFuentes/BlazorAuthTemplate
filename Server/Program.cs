using Server.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServerServices();
var app = builder.Build();

app.UseApp();


app.Run();
