using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Infrastructure.Image;

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
        testBitmap = PaintImage(new Bitmap(200, 500), Color.Blue);

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

        byte[] storedThumbnail = ImageToByte(bmp);
        byte[] generatedThumbnail = ImageToByte(thumbnail);
        
        Assert.That(storedThumbnail, Is.EqualTo(generatedThumbnail));
    }

    [Test]
    public async Task ImageHistoryServiceReturnsVersions()
    {
        IEnumerable<string> files = Directory.GetFiles(_tempLocation).Where(x => x.Contains(TestFileName));
        IEnumerable<Image> expected = files.Select(x => GetBitmapWithoutLock(x));

        IEnumerable<Image> history = await _imageHistoryService.GetVersions($"{TestFileName}.bmp");
        
        IEnumerable<byte[]> returnedImageBytes = history.Select(x => ImageToByte(x));
        IEnumerable<byte[]> expectedImageBytes = expected.Select(x => ImageToByte(x));
        
        Assert.That(history.Count(), Is.EqualTo(1));
        Assert.That(returnedImageBytes, Is.EquivalentTo(expectedImageBytes));
    }
    
    [Test]
    public async Task ImageHistoryServiceRestoresVersion()
    {
        using Bitmap newBitmap = PaintImage(new Bitmap(200, 500), Color.Red);
        
        using Bitmap storedNewVersion = await _imageHistoryService.StoreCurrentVersionAndGetThumbnail(newBitmap, $"{TestFileName}.bmp");
        using Bitmap oldVersion = _imageHistoryService.RestoreVersion($"{TestFileName}.bmp", 0);
        {
            byte[] storedNewVersionByte = ImageToByte(storedNewVersion);
            byte[] oldVersionByte = ImageToByte(oldVersion);
            byte[] firstVersionByte = ImageToByte(testBitmap);
            
            Assert.That(oldVersionByte, Is.Not.EquivalentTo(storedNewVersionByte));
            Assert.That(oldVersionByte, Is.EquivalentTo(firstVersionByte));
        }
    }

    [TearDown]
    public void TearDown()
    {
        ClearTempImages();
    }
    
    private static byte[] ImageToByte(Image img)
    {
        byte[] bytes = null;
        
        using (var stream = new MemoryStream())
        {
            img.Save(stream, ImageFormat.Bmp);
            bytes = stream.ToArray();
        }
        
        return bytes;
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

    private static Bitmap PaintImage(Bitmap bitmap, Color color)
    {
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            Pen pen = new Pen(color);
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            graphics.DrawRectangle(pen, rect);
            graphics.Save(); 
        }
        
        return new Bitmap(bitmap);
    }
    
    private static Image GetBitmapWithoutLock(string path)
    {
        Image img;
            
        using (var bmpTemp = new Bitmap(path))
        {
            img = new Bitmap(bmpTemp);
        }
            
        return img;
    }
}