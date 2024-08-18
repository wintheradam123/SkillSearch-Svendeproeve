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
    public static class AlgoliaHelperSolutions
    {
        public const string IndexName = "dev_SkillSearch_Solutions";

        public static async Task Index(IEnumerable<AlgoliaSolution> solutions, AlgoliaSettings settings)
        {
            var client = new SearchClient(settings.ApplicationId, settings.WriteApiKey);
            var index = client.InitIndex(IndexName);

            await index.SaveObjectsAsync(solutions);
        }

        public static async Task PartialUpdate(IEnumerable<AlgoliaSolution> solutions, AlgoliaSettings settings)
        {
            var client = new SearchClient(settings.ApplicationId, settings.WriteApiKey);
            var index = client.InitIndex(IndexName);

            // TODO: Set autoGenerateObjectIDIfNotExist to true
            await index.PartialUpdateObjectsAsync(solutions, new RequestOptions());
        }

        public static IEnumerable<AlgoliaSolution> TransformToAlgolia(IEnumerable<Solution> solutions)
        {
            var algoliaSolutions = solutions.Select(solution => new AlgoliaSolution
            {
                ObjectID = solution.Id.ToString(),
                SolutionName = solution.Title,
                Link = solution.Link
            }).ToList();

            return algoliaSolutions;
        }

        public static async Task Delete(IEnumerable<string> idsToDelete, AlgoliaSettings settings)
        {
            var client = new SearchClient(settings.ApplicationId, settings.WriteApiKey);
            var index = client.InitIndex(IndexName);

            // TODO: Set autoGenerateObjectIDIfNotExist to true
            await index.DeleteObjectsAsync(idsToDelete, new RequestOptions());
        }
    }
}
