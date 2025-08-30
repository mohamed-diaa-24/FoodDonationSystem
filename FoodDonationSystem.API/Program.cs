
using FoodDonationSystem.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAllServices(builder.Configuration);


var app = builder.Build();

await app.ConfigurePipelineAsync();
app.Run();
