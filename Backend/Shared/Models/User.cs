using EntityFrameworkCore.EncryptColumn.Attribute;

namespace Shared.Models
{
    public class User
    {
        public int Id { get; set; }
        public int ExternalId { get; set; }

        [EncryptColumn] public string DisplayName { get; set; }

        [EncryptColumn] public string UserPrincipalName { get; set; }

        public string? OfficeLocation { get; set; }

        [EncryptColumn] public string? JobTitle { get; set; }

        public int? Hash { get; set; }

        public List<Skill>? Skills { get; set; }

        public List<Solution>? Solutions { get; set; }

        public string Role { get; set; }
        [EncryptColumn] public string Password { get; set; }

        //public byte[]? ImageData { get; set; }

        //public int? ImageSize { get; set; }


        /// <summary>
        /// Compare all properties except Id and hash
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True or false</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var otherUser = (User)obj;

            return DisplayName == otherUser.DisplayName &&
                   UserPrincipalName == otherUser.UserPrincipalName &&
                   OfficeLocation == otherUser.OfficeLocation &&
                   //ImageSize == otherUser.ImageSize &&
                   JobTitle == otherUser.JobTitle;
            //SlackName == otherUser.SlackName &&
            //AccountEnabled == otherUser.AccountEnabled;

            // TODO KINGO: Add an active/inactive boolean flag here in above return
        }

        //public override int GetHashCode()
        //{
        //    // Generate hash code based on properties
        //    return HashCode.Combine(DisplayName, UserPrincipalName, OfficeLocation, JobTitle);
        //}
    }
}