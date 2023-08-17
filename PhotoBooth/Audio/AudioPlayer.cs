using System.Media;
using System.Reflection;

namespace PhotoBooth.Audio;

public interface IAudioPlayer
{
    void PlayCameraShutter();
}

public class AudioPlayer : IAudioPlayer
{
    private readonly Lazy<SoundPlayer> _soundPlayer;

    public AudioPlayer()
    {
        _soundPlayer = new(() =>
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Where(x => x.EndsWith("camera.wav")).Single();
            return new SoundPlayer(assembly.GetManifestResourceStream(resourceName));
        });
    }

    public void PlayCameraShutter()
    {
        _soundPlayer.Value.Play();
    }
}
