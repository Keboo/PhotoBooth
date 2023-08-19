using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

using PhotoBooth.Audio;
using PhotoBooth.Cameras;
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
            using Camera camera = new(1);
            while (!token.IsCancellationRequested)
            {
                Image = camera.GetImage(Image);
                
                if (Interlocked.CompareExchange(ref _takePicture, 0, 1) == 1 && Image is { } image)
                {
                    AudioPlayer.PlayCameraShutter();
                    ImageService.SaveImage(image.ToMat());
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
