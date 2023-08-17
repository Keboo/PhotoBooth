namespace PhotoBooth;

/// <summary>
/// Interaction logic for PhotoBoothWindow.xaml
/// </summary>
public partial class PhotoBoothWindow
{
    private PhotoBoothViewModel ViewModel { get; }

    public PhotoBoothWindow(PhotoBoothViewModel viewModel)
    {
        DataContext = ViewModel = viewModel;
        InitializeComponent();
        Loaded += PhotoBoothWindow_Loaded;
    }

    private void PhotoBoothWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (ViewModel.StartCameraCommand.CanExecute(null))
        {
            ViewModel.StartCameraCommand.Execute(null);
        }
    }
}
