using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using MockupGeneratorLinux.Abstractions;
using Microsoft.Extensions.Options;
using MockupGeneratorLinux.Settings;

namespace MockupGeneratorLinux.Services
{
    public class ImageProcessor:IImageProcessor
    {
        private readonly PathSettings _paths;
        private readonly IGreenFinder _greenFinder;
        private readonly IResizer _resizer;
        private readonly Random _rng;

        public ImageProcessor(IOptions<PathSettings> options, IGreenFinder greenFinder, IResizer resizer)
        {
            _paths = options.Value;
            _greenFinder = greenFinder;
            _resizer = resizer;
            _rng = new Random();
        }

        public Task<string?> GetNextFileAsync()
        {
            var nextFile = Directory
                .GetFiles(_paths.InputFolder)
                .FirstOrDefault(f =>
                    f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    f.EndsWith(".png", StringComparison.OrdinalIgnoreCase));

            return Task.FromResult<string?>(nextFile is null
                ? null
                : Path.GetFileName(nextFile));
        }

        public async Task ProcessAsync(string fileName)
        {
            var inputPath = Path.Combine(_paths.InputFolder, fileName);
            var backupPath = Path.Combine(_paths.BackupFolder, fileName);
            
            File.Copy(inputPath, backupPath, overwrite: true);
            Console.WriteLine($"Backup сохранён: {backupPath}");

            string templateSubDir = _rng.Next(1, 11).ToString("D3");
            string templateDir = Path.Combine(_paths.TemplatesFolder, templateSubDir);

            if (!Directory.Exists(templateDir))
            {
                Console.WriteLine($"Папка шаблонов не найдена: {templateDir}");
                return;
            }

            var templateFiles = Directory.EnumerateFiles(templateDir)
                .Where(f =>
                    f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                    f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!templateFiles.Any())
            {
                Console.WriteLine($"Нет файлов-шаблонов в папке: {templateDir}");
                return;
            }

            var baseName = Path.GetFileNameWithoutExtension(fileName);
            var resultDir = Path.Combine(_paths.OutputFolder, baseName);
            Directory.CreateDirectory(resultDir);

            foreach (var templatePath in templateFiles)
            {
                using var template = Image.Load<Rgba32>(templatePath);
                using var product = Image.Load<Rgba32>(inputPath);
                
                var zone = _greenFinder.FindGreenZone(template);
                var resizedProduct = _resizer.ResizeImage(product, zone.Width, zone.Height);

                template.Mutate(ctx =>
                    ctx.DrawImage(resizedProduct, new Point(zone.X, zone.Y), opacity: 1f)
                );

                var tplBase = Path.GetFileNameWithoutExtension(templatePath);
                var prodBase = Path.GetFileNameWithoutExtension(fileName);
                var ext = Path.GetExtension(templatePath);
                var outputName = $"{prodBase}{tplBase}{ext}";
                var outputPath = Path.Combine(resultDir, outputName);

                await template.SaveAsync(outputPath);
                Console.WriteLine($"Сгенерирован макет: {outputPath}");
            }
            //File.Delete(inputPath);
        }


    }
}