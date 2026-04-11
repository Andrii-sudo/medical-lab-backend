using LabAPI;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddIdentityServices(builder.Configuration)
    .AddCorsServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // https://localhost:7171/openapi/v1.json
    app.MapOpenApi();
    
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Lab API v1");
    });
}

app.UseHttpsRedirection();

app.UseCors(CorsPolicies.Angular);
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
