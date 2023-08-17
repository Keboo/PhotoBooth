using System.IO;

using OpenCvSharp;

namespace PhotoBooth.Images;

public interface IImageService
{
    void SaveImage(Mat image);
}

public class ImageService : IImageService
{
    private int _imageNumber = 0;
    private FileInfo GetImageFile()
    {
        return new FileInfo(Path.GetFullPath(Path.Combine(".", "Images", $"Image{Interlocked.Increment(ref _imageNumber):0000}.png")));
    }

    public void SaveImage(Mat image)
    {
        FileInfo file = GetImageFile();
        file.Directory?.Create();
        image.SaveImage(file.FullName);
    }
}
