using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

using PhotoBooth.Audio;
using PhotoBooth.Images;

namespace PhotoBooth;

public partial class PhotoBoothViewModel : ObservableObject
{
    [ObservableProperty]
    private WriteableBitmap? _image;

    private int _takePicture;

    private IImageService ImageService { get; }
    private IAudioPlayer AudioPlayer { get; }

    public PhotoBoothViewModel(IImageService imageService, IAudioPlayer audioPlayer)
    {
        ImageService = imageService;
        AudioPlayer = audioPlayer;
    }

    [RelayCommand]
    private void TakePicture()
    {
        Interlocked.Exchange(ref _takePicture, 1);
    }

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
                if (Interlocked.CompareExchange(ref _takePicture, 0, 1) == 1)
                {
                    AudioPlayer.PlayCameraShutter();
                    ImageService.SaveImage(image);
                    await Task.Delay(TimeSpan.FromSeconds(1), token);
                    continue;
                }
                else
                {
                    await Task.Delay(10, token);
                }
            }
        }
        catch(OperationCanceledException)
        { }
        catch (Exception)
        { }
    }
}
