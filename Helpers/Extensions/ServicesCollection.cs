using MVCRESTAPI.Services.AuthenticationService;
using MVCRESTAPI.Services.CommandService;

namespace MVCRESTAPI.Helpers.Extensions
{
    public static  class ServicesCollection
    {
        public static IServiceCollection AddMyDependencyGroup(
            this IServiceCollection services)
        {
            // Register PollyClientPolicy as a singleton
            services.AddSingleton<PollyClientPolicy>();

            // Register HttpClient with Polly policies
            services.AddHttpClient("HttpClientFactory")
                .AddPolicyHandler((provider, request) =>
                {
                    var pollyPolicy = provider.GetRequiredService<PollyClientPolicy>();
                    return request.Method == HttpMethod.Get ? pollyPolicy.ImmediateHttpRetry : pollyPolicy.ExponentialHttpRetry;
                });


            services.AddScoped<ICommandService, CommandService>();
            services.AddScoped<IUserService, UserService>();
            return services;
        
        }

    }
}
