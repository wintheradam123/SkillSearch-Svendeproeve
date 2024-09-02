using Algolia.Search.Clients;
using Algolia.Search.Http;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeHangfireCron.Algolia;

namespace Shared.Helpers
{
    public static class AlgoliaHelperSkills
    {
        public const string IndexName = "dev_SkillSearch_Skills_Nicky";

        public static async Task Index(IEnumerable<AlgoliaSkill> skills, AlgoliaSettings settings)
        {
            var client = new SearchClient(settings.ApplicationId, settings.WriteApiKey);
            var index = client.InitIndex(IndexName);

            await index.SaveObjectsAsync(skills);
        }

        public static async Task PartialUpdate(IEnumerable<AlgoliaSkill> skills, AlgoliaSettings settings)
        {
            var client = new SearchClient(settings.ApplicationId, settings.WriteApiKey);
            var index = client.InitIndex(IndexName);

            // TODO: Set autoGenerateObjectIDIfNotExist to true
            await index.PartialUpdateObjectsAsync(skills, new RequestOptions());
        }

        public static async Task Delete(IEnumerable<string> objectIds, AlgoliaSettings settings)
        {
            var client = new SearchClient(settings.ApplicationId, settings.WriteApiKey);
            var index = client.InitIndex(IndexName);

            // TODO: Set autoGenerateObjectIDIfNotExist to true
            await index.DeleteObjectsAsync(objectIds, new RequestOptions());
        }

        public static IEnumerable<AlgoliaSkill> TransformToAlgolia(IEnumerable<Skill> skills)
        {
            var algoliaSkills = skills.Select(skill => new AlgoliaSkill
            {
                ObjectID = skill.Id.ToString(),
                Tag = skill.Title,
                Category = skill.Category,
            }).ToList();

            return algoliaSkills;
        }
    }
}
