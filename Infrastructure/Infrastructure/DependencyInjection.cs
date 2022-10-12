using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Infrastructure.IO;
using Microsoft.Extensions.DependencyInjection;

namespace ImageManipulator.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IFileService, FileService>();

        return services;
    }
}
