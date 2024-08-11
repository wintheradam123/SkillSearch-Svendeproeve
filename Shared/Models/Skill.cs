namespace Shared.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public List<User>? Users { get; set; }
    }
}