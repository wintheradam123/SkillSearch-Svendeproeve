namespace Shared.Models
{
    public class AlgoliaUser
    {
        public string ObjectID { get; set; }
        public string DisplayName { get; set; }
        public string UserPrincipalName { get; set; }
        public string? OfficeLocation { get; set; }
        public string? JobTitle { get; set; }
        //public bool HasImage { get; set; }

        // TODO KINGO: Add an active/inactive boolean flag here

        //public string? ImageUrl { get; set; } // TODO: Do we need this field in Algolia?
        public List<AlgoliaSkill> Skills { get; set; }
        public List<AlgoliaSolution> Solutions { get; set; }
        public string Role { get; set; }
    }
}