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
    public class SkillController : ControllerBase
    {
        private readonly Context _context;
        private readonly AlgoliaSettings _algoliaSettingsSkills;
        private readonly AlgoliaSettings _algoliaSettingsUsers;

        public SkillController(Context context)
        {
            _context = context;
            _algoliaSettingsSkills = AlgoliaHelper.LoadAlgoliaSettingsSkills();
            _algoliaSettingsUsers = AlgoliaHelper.LoadAlgoliaSettingsUsers();
        }

        // GET: api/Skill
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Skill>>> GetSkills()
        {
            try
            {
                return await _context.Skills
                    .OrderBy(on => on.Title)
                    .Select(s => new Skill
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Category = s.Category,
                        Users = s.Users
                    })
                    .ToListAsync();
            }
            catch (Exception e)
            {
                return BadRequest("Error occurred while retrieving skills: " + e.Message);
            }
        }

        [HttpGet("SkillsWithoutUsers")]
        public async Task<ActionResult<IEnumerable<Skill>>> GetSkillsWithoutUsers()
        {
            try
            {
                return await _context.Skills
                    .OrderBy(on => on.Title)
                    .Select(s => new Skill
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Category = s.Category
                    })
                    .ToListAsync();
            }
            catch (Exception e)
            {
                return BadRequest("Error occurred while retrieving skills: " + e.Message);
            }
        }

        // GET: api/Skill/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Skill>> GetSkill(int id)
        {
            try
            {
                var skill = await _context.Skills.FindAsync(id);

                if (skill == null)
                {
                    return NotFound();
                }

                return skill;
            }
            catch (Exception e)
            {
                return BadRequest("Error occurred while retrieving skill: " + e.Message);
            }
        }

        [HttpGet("IndexAllSkills")]
        public async Task<IActionResult> IndexAllSkills()
        {
            var skills = await _context.Skills.ToListAsync();

            var algoliaSkills = AlgoliaHelperSkills.TransformToAlgolia(skills);

            await AlgoliaHelperSkills.Index(algoliaSkills, _algoliaSettingsSkills);

            return Ok();
        }

        // PUT: api/Skill/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSkill(int id, Skill skill)
        {
            try
            {
                if (id == 0 || skill == null)
                {
                    return BadRequest("ID and/or skill is null");
                }

                if (id != skill.Id)
                {
                    return BadRequest("ID's does not match");
                }

                if (skill.Users != null)
                {
                    return BadRequest("Cannot accept users");
                }

                try
                {
                    _context.Update(skill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (!SkillExists(id))
                    {
                        return NotFound();
                    }

                    return BadRequest(e.Message);
                }

                try
                {
                    var algSkills = new List<Skill> { skill };

                    var skillsToUpdateAlgolia = AlgoliaHelperSkills.TransformToAlgolia(algSkills);
                    await AlgoliaHelperSkills.PartialUpdate(skillsToUpdateAlgolia, _algoliaSettingsSkills);


                    //if (skill.Users != null)
                    //{
                    //    var usersToUpdateAlgolia = new List<User>();

                    //    foreach (var user in skill.Users)
                    //    {
                    //        usersToUpdateAlgolia.Add(await _context.Users.Include(u => u.Solutions)
                    //            .Include(u => u.Skills).Where(u => u.Id == user.Id).SingleOrDefaultAsync());
                    //    }

                    //    var algUsers = AlgoliaHelper.TransformUsersToAlgoliaUsers(usersToUpdateAlgolia);
                    //    await AlgoliaHelper.PartialUpdateUsers(algUsers);
                    //}


                    // TODO: I don't like this way of doing it... Look at alternatives
                    //var userIds = skill.Users?.Select(u => u.Id).ToList();
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
        public async Task<IActionResult> SubscribeSkill(int id, string subscribe, SkillUpdateDto skillUpdateDto)
        {
            if (id != skillUpdateDto.Id)
            {
                return BadRequest("ID's do not match");
            }

            if (subscribe is not ("subscribe" or "unsubscribe"))
            {
                return BadRequest("Subscribe has to be 'subscribe' or 'unsubscribe'.");
            }

            if (skillUpdateDto.UserId == 0)
            {
                return BadRequest("UserId cannot be null");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Selects the Skill from DB using the Id
                var skill = await _context.Skills.Include(s => s.Users).FirstOrDefaultAsync(s => s.Id == id);

                if (skill == null)
                {
                    return NotFound();
                }

                // Retrieve the user with the specified ID
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == skillUpdateDto.UserId);

                if (user == null)
                {
                    return NotFound();
                }

                switch (subscribe)
                {
                    case "subscribe":
                        // Add the retrieved users to the skill's list of users
                        skill.Users.Add(user);
                        break;
                    case "unsubscribe":
                        skill.Users.Remove(user);
                        break;
                }

                await _context.SaveChangesAsync();

                try
                {
                    user = await _context.Users
                        .Include(u => u.Skills)
                        .Include(u => u.Solutions)
                        .FirstOrDefaultAsync(x => x.Id == skillUpdateDto.UserId);

                    var users = new List<User> { user };

                    var algUsers = AlgoliaHelperUsers.TransformToAlgolia(users);
                    await AlgoliaHelperUsers.PartialUpdate(algUsers, _algoliaSettingsUsers);

                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(e);
                    return BadRequest("Error occurred while updating Algolia: " + e.Message);
                }

                if (skill.Users != null && skill.Users.Any())
                {
                    skill.Users.Clear();
                }

                return Ok(skill);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return BadRequest("Error occurred while subscribing/unsubscribing skill: " + e.Message);
            }
        }

        //[HttpPut("UpdateUserSkills")]
        //public async Task<IActionResult> UpdateUserSkills(List<SkillUpdateDto> skillUpdateDtos)
        //{
        //    if (skillUpdateDtos == null || !skillUpdateDtos.Any())
        //    {
        //        return BadRequest("SkillUpdateDtos is null or empty");
        //    }

        //    foreach (var skillUpdateDto in skillUpdateDtos)
        //    {
        //        var skill = await _context.Skills.Include(s => s.Users)
        //            .FirstOrDefaultAsync(s => s.Id == skillUpdateDto.Id);

        //        if (skill == null)
        //        {
        //            return NotFound();
        //        }

        //        var user = await _context.Users.Include(u => u.Skills)
        //            .FirstOrDefaultAsync(u => u.Id == skillUpdateDto.UserId);

        //        if (user == null)
        //        {
        //            return NotFound();
        //        }

        //        if (skill.Users.Contains(user))
        //        {
        //            skill.Users.Remove(user);
        //        }
        //        else
        //        {
        //            skill.Users.Add(user);
        //        }
        //    }

        //    await _context.SaveChangesAsync();

        //    var users = await _context.Users
        //        .Include(u => u.Skills)
        //        .Include(u => u.Solutions)
        //        .ToListAsync();

        //    var algUsers = AlgoliaHelperUsers.TransformToAlgolia(users);
        //    await AlgoliaHelperUsers.PartialUpdate(algUsers, _algoliaSettingsUsers);

        //    return Ok();
        //}


        // POST: api/Skill
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Skill>> PostSkill(Skill skill)
        {
            try
            {
                _context.Skills.Add(skill);
                await _context.SaveChangesAsync();

                try
                {
                    var alg = new List<Skill> { skill };

                    var skillsToUpdateAlgolia = AlgoliaHelperSkills.TransformToAlgolia(alg);
                    await AlgoliaHelperSkills.Index(skillsToUpdateAlgolia, _algoliaSettingsSkills);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                return CreatedAtAction("GetSkill", new { id = skill.Id }, skill);
            }
            catch (Exception e)
            {
                return BadRequest("Error occurred while posting skill:" + e.Message);
            }
        }

        [HttpPost("list")]
        public async Task<ActionResult<Skill>> PostSkills(List<Skill> skills)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var alg = new List<Skill>();

                foreach (var skill in skills)
                {
                    _context.Skills.Add(skill);
                    alg.Add(skill);
                }

                await _context.SaveChangesAsync();

                try
                {
                    var skillsToUpdateAlgolia = AlgoliaHelperSkills.TransformToAlgolia(alg);
                    await AlgoliaHelperSkills.Index(skillsToUpdateAlgolia, _algoliaSettingsSkills);

                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(e);
                    return BadRequest("Error occurred while updating Algolia: " + e.Message);
                }

                return Ok(skills);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return BadRequest("Error occurred while saving skills: " + e.Message);
            }
        }

        // DELETE: api/Skill/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Skill>> DeleteSkill(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var skill = await _context.Skills.Include(s => s.Users).FirstOrDefaultAsync(x => x.Id == id);
                if (skill == null)
                {
                    return NotFound();
                }

                if (skill.Users != null && skill.Users.Any())
                {
                    var usersToUpdate = skill.Users.ToList();

                    foreach (var user in usersToUpdate)
                    {
                        user.Skills.Remove(skill);
                    }

                    skill.Users.Clear();
                }

                _context.Skills.Remove(skill);
                await _context.SaveChangesAsync();

                if (skill.Users != null && skill.Users.Any())
                {
                    var usersToUpdateAlgolia = AlgoliaHelperUsers.TransformToAlgolia(skill.Users.ToList());
                    await AlgoliaHelperUsers.PartialUpdate(usersToUpdateAlgolia, _algoliaSettingsUsers);
                }

                try
                {
                    var idsToDelete = new List<string> { skill.Id.ToString() };
                    await AlgoliaHelperSkills.Delete(idsToDelete, _algoliaSettingsSkills);

                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(e);
                    return BadRequest("Error occurred while updating Algolia: " + e.Message);
                }

                return skill;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return BadRequest("Error occurred while deleting skill: " + e.Message);
            }
        }

        private bool SkillExists(int id)
        {
            return _context.Skills.Any(e => e.Id == id);
        }
    }
}
