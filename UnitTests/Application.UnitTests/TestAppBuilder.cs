using System.Diagnostics.CodeAnalysis;
using Application.UnitTests;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using Avalonia.Platform;
using Avalonia.ReactiveUI;
using ImageManipulator.Presentation;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

namespace Application.UnitTests;

[ExcludeFromCodeCoverage(Justification = "Used for [AvaloniaTest]")]
public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions
            {
                UseHeadlessDrawing = false,
                FrameBufferFormat = PixelFormat.Rgba8888
            })
            .UseWin32()
            .UseReactiveUI();
}