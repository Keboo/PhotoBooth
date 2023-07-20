using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

namespace PhotoBooth;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private WriteableBitmap? _image;
    
    public MainWindowViewModel()
    {
    }

    //This is using the source generators from CommunityToolkit.Mvvm to generate a RelayCommand
    //See: https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/generators/relaycommand
    //and: https://learn.microsoft.com/windows/communitytoolkit/mvvm/relaycommand
    [RelayCommand(IncludeCancelCommand = true)]
    private async Task StartCamera(CancellationToken token)
    {
        try
        {
            using VideoCapture capture = VideoCapture.FromCamera(1);
            capture.FrameWidth = 1920;
            capture.FrameHeight = 1080;
            using Mat image = new();
            while (!token.IsCancellationRequested)
            {
                capture.Read(image);
                if (image.Empty())
                    break;

                if (Image is null)
                {
                    Image = image.ToWriteableBitmap();
                }
                else
                {
                    WriteableBitmapConverter.ToWriteableBitmap(image, Image);
                }

                await Task.Delay(10, token);
            }
        }
        catch(Exception e)
        {

        }
    }
}
