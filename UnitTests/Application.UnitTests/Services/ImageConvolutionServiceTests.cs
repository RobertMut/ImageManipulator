using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Text.Json;
using Core;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.Common.Services;
using NUnit.Framework;

namespace Application.UnitTests.Services;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ImageConvolutionServiceTests
{
    private IImageConvolutionService _imageConvolutionService;
    private Bitmap _testImage;

    private readonly double[,] _kernel = {
        { 0, -1, 0 },
        { -1, 4, -1 },
        { 0, -1, 0 }
    };
    
    [SetUp]
    public async Task SetUp()
    {
        _testImage = new Bitmap("Resources/image.png");
        _imageConvolutionService = new ImageConvolutionService();
    }

    [Test]
    public async Task ImagePointOperationsServiceComputesGradient()
    {
        double[,] expected = GetGradientFromFile(await File.ReadAllTextAsync("Resources/gradient"));
        double[,] gradient = _imageConvolutionService.ComputeGradient(_testImage, (gx, gy) => Math.Sqrt(gx * gx + gy * gy));
        
        Assert.That(gradient, Is.EqualTo(expected));
    }
    
    [Test]
    public async Task ImagePointOperationsServiceExecutesHysteresisThresholdWithSpecifiedGradient()
    {
        double[,] gradient = GetGradientFromFile(await File.ReadAllTextAsync("Resources/gradient"));
        Bitmap expectedImage = new Bitmap("Resources/image_hysteresis.png");
        Bitmap bitmap = _imageConvolutionService.HysteresisThresholding(512, 512, 10, 255, gradient);
        
        Bitmap pngBitmap;
        using (MemoryStream stream = new MemoryStream())
        {
            bitmap.Save(stream, ImageFormat.Png);
            pngBitmap = new Bitmap(stream);
        }
        
        byte[] expectedBytes = ImageHelper.ImageToByte(expectedImage);
        byte[] actualBytes = ImageHelper.ImageToByte(pngBitmap);
        
        Assert.That(actualBytes, Is.EqualTo(expectedBytes));
    }
    
    [Test]
    public async Task ImagePointOperationsServiceExecutesWithSpecifiedKernelAndSharpen()
    {
        Bitmap expectedImage = new Bitmap("Resources/image_convolution_sharpen.png");
        Bitmap bitmap = _imageConvolutionService.Execute(_testImage, _kernel);
        
        Bitmap pngBitmap;
        using (MemoryStream stream = new MemoryStream())
        {
            bitmap.Save(stream, ImageFormat.Png);
            pngBitmap = new Bitmap(stream);
        }
        
        byte[] expectedBytes = ImageHelper.ImageToByte(expectedImage);
        byte[] actualBytes = ImageHelper.ImageToByte(pngBitmap);
        
        Assert.That(actualBytes, Is.EqualTo(expectedBytes));
    }
    
    [Test]
    public async Task ImagePointOperationsServiceExecutesWithSpecifiedKernelAndSoften()
    {
        Bitmap expectedImage = new Bitmap("Resources/image_convolution_soften.png");
        Bitmap bitmap = _imageConvolutionService.Execute(_testImage, _kernel, true);
        
        Bitmap pngBitmap;
        using (MemoryStream stream = new MemoryStream())
        {
            bitmap.Save(stream, ImageFormat.Png);
            pngBitmap = new Bitmap(stream);
        }
        
        byte[] expectedBytes = ImageHelper.ImageToByte(expectedImage);
        byte[] actualBytes = ImageHelper.ImageToByte(pngBitmap);
        
        Assert.That(actualBytes, Is.EqualTo(expectedBytes));
    }
    
    private static double[,] GetGradientFromFile(string fileWithArray)
    {
        string[] lines = fileWithArray.Split("\r\n");
        
        double[,] array = new double[lines.Length, lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            string[] line = lines[i].Split("\t");

            for (int j = 0; j < line.Length; j++)
            {
                if (!string.IsNullOrEmpty(line[j]))
                {
                    array[i, j] = double.Parse(line[j]);
                }
            }
        }

        return array;
    }
}