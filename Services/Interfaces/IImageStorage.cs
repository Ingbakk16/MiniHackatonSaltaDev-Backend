namespace AnimalesPerdidos.Services.Interfaces
{
    public interface IImageStorage
    {
        Task<List<string>> UploadAsync(IEnumerable<IFormFile> files, CancellationToken ct);
    }
}
