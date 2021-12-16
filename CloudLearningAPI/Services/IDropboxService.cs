using System.IO;
using System.Threading.Tasks;

namespace CloudLearningAPI.Services
{
    public interface IDropboxService
    {
        Task<byte[]> GetFile(string file);
        Task<bool> UploadFile(string file, MemoryStream fileStream);
    }
}