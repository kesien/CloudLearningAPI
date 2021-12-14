using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CloudLearningAPI.Dtos;
using CloudLearningAPI.Interfaces;
using CloudLearningAPI.Repositories;
using CloudLearningAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudLearningAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SwitchController : ControllerBase
    {
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;
        private readonly SwitchClient _client;
        private readonly ICourseRepository _repository;
        private readonly IFileService _fileService;

        public SwitchController(IJwtAuthenticationManager jwtAuthenticationManager, SwitchClient client, ICourseRepository repository, IFileService fileService)
        {
            _fileService = fileService;
            _jwtAuthenticationManager = jwtAuthenticationManager;
            _client = client;
            _repository = repository;
        }

        [HttpPost("courses")]
        public async Task<ActionResult<List<CourseDto>>> GetCourses(List<CourseRequestDto> courses)
        {
            var coursenumbers = courses.Select(courserequest => courserequest.Coursenumber).ToList();
            var courseData = await _repository.GetMultipleCourseData(coursenumbers);
            if (courseData is null)
            {
                return BadRequest();
            }
            var result = new List<CourseDto>();
            foreach (var course in courseData)
            {
                var courseDto = courses.Where(c => c.Coursenumber == course.Coursenumber).FirstOrDefault();
                if (courseDto is null)
                {
                    continue;
                }
                var importFile = (courseDto.Homework || courseDto.Cloudlearning || courseDto.Ebook) ? _fileService.GenerateImportFile(course, courseDto.Cloudlearning, courseDto.Homework, courseDto.Ebook) : null;
                var wordFile = (courseDto.Homework || courseDto.Ebook) ? _fileService.GenerateWordDoc(course) : null;
                result.Add(new CourseDto 
                {
                    Id = Guid.NewGuid(),
                    Coursenumber = course.Coursenumber,
                    Language = course.Language,
                    Level = course.Level,
                    Participants = course.Participants,
                    WordFilePath = wordFile is not null ? $"word/{wordFile}" : null,
                    ImportFilePath = importFile is not null ? $"import/{importFile}" : null
                });
            }
            return result;
        }

        [HttpPost("course")]
        public async Task<ActionResult<CourseDto>> GetCourse(CourseRequestDto course)
        {
            var courseData = await _repository.GetCourseData(course.Coursenumber);
            if (courseData is null)
            {
                return BadRequest();
            }

            var importFile = (course.Homework || course.Cloudlearning || course.Ebook) ? _fileService.GenerateImportFile(courseData, course.Cloudlearning, course.Homework, course.Ebook) : null;
            var wordFile = (course.Homework || course.Ebook) ? _fileService.GenerateWordDoc(courseData) : null;
            return new CourseDto 
            {
                Id = Guid.NewGuid(),
                Coursenumber = courseData.Coursenumber,
                Language = courseData.Language,
                Level = courseData.Level,
                Participants = courseData.Participants,
                WordFilePath = wordFile is not null ? $"word/{wordFile}" : null,
                ImportFilePath = importFile is not null ? $"import/{importFile}" : null
            };
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Authenticate(UserDto user)
        {
            if (!await _client.LoginAsync(user.username, user.password))
            {
                return Unauthorized();
            }

            var token = _jwtAuthenticationManager.Authenticate(user.username);
            return Ok(new { Token = token });
        }
    }
}
