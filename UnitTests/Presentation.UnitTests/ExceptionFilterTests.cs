using System.Diagnostics.CodeAnalysis;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Presentation.Filters;
using Moq;

namespace Presentation.UnitTests;

[TestFixture]
[ExcludeFromCodeCoverage]
public class ExceptionFilterTests
{
    private Mock<ICommonDialogService> commonDialogServiceMock;
    private ExceptionFilter _exceptionFilter;
    
    [SetUp]
    public void Setup()
    {
        commonDialogServiceMock = new Mock<ICommonDialogService>();
        commonDialogServiceMock.Setup(x => x.ShowException(It.IsAny<string>())).Returns(Task.CompletedTask);
        _exceptionFilter = new ExceptionFilter(commonDialogServiceMock.Object);
    }

    [Test]
    public void ExceptionFilterHandlesUnknownException()
    {
        _exceptionFilter.OnNext(new ApplicationException("UnknownException", new Exception("ex")));
        commonDialogServiceMock.Verify(x => x.ShowException(It.IsAny<string>()), Times.Once);
    }
    
    [Test]
    public void ExceptionFilterHandlesNullReferenceException()
    {
        _exceptionFilter.OnNext(new NullReferenceException("Ex"));
        commonDialogServiceMock.Verify(x => x.ShowException(It.IsAny<string>()), Times.Once);
    }
    
    [Test]
    public void ExceptionFilterHandlesIOException()
    {
        _exceptionFilter.OnNext(new IOException("Ex"));
        commonDialogServiceMock.Verify(x => x.ShowException(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void ExceptionFilterOnCompleted()
    {
        Assert.DoesNotThrow(_exceptionFilter.OnCompleted);
    }

    [Test]
    public void ExceptionFilterOnErrorThrows()
    {
        Assert.Throws<Exception>(() => _exceptionFilter.OnError(new Exception("ex")));
    }
}