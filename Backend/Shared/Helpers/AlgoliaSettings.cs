using Microsoft.Extensions.Configuration;

namespace EmployeeHangfireCron.Algolia
{
    public class AlgoliaSettings
    {
        public string? ApplicationId { get; set; }
        public string? WriteApiKey { get; set; }
        public string? Index { get; set; }
    }
}
