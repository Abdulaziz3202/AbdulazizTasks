using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MVCRESTAPI.Helpers.Swagger
{
    public class SwaggerBaseUrlFilter : IOperationFilter
    {
        private readonly IConfiguration _configuration;
        public SwaggerBaseUrlFilter(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {

            var serverRootAddress = _configuration.GetValue<string>("App:ServerRootAddress");

            // If the serverRootAddress is not specified in appsettings.json, you can provide a default value
            serverRootAddress = serverRootAddress ?? "https://localhost:7178";


            operation.Servers = new List<OpenApiServer>
        {
           new OpenApiServer { Url = serverRootAddress }
        };
        }
    }
}
