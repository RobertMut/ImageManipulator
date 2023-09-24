using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.CQRS.Interfaces;

namespace ImageManipulator.Application.Common.CQRS.Command.AddImageVersion;

public class AddImageVersionCommandHandler : ICommandHandler<AddImageVersionCommand, Bitmap>
{
    private readonly IImageHistoryService _imageHistoryService;

    public AddImageVersionCommandHandler(IImageHistoryService imageHistoryService)
    {
        _imageHistoryService = imageHistoryService;
    }

    public async Task<Bitmap> Handle(AddImageVersionCommand command, CancellationToken cancellationToken)
    {
        return await _imageHistoryService.StoreCurrentVersionAndGetThumbnail(command.Image, command.Path);
    }
}