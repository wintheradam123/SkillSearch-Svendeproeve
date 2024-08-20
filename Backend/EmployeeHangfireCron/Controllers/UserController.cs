using EmployeeHangfireCron.Algolia;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Helpers;
using Shared.Models;

namespace GraphCronJob.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Context _context;

        public UserController(Context context)
        {
            _context = context;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<List<User>> PostUsersAndUpdate(List<User> adUsers, AlgoliaSettings settings)
        {
            var usersToUpdate = new List<User>();
            var usersToIndex = new List<User>();

            try
            {
                foreach (var adUser in adUsers)
                {
                    // Get the corresponding user from the local database
                    var dbUser = _context.Users.FirstOrDefault(x => x.ExternalId == adUser.ExternalId);

                    if (dbUser != null)
                    {
                        // If the users are the same, skip updating the db user
                        if (dbUser.Equals(adUser))
                        {
                            continue;
                        }

                        adUser.Id = dbUser.Id;
                        //adUser.Hash = adUser.GetHashCode();

                        _context.Entry(dbUser).CurrentValues.SetValues(adUser);

                        // Keep a list of users to update in Algolia
                        usersToUpdate.Add(dbUser);
                    }
                    else
                    {
                        // Add the new user to the database
                        //adUser.Hash = adUser.GetHashCode();
                        _context.Users.Add(adUser);
                        
                        // Keep a list of users to create in Algolia
                        usersToIndex.Add(adUser);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            try
            {
                // TODO: Add this
                // Update Algolia
                if (usersToUpdate.Count != 0)
                {
                    var usersToUpdateAlgolia = AlgoliaHelperUsers.TransformToAlgolia(usersToUpdate);
                    await AlgoliaHelperUsers.PartialUpdate(usersToUpdateAlgolia, settings);
                }

                if (usersToIndex.Count != 0)
                {
                    var usersToIndexAlgolia = AlgoliaHelperUsers.TransformToAlgolia(usersToIndex);
                    await AlgoliaHelperUsers.Index(usersToIndexAlgolia, settings);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            // TODO: do not return a list of users that's the same as we started with.
            // Will maybe break Hangfire if we change
            return adUsers;
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutUser(User user)
        //{
        //    try
        //    {
        //        // user.Id is always 0, so why the check?
        //        //if (dbUserId != adUser.Id)
        //        //{
        //        //    return BadRequest();
        //        //}

        //        // TODO: Is this still used?
        //        user.Hash = user.GetHashCode();

        //        try
        //        {
        //            //_context.Update(adUser);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException e)
        //        {
        //            if (!UserExists(userId))
        //            {
        //                return NotFound();
        //            }
        //        }

        //        return NoContent();
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest("Error occurred while editing user: " + e.Message);
        //    }
        //}

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<int>> DeleteUsers(List<User> users, AlgoliaSettings settings)
        {
            _context.Users.RemoveRange(users);

            try
            {
                // This code deletes the opposite of all disabled users
                //    List<string> idsToDelete = new();
                //    foreach (var user in _context.Users)
                //    {
                //        try
                //        {
                //            var userAzureId = users.FirstOrDefault(x => x.AzureId == user.AzureId);

                //            if (userAzureId != null) continue;

                //            _context.Users.Remove(user);
                //            idsToDelete.Add(user.AzureId);
                //        }
                //        catch
                //        {
                //            //Log it here!
                //        }
                //    }

                await _context.SaveChangesAsync();

                var idsToDelete = users.Select(x => x.ExternalId.ToString()).ToList();

                if (idsToDelete.Count == 0)
                {
                    return NoContent();
                }

                // TODO: Add algolia
                await AlgoliaHelperUsers.Delete(idsToDelete, settings);

                return Ok(idsToDelete.Count);
            }
            catch (Exception e)
            {
                return BadRequest("Error while deleting users: " + e.Message);
            }
        }

        [HttpDelete("DeleteUsers")]
        public async Task<ActionResult<int>> DeleteUsers()
        {
            var userIds = _context.Users.Select(x => x.ExternalId.ToString()).ToList();
            //Delete all users
            _context.Users.RemoveRange(_context.Users);

            try
            {
                // This code deletes the opposite of all disabled users
                //    List<string> idsToDelete = new();
                //    foreach (var user in _context.Users)
                //    {
                //        try
                //        {
                //            var userAzureId = users.FirstOrDefault(x => x.AzureId == user.AzureId);

                //            if (userAzureId != null) continue;

                //            _context.Users.Remove(user);
                //            idsToDelete.Add(user.AzureId);
                //        }
                //        catch
                //        {
                //            //Log it here!
                //        }
                //    }

                await _context.SaveChangesAsync();

                if (userIds.Count == 0)
                {
                    return NoContent();
                }

                var algoliaSettings = AlgoliaHelper.LoadAlgoliaSettings();

                await AlgoliaHelperUsers.Delete(userIds, algoliaSettings);

                return Ok(userIds.Count);
            }
            catch (Exception e)
            {
                return BadRequest("Error while deleting users: " + e.Message);
            }
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        //public async Task<List<User>> GetDisabledUsers()
        //{
        //    var disabledUsers = _context.Users.Where(x => x.AccountEnabled == null || x.AccountEnabled == false).ToList();
        //    return disabledUsers;
        //}
    }
}