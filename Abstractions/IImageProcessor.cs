namespace MockupGeneratorLinux.Abstractions
{
    public interface IImageProcessor
    {
        Task ProcessAsync(string fileName);
        Task<string?> GetNextFileAsync();
    }
}

