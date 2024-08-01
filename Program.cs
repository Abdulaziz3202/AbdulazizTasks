using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using MVCRESTAPI.Helpers;
using MVCRESTAPI.Helpers.Swagger;
using MVCRESTAPI.Services.AuthenticationService;
using MVCRESTAPI.Services.AuthenticationService.AuthHelper;
using MVCRESTAPI.Services.CommandService;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerUI;
using Serilog;
using Serilog.Exceptions;
using MVCRESTAPI.Helpers.Extensions;
using MVCRESTAPI.EntityFrameworkCore;
using MVCRESTAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//declare the DbContext that we are using it and connect it with the connection string 
builder.Services.AddDbContext<CommanderContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("CommanderConnection")));
builder.Services.AddControllers().AddNewtonsoftJson(options => {
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
}).AddJsonOptions(options =>
{
    // for GeoMetry Serlization purposes
    options.JsonSerializerOptions.Converters.Add(new GeometryJsonConverter());
}); ;





builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddCors(
              options => options.AddPolicy(
                   builder.Configuration.GetSection("App:Domain").Value,
                  corsPolicyBuilder => corsPolicyBuilder
                      .WithOrigins(
                          // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                          builder.Configuration.GetSection("App:CorsOrigins").Value
                              .Split(",", StringSplitOptions.RemoveEmptyEntries)
                              .Select(o => o.RemovePostFix("/"))
                              .ToArray()
                      )
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials()
              //.WithExposedHeaders("Content-Disposition") // content-disposition is *exposed* (and allowed because of AllowAnyHeader)

              )
          );
// Additional service registration
builder.Services.AddMyDependencyGroup();

builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSwaggerGen(c =>
{

    c.SwaggerDoc("v1", new OpenApiInfo
    {

        Title = "REST API",

        Version = "v1"
    });
    c.AddSecurityDefinition("basic", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference=new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="basic"
                }
            },
            new string[] {}
        }
    });
    c.OperationFilter<SwaggerBaseUrlFilter>();
});

Log.Logger = new LoggerConfiguration().Enrich.WithExceptionDetails().CreateBootstrapLogger();

builder.Host.UseSerilog(((ctx, lc) => lc
.ReadFrom.Configuration(ctx.Configuration)));


var app = builder.Build();
app.UseCors(builder.Configuration.GetSection("App:Domain").Value); // Enable CORS!
app.UseSerilogRequestLogging();
app.UseCustomExceptionMiddleware();




app.UseSwagger(c =>
{
    var basePath = builder.Configuration.GetSection("Swagger:BasePath").Value;
    c.PreSerializeFilters.Add((swaggerDoc, httpRequest) =>
    {
        if (!httpRequest.Headers.ContainsKey("X-Forwarded-Host")) return;


        var serverUrl = $"{httpRequest.Scheme}://{httpRequest.Headers["X-Forwarded-Host"]}{basePath}";

        swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = serverUrl } };
    });
});
var swaggerEndpoint = builder.Configuration.GetSection("Swagger:Endpoint").Value;
if (string.IsNullOrEmpty(swaggerEndpoint))
{
    throw new InvalidOperationException("Swagger endpoint configuration is missing or empty.");
}
app.UseSwaggerUI(options =>
{

    // Swagger JSON endpoint.
    options.SwaggerEndpoint(swaggerEndpoint, $"Project Name API v1");




    options.DisplayRequestDuration(); // Controls the display of the request duration (in milliseconds) for "Try it out" requests. 
    options.DocExpansion(DocExpansion.None); // Set the API operations to be collapsed by default

    options.DefaultModelsExpandDepth(-1); // Set the model schemas to be collapsed by default
});

app.Use((context, next) =>
{
    StringValues pathBase = context.Request.Headers["X-Forwarded-PathBase"];
    context.Request.PathBase = new PathString(pathBase);
    return next();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
