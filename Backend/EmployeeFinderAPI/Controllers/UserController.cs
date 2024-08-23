using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Shared;
using Shared.Models;
using SkillSearchAPI.Models;

namespace SkillSearchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")] // Apply CORS policy
    public class UserController : ControllerBase
    {
        /// <summary>
        /// TODO:
        /// DONE - Handle users who doesn't exist in Azure anymore (Been deleted)
        /// Add authorization to only allow trusted access
        /// Remove controllers for adding, deleting and updating users? (Check if we can update skills and solutions and still remove these controllers)
        /// </summary>
        private readonly Context _context;


        //private IMemoryCache _cache;

        public UserController(Context context /*, IMemoryCache cache */ /*, IUserService userService*/)
        {
            _context = context;
            //_cache = cache;
            //_userService = userService;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                var users = await _context.Users
                    .OrderBy(on => on.DisplayName)
                    .Select(u => new User
                    {
                        Id = u.Id,
                        ExternalId = u.ExternalId,
                        DisplayName = u.DisplayName,
                        JobTitle = u.JobTitle,
                        OfficeLocation = u.OfficeLocation,
                        Skills = u.Skills,
                        Solutions = u.Solutions,
                        UserPrincipalName = u.UserPrincipalName,
                        Role = u.Role,
                        //ImageSize = u.ImageSize,
                        //ImageData = null
                    })
                    .ToListAsync();

                    var metadata = new
                    {
                        users.Count
                    };

                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                    return Ok(users);
            }
            catch (Exception e)
            {
                return BadRequest("Error while getting users: " + e.Message);
            }
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                if (id == 0)
                    return BadRequest("ID is cannot be empty");

                var user = await _context.Users
                    .Select(u => new User
                    {
                        Id = u.Id,
                        ExternalId = u.ExternalId,
                        DisplayName = u.DisplayName,
                        JobTitle = u.JobTitle,
                        OfficeLocation = u.OfficeLocation,
                        Skills = u.Skills,
                        Solutions = u.Solutions,
                        UserPrincipalName = u.UserPrincipalName,
                        Role = u.Role,
                        //ImageSize = u.ImageSize,
                        //ImageData = null
                    })
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound(id);
                }

                return user;
            }
            catch (Exception e)
            {
                return BadRequest("Error while getting user: " + e.Message);
            }
        }

        /// <summary>
        /// Supposed to receive a list of users, checks if all the users exist in the database. If a user in the database doesn't exist in the list, it gets removed.
        /// Should never be called, only by the scheduled retrieval of users!
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<int>> CompareDeleteUpdateUsers(List<User> users)
        {
            if (users.Count < 300)
                return BadRequest();

            try
            {
                List<User> respUsers = new();
                foreach (var user in _context.Users)
                {
                    try
                    {
                        var userAzureId = users.FirstOrDefault(x => x.ExternalId == user.ExternalId);

                        if (userAzureId != null) continue;

                        _context.Users.Remove(user);
                        respUsers.Add(user);
                    }
                    catch
                    {
                        //Log it here!
                    }
                }

                await _context.SaveChangesAsync();

                if (respUsers.Count > 0)
                {
                    return Ok(respUsers.Count);
                }

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest("Error while comparing users: " + e.Message);
            }
        }

        [HttpGet("azureId/{id}")]
        public async Task<ActionResult<User>> GetUserExternalId(int id)
        {
            try
            {
                var user = await _context.Users
                    .Select(u => new User
                    {
                        Id = u.Id,
                        ExternalId = u.ExternalId,
                        DisplayName = u.DisplayName,
                        JobTitle = u.JobTitle,
                        OfficeLocation = u.OfficeLocation,
                        Skills = u.Skills,
                        Solutions = u.Solutions,
                        UserPrincipalName = u.UserPrincipalName,
                        //ImageSize = u.ImageSize,
                        //ImageData = null
                    })
                    .FirstOrDefaultAsync(x => x.ExternalId == id);

                if (user == null)
                {
                    return NotFound(id);
                }

                return user;
            }
            catch (Exception e)
            {
                return BadRequest("Error occurred while getting user: " + e.Message);
            }
        }

        // GET: api/User/image/3771db45-af47-4c1e-ab34-dacc653f58ff
        // return image as byte array
        //[HttpGet("image/{azureId}")]
        //public async Task<ActionResult<byte[]>> GetImageUserId(int externalId)
        //{
        //    try
        //    {
        //        if (!_cache.TryGetValue(externalId, out byte[] imageData))
        //        {
        //            var user = await _context.Users
        //                .Select(u => new User
        //                {
        //                    Id = u.Id,
        //                    ExternalId = u.ExternalId,
        //                    ImageData = u.ImageData
        //                })
        //                .FirstOrDefaultAsync(x => x.ExternalId == externalId);

        //            if (user == null)
        //            {
        //                return NotFound(externalId);
        //            }

        //            imageData = user.ImageData ?? new byte[] { 0 };

        //            var cacheEntryOptions = new MemoryCacheEntryOptions()
        //                .SetAbsoluteExpiration(TimeSpan.FromHours(2));

        //            _cache.Set(externalId, imageData, cacheEntryOptions);
        //        }

        //        return File(imageData, "image/jpeg");
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest("Error occurred while getting image for user: " + e.Message);
        //    }
        //}

        //[HttpGet("clearCache")]
        //public ActionResult ClearCache()
        //{
        //    try
        //    {
        //        if (_cache is MemoryCache concreteMemoryCache)
        //        {
        //            concreteMemoryCache.Clear();
        //        }

        //        return Ok();
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest("Error occurred while clearing cache: " + e.Message);
        //    }
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            try
            {
                if (id != user.Id)
                {
                    return BadRequest();
                }

                user.Hash = user.GetHashCode();

                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (!UserExists(id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest("Error occurred while editing user: " + e.Message);
            }
        }

        //TODO: Fix these methods
        //[HttpGet("IndexAllUsers")]
        //public async Task<IActionResult> IndexAllUsers()
        //{
        //    var users = await _context.Users.Include(user => user.Solutions).Include(user => user.Skills).ToListAsync();

        //    var algoliaUsers = AlgoliaHelperUsers.TransformToAlgolia(users);

        //    await AlgoliaHelperUsers.Index(algoliaUsers);

        //    return Ok();
        //}

        //[HttpGet("PartialIndexAllUsers")]
        //public async Task<IActionResult> PartialIndexAllUsers()
        //{
        //    var users = await _context.Users.Include(user => user.Solutions).Include(user => user.Skills).ToListAsync();

        //    var algoliaUsers = AlgoliaHelperUsers.TransformToAlgolia(users);

        //    await AlgoliaHelperUsers.PartialUpdate(algoliaUsers);

        //    return Ok();
        //}

        [HttpPost("CreatePassword")]
        public async Task<IActionResult> CreatePassword(LoginUserRequest user)
        {
            try
            {
                var userToUpdate =
                    await _context.Users.FirstOrDefaultAsync(x => x.UserPrincipalName == user.Email);

                if (userToUpdate == null)
                {
                    return NotFound();
                }

                if (userToUpdate.Password != null || !userToUpdate.Password.IsNullOrEmpty())
                {
                    return BadRequest("Password already exists for user");
                }

                //Checks password for security
                if (user.Password.Length < 8)
                {
                    return BadRequest("Password must be at least 8 characters long");
                }

                userToUpdate.Password = user.Password;

                _context.Update(userToUpdate);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest("Error occurred while creating password: " + e.Message);
            }
        }

        [HttpPost("LoginUser")]
        public async Task<IActionResult> LoginUser(LoginUserRequest user)
        {
            try
            {
                var userToLogin =
                    await _context.Users.FirstOrDefaultAsync(x => x.UserPrincipalName == user.Email);

                if (userToLogin == null || userToLogin.Password.IsNullOrEmpty())
                {
                    return NotFound();
                }

                if (userToLogin.Password != user.Password)
                {
                    return Unauthorized();
                }

                return Ok(userToLogin);
            }
            catch (Exception e)
            {
                return BadRequest("Error occurred while logging in user: " + e.Message);
            }
        }

        [HttpPut("ChangeRole")]
        public async Task<IActionResult> ChangeRole(User user)
        {
            try
            {
                var userToUpdate =
                    await _context.Users.FirstOrDefaultAsync(x => x.UserPrincipalName == user.UserPrincipalName);

                if (userToUpdate == null)
                {
                    return NotFound();
                }

                userToUpdate.Role = user.Role;

                _context.Update(userToUpdate);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest("Error occurred while changing role: " + e.Message);
            }
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}