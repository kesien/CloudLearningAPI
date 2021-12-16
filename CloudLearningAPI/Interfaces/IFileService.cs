using CloudLearningAPI.Models;
using System.Threading.Tasks;

namespace CloudLearningAPI.Interfaces
{
    public interface IFileService
    {
        Task<string> GenerateImportFile(Course course, bool cloudlearning = false, bool homework = false, bool ebook = false);
        Task<string> GenerateWordDoc(Course course);
    }
}