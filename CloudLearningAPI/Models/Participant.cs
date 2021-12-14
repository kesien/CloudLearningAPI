namespace CloudLearningAPI.Models
{
    public class Participant
    {
        public string ID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Coursenumber { get; set; }
        public string Email { get; set; }

        public override string ToString()
        {
            return $"{Firstname} {Lastname}";
        }
    }
}
