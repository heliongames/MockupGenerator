using MockupGeneratorLinux.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace MockupGeneratorLinux.Services
{
    public class Resizer : IResizer
    {
        public Image<Rgba32> ResizeImage(Image<Rgba32> image, int width, int height)
        {
            return image.Clone(ctx => ctx.Resize(width, height));
        }
    }
}
