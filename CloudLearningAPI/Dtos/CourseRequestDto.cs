using System.ComponentModel.DataAnnotations;

namespace CloudLearningAPI.Dtos
{
    public class CourseRequestDto
    {
        [Required]
        [MaxLength(5)]
        [MinLength(5)]
        public string Coursenumber { get; set; }
        public bool Cloudlearning { get; set; }
        public bool Homework { get; set; }
        public bool Ebook { get; set; }
    }
}
