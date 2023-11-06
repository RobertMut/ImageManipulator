using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Infrastructure.Image;
using UnitTests.Core;

namespace Infrastructure.UnitTests;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ImageHistoryServiceTests
{
    private const string TestFileName = "testFile";
    private string _tempLocation = Path.GetTempPath();
    private IImageHistoryService _imageHistoryService;
    private Bitmap testBitmap;
    
    [SetUp]
    public async Task SetUp()
    {
        ClearTempImages();
        _imageHistoryService = new ImageHistoryService();
        testBitmap = ImageHelper.PaintImage(new Bitmap(200, 500), Color.Blue);

        await _imageHistoryService.StoreCurrentVersionAndGetThumbnail(testBitmap, $"{TestFileName}.bmp");
    }

    [Test]
    public async Task StoreCurrentVersionAndGetThumbnailReturnsThumbnail()
    {
        Bitmap bmp = await _imageHistoryService.StoreCurrentVersionAndGetThumbnail(testBitmap, $"{TestFileName}.bmp");
        
        float ratioX = testBitmap.Width > 300 || testBitmap.Height > 300 ? testBitmap.Width / testBitmap.Width / 3 : testBitmap.Width;
        float ratioY = testBitmap.Width > 300 || testBitmap.Height > 300 ? testBitmap.Height / testBitmap.Height / 3 : testBitmap.Height;
        float ratio = Math.Min(ratioX, ratioY);

        Bitmap thumbnail = new Bitmap(testBitmap.GetThumbnailImage((int)(testBitmap.Width * ratio), (int)(testBitmap.Height * ratio), null, IntPtr.Zero));

        byte[] storedThumbnail = ImageHelper.ImageToByte(bmp);
        byte[] generatedThumbnail = ImageHelper.ImageToByte(thumbnail);
        
        Assert.That(storedThumbnail, Is.EqualTo(generatedThumbnail));
    }

    [Test]
    public async Task ImageHistoryServiceReturnsVersions()
    {
        IEnumerable<string> files = Directory.GetFiles(_tempLocation).Where(x => x.Contains(TestFileName));
        IEnumerable<Image> expected = files.Select(x => ImageHelper.GetBitmapWithoutLock(x));

        IEnumerable<Image> history = await _imageHistoryService.GetVersions($"{TestFileName}.bmp");
        
        IEnumerable<byte[]> returnedImageBytes = history.Select(x => ImageHelper.ImageToByte(new Bitmap(x)));
        IEnumerable<byte[]> expectedImageBytes = expected.Select(x => ImageHelper.ImageToByte(new Bitmap(x)));
        
        Assert.That(history.Count(), Is.EqualTo(1));
        Assert.That(returnedImageBytes, Is.EquivalentTo(expectedImageBytes));
    }
    
    [Test]
    public async Task ImageHistoryServiceRestoresVersion()
    {
        using Bitmap newBitmap = ImageHelper.PaintImage(new Bitmap(200, 500), Color.Red);
        
        using Bitmap storedNewVersion = await _imageHistoryService.StoreCurrentVersionAndGetThumbnail(newBitmap, $"{TestFileName}.bmp");
        using Bitmap oldVersion = _imageHistoryService.RestoreVersion($"{TestFileName}.bmp", 0);
        {
            oldVersion.Compare(testBitmap);
        }
    }

    [TearDown]
    public void TearDown()
    {
        ClearTempImages();
    }
    

    private void ClearTempImages()
    {
        var files = Directory.GetFiles(_tempLocation).Where(x => x.Contains(TestFileName));

        foreach (var file in files)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }
}