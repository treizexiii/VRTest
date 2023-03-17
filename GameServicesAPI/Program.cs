var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleansClient(builder => { builder.UseLocalhostClustering(); });

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerDocument();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseOpenApi();
app.UseSwaggerUi3();

app.Run();
