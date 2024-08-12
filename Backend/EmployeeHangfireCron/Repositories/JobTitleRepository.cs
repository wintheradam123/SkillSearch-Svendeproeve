using EmployeeHangfireCron.Algolia;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Shared;
using Shared.Helpers;

namespace GraphCronJob.Repositories
{
    public class JobTitleRepository
    {
        private readonly Context _context;

        public JobTitleRepository(Context context)
        {
            _context = context;
        }

        public async Task<List<JobTitle>> GetJobTitles()
        {
            return await _context.JobTitles.ToListAsync();
        }

        public async Task PostJobTitle(JobTitle jobTitle, AlgoliaSettings algoliaSettings)
        {
            try
            {
                _context.JobTitles.Add(jobTitle);
                await _context.SaveChangesAsync();

                var jobTitles = new List<JobTitle> { jobTitle };

                // Here we should batch these and send them to Algolia in chunks, but bc of time constraints we send them one at a time
                var algJobTitles = AlgoliaHelperJobTitles.TransformToAlgolia(jobTitles);
                await AlgoliaHelperJobTitles.Index(algJobTitles, algoliaSettings);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task DeleteJobTitle(int id, AlgoliaSettings algoliaSettings)
        {
            try
            {
                var jobTitle = await _context.JobTitles.FindAsync(id);
                if (jobTitle == null)
                {
                    return;
                }

                _context.JobTitles.Remove(jobTitle);
                await _context.SaveChangesAsync();

                List<string> idsToDelete = new() { id.ToString() };

                await AlgoliaHelperJobTitles.Delete(idsToDelete, algoliaSettings);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
