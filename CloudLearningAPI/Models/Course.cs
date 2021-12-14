using System.Collections.Generic;

namespace CloudLearningAPI.Models
{
    public class Course
    {
        public string Coursenumber { get; set; }
        public string Language { get; set; }
        public string Level { get; set; }
        public List<Participant> Participants { get; set; }
        public override string ToString()
        {
            return $"{Coursenumber} - {Language} / {Level}\nParticipants: {Participants.Count}";
        }
    }
}
