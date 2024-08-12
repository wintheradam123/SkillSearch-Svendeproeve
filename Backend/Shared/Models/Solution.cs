using EntityFrameworkCore.EncryptColumn.Attribute;

namespace Shared.Models
{
    public class Solution
    {
        public int Id { get; set; }

        [EncryptColumn] public string Title { get; set; }

        public string? Link { get; set; }
        public List<User>? Users { get; set; }
    }
}