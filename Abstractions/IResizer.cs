using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace MockupGeneratorLinux.Abstractions
{
    public interface IResizer
    {
        Image<Rgba32> ResizeImage(Image<Rgba32> image, int width, int height);
    }
}
