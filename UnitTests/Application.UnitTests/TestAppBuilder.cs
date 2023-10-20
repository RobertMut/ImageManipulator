using Application.UnitTests;
using Avalonia;
using Avalonia.Headless;
using Avalonia.Platform;
using Avalonia.ReactiveUI;
using ImageManipulator.Presentation;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

namespace Application.UnitTests;

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