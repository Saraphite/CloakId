using CloakId;
using CloakId.Sqids;

var builder = WebApplication.CreateBuilder(args);

// Add CloakId services with Sqids encoding
builder.Services.AddCloakIdWithSqids();

// Add controllers with CloakId model binding support
builder.Services.AddControllers().AddCloakIdModelBinding();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
