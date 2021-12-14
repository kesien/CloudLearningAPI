using CloudLearningAPI.Interfaces;
using CloudLearningAPI.Models;
using CloudLearningAPI.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CloudLearningAPI.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly IDataParser _dataParser;
        private readonly SwitchClient _client;
        public CourseRepository(IDataParser dataParser, SwitchClient client)
        {
            _client = client;
            _dataParser = dataParser;
        }

        public async Task<Course> GetCourseData(string courseNumber)
        {
            var excelStream = await _client.DownloadListAsync(new List<string> { courseNumber });
            if (excelStream is null)
            {
                return null;
            }
            var parsedData = _dataParser.ParseData(excelStream);
            return parsedData.FirstOrDefault();
        }

        public async Task<List<Course>> GetMultipleCourseData(List<string> courseNumbers)
        {
            var excelStream = await _client.DownloadListAsync(courseNumbers);
            if (excelStream is null) 
            {
                return null;
            }

            return _dataParser.ParseData(excelStream);
        }
    }
}
