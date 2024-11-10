
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;

using KPMGTask.Helpers.Swagger;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerUI;
using Serilog;
using Serilog.Exceptions;
using KPMGTask.Helpers.Extensions;
using KPMGTask.EntityFrameworkCore;
using KPMGTask.Middlewares;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using KPMGTask.Models;
using KPMGTask.Services.AuthenticationServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using KPMGTask.Services.TransactionServices;
using KPMGTask.Services.TransactionServices.Dto;
using KPMGTask.Dtos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
// Add services to the container.

//declare the DbContext that we are using it and connect it with the connection string 
builder.Services.AddDbContext<AppDBContextContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Register Identity services (this includes UserManager, SignInManager, etc.)
builder.Services.AddIdentity<User, Role>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<AppDBContextContext>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
        ClockSkew = TimeSpan.Zero
    };
});


builder.Services.AddScoped<IAuthService, AuthService>(); // Register your AuthService
builder.Services.AddScoped<ITransactionServices, TransactionServices>();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});





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

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{

    c.SwaggerDoc("v1", new OpenApiInfo
    {

        Title = "REST API",

        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference=new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
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


builder.Services.AddDistributedMemoryCache(); // Registers the memory cache for session storage
builder.Services.AddSession(); // Registers session services that will use the distributed cache
builder.Services.AddHttpContextAccessor(); // Register HttpContextAccessor (useful for getting HttpContext)
builder.Services.AddAuthorization(); // Authorization doesn't directly depend on other services, but it comes after context
builder.Services.AddScoped<TransactionServices>(); // Register your Transaction services (shouldn't depend on session/cache or context directly)
builder.Services.AddSingleton<KPMGTask.Middlewares.WebSocketManager>(); // Register WebSocketManager as a singleton


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

app.UseAuthentication(); // This is required to authenticate JWT tokens

app.UseAuthorization();
app.UseWebSockets();
app.MapControllers();

app.Run();
