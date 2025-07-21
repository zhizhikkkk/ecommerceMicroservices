using ProductApi.Infrastructure.DependencyInjection;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureService(builder.Configuration);
var app = builder.Build();
app.UseInfrastructurePolicy();
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
