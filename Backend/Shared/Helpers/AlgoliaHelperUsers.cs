using Shared.Models;
using Algolia.Search.Clients;
using Algolia.Search.Http;
using System;
using EmployeeHangfireCron.Algolia;

namespace Shared.Helpers
{
    public static class AlgoliaHelperUsers
    {
        public const string IndexName = "dev_SkillSearch_Users";

        public static async Task Index(IEnumerable<AlgoliaUser> users, AlgoliaSettings settings)
        {
            var client = new SearchClient(settings.ApplicationId, settings.WriteApiKey);
            var index = client.InitIndex(IndexName);

            await index.SaveObjectsAsync(users);
        }

        public static async Task Delete(IEnumerable<string> objectIds, AlgoliaSettings settings)
        {
            var client = new SearchClient(settings.ApplicationId, settings.WriteApiKey);
            var index = client.InitIndex(IndexName);

            await index.DeleteObjectsAsync(objectIds);
        }

        public static async Task PartialUpdate(IEnumerable<AlgoliaUser> users, AlgoliaSettings settings)
        {
            var client = new SearchClient(settings.ApplicationId, settings.WriteApiKey);
            var index = client.InitIndex(IndexName);

            // TODO: Set autoGenerateObjectIDIfNotExist to true
            await index.PartialUpdateObjectsAsync(users, new RequestOptions());
        }

        public static IEnumerable<AlgoliaUser> TransformToAlgolia(IEnumerable<User> users)
        {
            var algoliaUsers = users.Select(user => new AlgoliaUser
            {
                ObjectID = user.ExternalId.ToString(),
                DisplayName = user.DisplayName,
                Solutions = user.Solutions?.Select(solution => new AlgoliaSolution()
                {
                    SolutionName = solution.Title
                }).ToList() ?? new List<AlgoliaSolution>(),
                JobTitle = user.JobTitle,
                OfficeLocation = user.OfficeLocation,
                Skills = user.Skills?.Select(skill => new AlgoliaSkill
                {
                    Tag = skill.Title
                }).ToList() ?? new List<AlgoliaSkill>(),
                UserPrincipalName = user.UserPrincipalName,
                //HasImage = user.ImageSize != null && user.ImageSize != 0,
                Role = user.Role
            }).ToList();

            return algoliaUsers;
        }
    }
}