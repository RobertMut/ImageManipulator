using System.Diagnostics.CodeAnalysis;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.Common.Services;
using ImageManipulator.Domain.Common.CQRS.Implementation;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ImageManipulator.Application;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.TryAddSingleton<ICommandDispatcher, CommandDispatcher>();
        services.TryAddSingleton<IQueryDispatcher, QueryDispatcher>();
        
        services.AddScoped<IImageDataService, ImageDataService>();
        services.AddScoped<ICommonDialogService, CommonDialogService>();
        services.AddScoped<IImagePointOperationsService, ImagePointOperationsService>();
        services.AddScoped<IImageArithmeticService, ImageArithmeticService>();
        services.AddScoped<IImageBitwiseService, ImageBitwiseService>();
        services.AddScoped<IImageConvolutionService, ImageConvolutionService>();
        services.AddScoped<IImageBorderService, ImageBorderService>();

        services.Scan(selector =>
        {
            selector.FromCallingAssembly()
                .AddClasses(filter =>
                {
                    filter.AssignableTo(typeof(IQueryHandler<,>));
                }).AsImplementedInterfaces()
                .WithSingletonLifetime();
        });
        
        services.Scan(selector =>
        {
            selector.FromCallingAssembly()
                .AddClasses(filter =>
                {
                    filter.AssignableTo(typeof(ICommandHandler<,>));
                }).AsImplementedInterfaces()
                .WithSingletonLifetime();
        });
        
        return services;
    }
}
