using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.Common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ImageManipulator.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IImageDataService, ImageDataService>();
        services.AddScoped<ICommonDialogService, CommonDialogService>();
        services.AddScoped<IGraphService, GraphService>();
        services.AddScoped<IImagePointOperationsService, ImagePointOperationsService>();
        services.AddScoped<IImageArithmeticService, ImageArithmeticService>();
        services.AddScoped<IImageBitwiseService, ImageBitwiseService>();
        services.AddScoped<IImageConvolutionService, ImageConvolutionService>();
        services.AddScoped<IImageBorderService, ImageBorderService>();
        
        return services;
    }
}
