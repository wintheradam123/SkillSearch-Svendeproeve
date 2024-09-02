using Algolia.Search.Clients;
using Algolia.Search.Http;
using EmployeeHangfireCron.Algolia;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Helpers
{
    public class AlgoliaHelperJobTitles
    {
        public const string IndexName = "dev_SkillSearch_Jobtitles_Nicky";

        public static async Task Index(IEnumerable<AlgoliaJobTitle> jobTitles, AlgoliaSettings settings)
        {
            var client = new SearchClient(settings.ApplicationId, settings.WriteApiKey);
            var index = client.InitIndex(IndexName);

            await index.SaveObjectsAsync(jobTitles);
        }

        public static async Task PartialUpdate(IEnumerable<AlgoliaJobTitle> jobTitles, AlgoliaSettings settings)
        {
            var client = new SearchClient(settings.ApplicationId, settings.WriteApiKey);
            var index = client.InitIndex(IndexName);

            // TODO: Set autoGenerateObjectIDIfNotExist to true
            await index.PartialUpdateObjectsAsync(jobTitles, new RequestOptions());
        }

        public static async Task Delete(IEnumerable<string> objectIds, AlgoliaSettings settings)
        {
            var client = new SearchClient(settings.ApplicationId, settings.WriteApiKey);
            var index = client.InitIndex(IndexName);

            // TODO: Set autoGenerateObjectIDIfNotExist to true
            await index.DeleteObjectsAsync(objectIds, new RequestOptions());
        }

        public static IEnumerable<AlgoliaJobTitle> TransformToAlgolia(IEnumerable<JobTitle> jobTitles)
        {
            var algoliaJobTitles = jobTitles.Select(jobTitle => new AlgoliaJobTitle
            {
                ObjectID = jobTitle.Id.ToString(),
                Title = jobTitle.Title
            }).ToList();

            return algoliaJobTitles;
        }
    }
}