using CloudLearningAPI.Models;

namespace CloudLearningAPI.Interfaces
{
    public interface IFileService
    {
        string GenerateImportFile(Course course, bool cloudlearning = false, bool homework = false, bool ebook = false);
        string GenerateWordDoc(Course course);
    }
}