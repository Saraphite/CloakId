using CloakId.Sqids;
using CloakId.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add CloakId services with Sqids encoding
builder.Services.AddCloakIdWithSqids();

// Add controllers with CloakId ASP.NET Core integration (model binding)
builder.Services.AddControllers().AddCloakId();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "CloakId WebAPI Example", Version = "v1" });
    
    // Add CloakId OpenAPI support - parameters marked with [CloakId] will be documented as strings
    c.AddCloakIdSupport();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CloakId WebAPI Example v1");
        c.DocumentTitle = "CloakId WebAPI Example";
    });
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
