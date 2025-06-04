using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace MockupGeneratorLinux.Abstractions
{
    public interface IGreenFinder
    {
        Rectangle FindGreenZone(Image<Rgba32> template);
    }
}
