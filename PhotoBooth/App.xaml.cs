using System.Windows;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.Messaging;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PhotoBooth.Audio;
using PhotoBooth.Images;

namespace PhotoBooth;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application, IRecipient<ShowPhotoBoothMessage>
{
    private Func<PhotoBoothOptions, PhotoBoothWindow> PhotoBoothWindowFactory { get; }

    public App(Func<PhotoBoothOptions, PhotoBoothWindow> photoBoothWindowFactory, IMessenger messenger)
    {
        messenger.RegisterAll(this);
        PhotoBoothWindowFactory = photoBoothWindowFactory;
    }

    void IRecipient<ShowPhotoBoothMessage>.Receive(ShowPhotoBoothMessage message)
    {
        if (Windows.OfType<PhotoBoothWindow>().FirstOrDefault() is { } existingWindow)
        {
            existingWindow.Activate();
        }
        else
        {
            PhotoBoothWindow window = PhotoBoothWindowFactory(message.Options);
            window.Show();
        }
    }

    [STAThread]
    public static void Main(string[] args)
    {
        using IHost host = CreateHostBuilder(args).Build();
        host.Start();

        App app = host.Services.GetRequiredService<App>();
        app.InitializeComponent();
        app.MainWindow = host.Services.GetRequiredService<MainWindow>();
        app.MainWindow.Visibility = Visibility.Visible;
        app.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder)
            => configurationBuilder.AddUserSecrets(typeof(App).Assembly))
        .ConfigureServices((hostContext, services) =>
        {
            services.AddSingleton<App>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();

            services.AddSingleton<Func<PhotoBoothOptions, PhotoBoothWindow>>(provider => (options) => 
            {
                PhotoBoothViewModel vm = new(options,
                    provider.GetRequiredService<IImageService>(),
                    provider.GetRequiredService<IAudioPlayer>());

                return new PhotoBoothWindow(vm);
            });
            services.AddTransient<PhotoBoothWindow>();
            services.AddTransient<PhotoBoothViewModel>();
            services.AddSingleton<PhotoBoothOptions>();

            services.AddSingleton<IImageService, ImageService>();
            services.AddSingleton<IAudioPlayer, AudioPlayer>();

            services.AddSingleton<WeakReferenceMessenger>();
            services.AddSingleton<IMessenger, WeakReferenceMessenger>(provider => provider.GetRequiredService<WeakReferenceMessenger>());

            services.AddSingleton(_ => Current.Dispatcher);

            services.AddTransient<ISnackbarMessageQueue>(provider =>
            {
                Dispatcher dispatcher = provider.GetRequiredService<Dispatcher>();
                return new SnackbarMessageQueue(TimeSpan.FromSeconds(3.0), dispatcher);
            });
        });
}
