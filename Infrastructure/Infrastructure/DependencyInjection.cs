using System.Diagnostics.CodeAnalysis;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Infrastructure.Image;
using ImageManipulator.Infrastructure.IO;
using ImageManipulator.Infrastructure.Tab;
using Microsoft.Extensions.DependencyInjection;

namespace ImageManipulator.Infrastructure;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IFileService, FileService>();
        services.AddSingleton<ITabService, TabService>();
        services.AddSingleton<IImageHistoryService, ImageHistoryService>();
        
        return services;
    }
}
