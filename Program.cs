using Microsoft.EntityFrameworkCore;
using MVCRESTAPI.Data;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//declare the DbContext that we are using it and connect it with the connection string 
builder.Services.AddDbContext<CommanderContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("CommanderConnection")));
builder.Services.AddControllers().AddNewtonsoftJson(s=> { s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); });

//builder.Services.AddScoped<ICommanderRepo, MockCommanderRepo>(); //This is mock will not return a real data
builder.Services.AddScoped<ICommanderRepo, SqlCommanderRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
