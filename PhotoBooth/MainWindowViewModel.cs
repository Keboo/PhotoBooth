using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using PhotoBooth.Cameras;

namespace PhotoBooth;

public partial class MainWindowViewModel : ObservableObject
{
    private IMessenger Messenger { get; }

    [ObservableProperty]
    private WriteableBitmap? _image;

    [ObservableProperty]
    private int _cameraIndex;

    private Camera _camera;

    partial void OnCameraIndexChanged(int value)
    {
        _camera?.Dispose();
        _camera = new(value);
    }

    public MainWindowViewModel(IMessenger messenger)
    {
        Messenger = messenger;
        _camera = new(0);
    }

    [RelayCommand]
    public void Start()
    {
        Messenger.Send(new ShowPhotoBoothMessage());
    }
}
