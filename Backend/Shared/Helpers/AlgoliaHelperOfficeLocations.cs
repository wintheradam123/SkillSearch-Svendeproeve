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
    public static class AlgoliaHelperOfficeLocations
    {
        public const string IndexName = "dev_SkillSearch_OfficeLocations";

        public static async Task Index(IEnumerable<AlgoliaOfficeLocation> officeLocations, AlgoliaSettings settings)
        {
            var client = new SearchClient(settings.ApplicationId, settings.WriteApiKey);
            var index = client.InitIndex(IndexName);

            await index.SaveObjectsAsync(officeLocations);
        }

        public static async Task PartialUpdate(IEnumerable<AlgoliaOfficeLocation> officeLocations,
            AlgoliaSettings settings)
        {
            var client = new SearchClient(settings.ApplicationId, settings.WriteApiKey);
            var index = client.InitIndex(IndexName);

            // TODO: Set autoGenerateObjectIDIfNotExist to true
            await index.PartialUpdateObjectsAsync(officeLocations, new RequestOptions());
        }

        public static async Task Delete(IEnumerable<string> objectIds, AlgoliaSettings settings)
        {
            var client = new SearchClient(settings.ApplicationId, settings.WriteApiKey);
            var index = client.InitIndex(IndexName);

            // TODO: Set autoGenerateObjectIDIfNotExist to true
            await index.DeleteObjectsAsync(objectIds, new RequestOptions());
        }

        public static IEnumerable<AlgoliaOfficeLocation> TransformToAlgolia(IEnumerable<OfficeLocation> officeLocations)
        {
            var algoliaOfficeLocation = officeLocations.Select(officeLocation => new AlgoliaOfficeLocation
            {
                ObjectID = officeLocation.Id.ToString(),
                LocationName = officeLocation.LocationName
            }).ToList();

            return algoliaOfficeLocation;
        }
    }
}