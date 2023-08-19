using System.Windows.Media.Imaging;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using PhotoBooth.Cameras;

namespace PhotoBooth;

public partial class MainWindowViewModel : ObservableObject
{
    private IMessenger Messenger { get; }

    [ObservableProperty]
    private WriteableBitmap? _previewImage;

    [ObservableProperty]
    private int _cameraIndex;

    private Camera _camera;

    private readonly Timer _timer;

    partial void OnCameraIndexChanged(int value)
    {
        //_camera?.Dispose();
        //_camera = new(value);
    }

    public MainWindowViewModel(IMessenger messenger)
    {
        Messenger = messenger;
        //_camera = new(CameraIndex);
        //_timer = new(OnTimerTick, null, 0, Timeout.Infinite);
    }

    private void OnTimerTick(object? _)
    {
        //_timer.Change(Timeout.Infinite, Timeout.Infinite);
        //PreviewImage = _camera?.GetImage(PreviewImage);
        //_timer.Change(1_000, Timeout.Infinite);
    }

    [RelayCommand]
    public void Start()
    {
        PhotoBoothOptions options = new(CameraIndex);
        Messenger.Send(new ShowPhotoBoothMessage(options));
    }
}
