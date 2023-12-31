﻿using System.Windows;
using System.Windows.Media.Imaging;

using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

using Size = OpenCvSharp.Size;

namespace PhotoBooth.Cameras;

public sealed class Camera : IDisposable
{
    private bool _disposedValue;

    private Lazy<VideoCapture> CaptureDevice { get; }
    private readonly Mat _cameraImage = new();

    public Camera(int index)
    {
        Index = index;
        CaptureDevice = new(() =>
        {
            var rv = VideoCapture.FromCamera(index);
            rv.FrameWidth = Resolution.Width;
            rv.FrameHeight = Resolution.Height;
            return rv;
        });
    }

    public int Index { get; }
    public Size Resolution { get; set; } = new(1920, 1080);

    public WriteableBitmap? GetImage(WriteableBitmap? image)
    {
        if (_disposedValue) return null;
        try
        {
            CaptureDevice.Value.Read(_cameraImage);
            if (_cameraImage.Empty())
                return image;

            if (image is null)
            {
                return Application.Current.Dispatcher.Invoke(() => _cameraImage.ToWriteableBitmap());
            }
            else
            {
                image.Dispatcher.Invoke(() =>
                {
                    WriteableBitmapConverter.ToWriteableBitmap(_cameraImage, image);
                });
                return image;
            }
        }
        catch(ObjectDisposedException) when (_disposedValue)
        {
            //Suppress as Read will hang
            return null;
        }
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                if (CaptureDevice.IsValueCreated)
                {
                    CaptureDevice.Value.Dispose();
                }
                _cameraImage.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

