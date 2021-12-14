using System.Collections.Generic;
using System.Threading.Tasks;
using CloudLearningAPI.Models;

namespace CloudLearningAPI.Repositories
{
    public interface ICourseRepository
    {
        Task<Course> GetCourseData(string courseNumber);
        Task<List<Course>> GetMultipleCourseData(List<string> courseNumbers);
    }
}