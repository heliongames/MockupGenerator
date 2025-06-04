using MockupGeneratorLinux.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace MockupGeneratorLinux.Services
{
    public class GreenFinder:IGreenFinder
    {
        private static readonly Rgba32 TargetGreen = new Rgba32(0, 255, 0);

        public Rectangle FindGreenZone(Image<Rgba32> image)
        {
            int minX = image.Width, minY = image.Height;
            int maxX = 0, maxY = 0;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    if (image[x, y].Equals(TargetGreen))
                    {
                        if (x < minX) minX = x;
                        if (y < minY) minY = y;
                        if (x > maxX) maxX = x;
                        if (y > maxY) maxY = y;
                    }
                }
            }

            minX = Math.Max(minX - 1, 0);
            minY = Math.Max(minY - 1, 0);
            maxX = Math.Min(maxX + 2, image.Width - 1);
            maxY = Math.Min(maxY + 2, image.Height - 1);

            return Rectangle.FromLTRB(minX, minY, maxX, maxY);
        }

    }
}