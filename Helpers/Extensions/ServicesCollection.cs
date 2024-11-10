

using KPMGTask.Services.AuthenticationServices;

namespace KPMGTask.Helpers.Extensions
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


            /*  services.AddScoped<ICommandService, CommandService>();
              services.AddScoped<IUserService, UserService>();*/
            services.AddScoped<IAuthService, AuthService>();
            return services;
        
        }

    }
}
