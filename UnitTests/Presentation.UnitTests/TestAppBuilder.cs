using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Headless;
using Avalonia.Platform;
using Avalonia.ReactiveUI;
using ImageManipulator.Presentation;
using Presentation.UnitTests;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

namespace Presentation.UnitTests;

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