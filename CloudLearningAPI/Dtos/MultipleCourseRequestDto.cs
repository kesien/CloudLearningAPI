using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CloudLearningAPI.Dtos
{
    public class MultipleCourseRequestDto
    {
        [Required]
        public List<CourseRequestDto> Coursenumbers { get; set; }
        public bool Cloudlearning { get; set; }
        public bool Homework { get; set; }
        public bool Ebook { get; set; }
    }
}
