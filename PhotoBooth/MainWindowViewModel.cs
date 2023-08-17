using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace PhotoBooth;

public partial class MainWindowViewModel : ObservableObject
{
    private IMessenger Messenger { get; }

    [ObservableProperty]
    private WriteableBitmap? _image;
    
    public MainWindowViewModel(IMessenger messenger)
    {
        Messenger = messenger;
    }

    [RelayCommand]
    public void Start()
    {
        Messenger.Send(new ShowPhotoBoothMessage());
    }
}
