using CativeConvert.Commands;
using CativeConvert.Commands.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CativeConvert
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddCativeConvert(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddScoped(typeof(IConvertToPdfCommand<>), typeof(ConvertToPdfCommand<>));

            services.AddScoped<IConverter, ModelConverter>();

            return services;
        }
    }
}
