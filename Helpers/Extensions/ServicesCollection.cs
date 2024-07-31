using MVCRESTAPI.Services.AuthenticationService;
using MVCRESTAPI.Services.CommandService;

namespace MVCRESTAPI.Helpers.Extensions
{
    public static  class ServicesCollection
    {
        public static IServiceCollection AddMyDependencyGroup(
            this IServiceCollection services)
        {

            services.AddScoped<ICommandService, CommandService>();
            services.AddScoped<IUserService, UserService>();
            return services;
        
        }

    }
}
