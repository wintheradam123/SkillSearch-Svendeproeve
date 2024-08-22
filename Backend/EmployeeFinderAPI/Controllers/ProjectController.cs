using EmployeeHangfireCron.Algolia;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Helpers;
using Shared.Models;

namespace SkillSearchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")] // Apply CORS policy
    public class ProjectController : ControllerBase
    {
        private readonly Context _context;
        private readonly AlgoliaSettings _algoliaSettingsProjects;
        private readonly AlgoliaSettings _algoliaSettingsUsers;

        public ProjectController(Context context)
        {
            _context = context;
            _algoliaSettingsProjects = AlgoliaHelper.LoadAlgoliaSettingsProjects();
            _algoliaSettingsUsers = AlgoliaHelper.LoadAlgoliaSettingsUsers();
        }

        // GET: api/Skill
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Solution>>> GetSolutions()
        {
            try
            {
                return await _context.Solutions
                    .OrderBy(on => on.Title)
                    .Select(s => new Solution()
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Users = s.Users
                    })
                    .ToListAsync();
            }
            catch (Exception e)
            {
                return BadRequest("Error occurred while retrieving solutions: " + e.Message);
            }
        }

        // GET: api/Skill/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Solution>> GetSolution(int id)
        {
            try
            {
                var solutions = await _context.Solutions.FindAsync(id);

                if (solutions == null)
                {
                    return NotFound();
                }

                return solutions;
            }
            catch (Exception e)
            {
                return BadRequest("Error occurred while retrieving solution: " + e.Message);
            }
        }

        [HttpGet("IndexAllSolutions")]
        public async Task<IActionResult> IndexAllSolutions()
        {
            var solutions = await _context.Solutions.ToListAsync();

            var algoliaSolutions = AlgoliaHelperSolutions.TransformToAlgolia(solutions);

            await AlgoliaHelperSolutions.Index(algoliaSolutions, _algoliaSettingsProjects);

            return Ok();
        }

        // PUT: api/Skill/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutSolution(int id, Solution solution)
        {
            try
            {
                if (id == 0 || solution == null)
                {
                    return BadRequest("ID and/or solution is null");
                }

                if (id != solution.Id)
                {
                    return BadRequest("ID's does not match");
                }

                if (solution.Users != null)
                {
                    return BadRequest("Cannot accept users");
                }

                try
                {
                    _context.Update(solution);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (!SolutionExists(id))
                    {
                        return NotFound();
                    }

                    return BadRequest(e.Message);
                }

                try
                {
                    var algSolutions = new List<Solution> { solution };

                    var solutionsToUpdate = AlgoliaHelperSolutions.TransformToAlgolia(algSolutions);
                    await AlgoliaHelperSolutions.PartialUpdate(solutionsToUpdate, _algoliaSettingsProjects);

                    //// TODO: I don't like this way of doing it... Look at alternatives
                    //var userIds = solution.Users?.Select(u => u.Id).ToList();
                    //if (userIds != null && userIds.Any())
                    //{
                    //    var usersToUpdateAlgolia = await _context.Users
                    //        .Include(u => u.Solutions)
                    //        .Include(u => u.Skills)
                    //        .Where(u => userIds.Contains(u.Id))
                    //        .ToListAsync();

                    //    var algUsers = AlgoliaHelper.TransformUsersToAlgoliaUsers(usersToUpdateAlgolia);
                    //    await AlgoliaHelper.PartialUpdateUsers(algUsers);
                    //}
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest("Error occurred while editing skill: " + e.Message);
            }
        }

        [HttpPut("Subscribe/{id:int}")]
        public async Task<IActionResult> SubscribeSolution(int id, string subscribe,
            SolutionUpdateDto solutionUpdateDto)
        {
            if (id != solutionUpdateDto.Id)
            {
                return BadRequest("ID's does not match");
            }

            if (subscribe is not ("subscribe" or "unsubscribe"))
            {
                return BadRequest("Subscribe has to be sub or unsubbed.");
            }

            if (solutionUpdateDto.UserId == 0)
            {
                return BadRequest("UserId cannot be null");
            }

            // Selects the Skill from DB using the Id
            var solution = await _context.Solutions.Include(s => s.Users).FirstOrDefaultAsync(s => s.Id == id);

            if (solution == null)
            {
                return NotFound();
            }

            //// Update the skill properties
            //skill.Tag = skillUpdateDto.Tag;

            // Retrieve the user with the specified ID
            var user = await _context.Users /*.Include(u => u.Skills)*/
                .FirstOrDefaultAsync(x => x.Id == solutionUpdateDto.UserId);

            if (user == null)
            {
                return NotFound();
            }

            switch (subscribe)
            {
                case "subscribe":
                {
                    // Add the retrieved users to the skill's list of users
                    solution.Users.Add(user);

                    await _context.SaveChangesAsync();

                    user = await _context.Users
                        .Include(u => u.Skills)
                        .Include(u => u.Solutions)
                        .FirstOrDefaultAsync(x => x.Id == solutionUpdateDto.UserId);

                    var users = new List<User> { user };

                    var algUsers = AlgoliaHelperUsers.TransformToAlgolia(users);
                    await AlgoliaHelperUsers.PartialUpdate(algUsers, _algoliaSettingsUsers);
                    break;
                }
                case "unsubscribe":
                {
                    solution.Users.Remove(user);
                    //skill.Users.RemoveRange(users);

                    await _context.SaveChangesAsync();

                    user = await _context.Users
                        .Include(u => u.Skills)
                        .Include(u => u.Solutions)
                        .FirstOrDefaultAsync(x => x.Id == solutionUpdateDto.UserId);

                    var users = new List<User> { user };

                    var algUsers = AlgoliaHelperUsers.TransformToAlgolia(users);
                    await AlgoliaHelperUsers.PartialUpdate(algUsers, _algoliaSettingsUsers);
                    break;
                }
            }

            if (solution.Users != null && solution.Users.Any())
            {
                solution.Users.Clear();
            }

            return Ok(solution);
        }

        // POST: api/Skill
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Solution>> PostSolution(Solution solution)
        {
            try
            {
                _context.Solutions.Add(solution);
                await _context.SaveChangesAsync();

                try
                {
                    var alg = new List<Solution> { solution };

                    var solutionsToIndex = AlgoliaHelperSolutions.TransformToAlgolia(alg);
                    await AlgoliaHelperSolutions.Index(solutionsToIndex, _algoliaSettingsProjects);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                return CreatedAtAction("GetSolution", new { id = solution.Id }, solution);
            }
            catch (Exception e)
            {
                return BadRequest("Error occurred while posting solution:" + e.Message);
            }
        }

        // DELETE: api/Skill/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Solution>> DeleteSolution(int id)
        {
            try
            {
                var solution = await _context.Solutions.FindAsync(id);
                if (solution == null)
                {
                    return NotFound();
                }

                try
                {
                    var idsToDelete = new List<string> { solution.Id.ToString() };
                    await AlgoliaHelperSolutions.Delete(idsToDelete, _algoliaSettingsProjects);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                _context.Solutions.Remove(solution);
                await _context.SaveChangesAsync();

                return solution;
            }
            catch (Exception e)
            {
                return BadRequest("Error occurred while deleting solution: " + e.Message);
            }
        }

        private bool SolutionExists(int id)
        {
            return _context.Solutions.Any(e => e.Id == id);
        }
    }
}
