using GraphCronJob.Jobs;
using GraphCronJob.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GraphCronJob.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraphUserController : ControllerBase
    {
        private readonly UserController _userController;
        private readonly OfficeLocationRepository _officeLocationRepository;
        private readonly JobTitleRepository _jobTitleRepository;

        public GraphUserController(UserController userController,
            OfficeLocationRepository officeLocationRepository,
            JobTitleRepository jobTitleRepository)
        {
            _userController = userController;
            _officeLocationRepository = officeLocationRepository;
            _jobTitleRepository = jobTitleRepository;
        }

        [HttpGet("RunGraphJob")]
        public async Task<IActionResult> RunCronJob()
        {
            var graphJobs = new CronJobs(_userController, _officeLocationRepository, _jobTitleRepository);
            await graphJobs.RunTask();

            return Ok();
        }
    }
}