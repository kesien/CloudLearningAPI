using CsvHelper.Configuration.Attributes;

namespace CloudLearningAPI.Models
{
    public class UserImport
    {
        [Name("username")]
        public string Username { get; set; } 
        [Name("password")]
        public string Password { get; set; } = "PW4you";
        [Name("email")]
        public string Email { get; set; }
        [Name("firstname")]
        public string Firstname { get; set; }
        [Name("lastname")]
        public string Lastname { get; set; }
        [Name("cohort1")]
        public string Cohort { get; set; }
        [Name("course2")]
        public string Homework { get; set; }
        [Name("course1")]
        public string Ebook { get; set; }
        [Name("group1")]
        public string Group { get; set; }
    }
}