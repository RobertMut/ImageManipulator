using System.Threading.Tasks;

namespace ImageManipulator.Application.Common.Interfaces
{
    public interface ICommonDialogService
    {
        Task<string[]> ShowFileDialogInNewWindow();
    }
}
