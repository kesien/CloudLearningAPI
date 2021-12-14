using System.Collections.Generic;
using System.IO;
using CloudLearningAPI.Models;

namespace CloudLearningAPI.Interfaces
{
    public interface IDataParser
    {
        List<Course> ParseData(Stream dataStream);
    }
}