namespace AWN.Services
{
    public interface IMediaSerivce
    {
        Task<string> AddAsync(IFormFile media);
        Task DeleteAsync(string url);
        Task<string> UpdateAsync(string oldUrl, IFormFile newMedia);
    }
}