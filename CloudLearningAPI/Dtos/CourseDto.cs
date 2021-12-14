using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudLearningAPI.Models;

namespace CloudLearningAPI.Dtos
{
    public class CourseDto
    {
        public Guid Id { get; set; }
        public string Coursenumber { get; set; }
        public string Language { get; set; }
        public string Level { get; set; }
        public List<Participant> Participants { get; set; }
        public string WordFilePath { get; set; }
        public string ImportFilePath { get; set; }
    }
}
