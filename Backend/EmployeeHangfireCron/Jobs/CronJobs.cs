using EmployeeHangfireCron;
using EmployeeHangfireCron.Algolia;
using GraphCronJob.Controllers;
using GraphCronJob.Repositories;
using Shared.Models;

namespace GraphCronJob.Jobs
{
    public class CronJobs
    {
        //private readonly Settings _settings = Settings.LoadSettings();
        private readonly UserController _userController;
        private readonly OfficeLocationRepository _officeLocationRepository;
        private readonly JobTitleRepository _jobTitleRepository;

        public CronJobs(UserController userController,
            OfficeLocationRepository officeLocationRepository,
            JobTitleRepository jobTitleRepository)
        {
            _userController = userController;
            _officeLocationRepository = officeLocationRepository;
            _jobTitleRepository = jobTitleRepository;
        }

        public async Task RunTask()
        {
            try
            {

                var users = await EmployeeHelper.GetAllUsersAsync();

                var filteredUsers = FilterUsers(users);
                var usersWithRoles = UsersWithRoles(filteredUsers);

                var algoliaSettings = AlgoliaHelper.LoadAlgoliaSettings();

                await IndexOfficeLocations(usersWithRoles, algoliaSettings);
                await IndexJobTitles(usersWithRoles, algoliaSettings);

                // TODO: Consider if we need to enrich users
                // Enrich the AD User object with photos, slack name before saving it.
                //var enrichedUsers = await ApplyUserPhotos(filteredUsers);

                // I don't know what this list contains or is used for. So I'm not gonna use it for now.
                var responseUserList = new List<User>();
                responseUserList.AddRange(await _userController.PostUsersAndUpdate(usersWithRoles, algoliaSettings));

                var deleteUsers = new List<User>();

                // This code adds users from db, which is not in AD
                foreach (var user in _userController.GetAllUsers())
                {
                    if (usersWithRoles.Count > 0 &&
                        usersWithRoles.FirstOrDefault(x => x.ExternalId == user.ExternalId) == null)
                    {
                        deleteUsers.Add(user);
                        Console.WriteLine(
                            $"Add user ({user.Id} , {user.DisplayName} , {user.ExternalId}) to deleteUsers");
                    }
                }

                await _userController.DeleteUsers(deleteUsers, algoliaSettings);

                // TODO: Add this when caching is setup
                //await ClearCacheApi();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Filter users based on DisplayName, AzureId and UserPrincipalName
        // A usecase for this could be to filter out users in the AD, that are not actual employees. E.g. printerUser@test.com is clearly not a user
        private static List<User> FilterUsers(IEnumerable<User> users)
        {
            List<int> filteredIds = new();
            List<string> filteredNames = new();
            List<string> filteredEmails = new()
            {
                "printeruser@test.com"
            };

            var filteredUsers = users.Where(x =>
                !filteredIds.Contains(x.ExternalId) &&
                !filteredEmails.Contains(x.UserPrincipalName) &&
                !filteredNames.Contains(x.DisplayName)).ToList();

            return filteredUsers;
        }

        // Gives admin roles to specified users
        private static List<User> UsersWithRoles(IEnumerable<User> users)
        {
            List<string> emails = new()
            {
                "John@test.com"
            };

            var usersWithRoles = users.Where(x => emails.Contains(x.UserPrincipalName)).ToList();
            foreach (var user in usersWithRoles)
            {
                user.Role = "Admin";
            }

            var usersWithoutRoles = users.Where(x => !emails.Contains(x.UserPrincipalName)).ToList();
            foreach (var user in usersWithoutRoles)
            {
                user.Role = "User";
            }

            return usersWithRoles.Concat(usersWithoutRoles).ToList();
        }

        // Enrich users with photo. Should be done with referenced variables and not by creating new lists.
        //private static async Task<List<User>> ApplyUserPhotos(List<User> users, bool skip = false)
        //{
        //    // Used for testing. Don't waste time enriching the users.
        //    if (skip)
        //    {
        //        return users;
        //    }

        //    var tempUsers = new List<User>();

        //    foreach (var user in users)
        //    {
        //        // Skip disabled (not the handicapped) users. We're gonna delete them anyway.
        //        if (user.AccountEnabled == false)
        //        {
        //            continue;
        //        }

        //        // This takes a veeeeery long time (+20 min.) so it can be stepped over or disabled during testing.
        //        // Note that values related to the methods won't be added or updated on the user in the DB or Algolia.
        //        try
        //        {
        //            var photoUser = await SlackHelper.GetUserPhoto(user);

        //            if ((photoUser?.ImageSize ?? 0) == 0)
        //            {
        //                photoUser = await GraphHelper.GetUserPhoto(user);
        //            }

        //            if ((photoUser?.ImageSize ?? 0) == 0)
        //            {
        //                photoUser = await GravatarHelper.GetUserPhoto(user);
        //            }

        //            tempUsers.Add(photoUser ?? user);
        //            Console.WriteLine($"Added photo for {photoUser?.DisplayName}. Slack name: {photoUser?.SlackName} ");
        //        }
        //        catch (Exception e)
        //        {
        //            tempUsers.Add(user);
        //            Console.WriteLine(e.InnerException);
        //        }
        //    }

        //    return tempUsers;
        //}

        private async Task IndexOfficeLocations(List<User> users, AlgoliaSettings algoliaSettings)
        {
            // Every location that exists already
            var dbLocations = await _officeLocationRepository.GetOfficeLocations();

            var userLocations = new List<OfficeLocation>();

            // For hver bruger der bliver hentet fra AD'et. Alle de brugere har listen vi skal bruge over locations.
            // For hver bruger skal vi tjekke på om lokationen allerede findes i databasen.
            // Hvis ikke den gør skal den oprettes i databasen.
            // Alle de lokationer der ikke er kommet med, skal fjernes fra databasen
            foreach (var user in users)
            {
                if (user.OfficeLocation == null)
                    continue;

                // Tjekker om lokation allerede er på listen. Altså er den unik
                var exists = userLocations.FirstOrDefault(l => l.LocationName == user.OfficeLocation);

                // Hvis den ikke er unik. Gå videre
                if (exists != null) continue;

                // Her indsætter vi lokation hvis den er unik
                OfficeLocation uniqueLocation = new()
                {
                    LocationName = user.OfficeLocation
                };
                userLocations.Add(uniqueLocation);
            }

            // This adds each new location to the DB
            foreach (var officeLocation in userLocations)
            {
                var exists = dbLocations.FirstOrDefault(l => l.LocationName == officeLocation.LocationName);

                if (exists != null) continue;
                await _officeLocationRepository.PostOfficeLocation(officeLocation, algoliaSettings);
            }


            // This deletes every location that is not in the list of unique locations
            foreach (var location in dbLocations)
            {
                var loc = userLocations.FirstOrDefault(l => l.LocationName == location.LocationName);

                if (loc != null)
                    continue;

                await _officeLocationRepository.DeleteOfficeLocation(location.Id, algoliaSettings);
            }
        }

        private static async Task ClearCacheApi()
        {
            var client = new HttpClient();
            await client.GetAsync("http://localhost:7270/api/user/clearCache");
        }

        private async Task IndexJobTitles(List<User> users, AlgoliaSettings algoliaSettings)
        {
            // Every jobTitle that exists already
            var dbJobTitles = await _jobTitleRepository.GetJobTitles();

            // Every unique location
            var userJobTitles = new List<JobTitle>();

            // For hver bruger der bliver hentet fra AD'et. Alle de brugere har listen vi skal bruge over JobTitles.
            // For hver bruger skal vi tjekke på om JobTitles allerede findes i databasen.
            // Hvis ikke den gør skal den oprettes i databasen.
            // Alle de JobTitles der ikke er kommet med, skal fjernes fra databasen
            foreach (var user in users)
            {
                if (user.JobTitle == null)
                    continue;

                // Tjekker om lokation allerede er på listen. Altså er den unik
                var exists = userJobTitles.FirstOrDefault(l => l.Title == user.JobTitle);

                // Hvis den ikke er unik. Gå videre
                if (exists != null) continue;

                // Her indsætter vi lokation hvis den er unik
                JobTitle uniqueJobTitle = new()
                {
                    Title = user.JobTitle
                };
                userJobTitles.Add(uniqueJobTitle);
            }

            List<JobTitle> titlesToPost = new();
            foreach (var jobTitles in userJobTitles)
            {
                var exists = dbJobTitles.FirstOrDefault(l => l.Title == jobTitles.Title);

                if (exists != null) continue;

                await _jobTitleRepository.PostJobTitle(jobTitles, algoliaSettings);

                titlesToPost.Add(jobTitles);
            }


            List<string> titlesToDelete = new();
            foreach (var jobTitle in dbJobTitles)
            {
                var title = userJobTitles.FirstOrDefault(l => l.Title == jobTitle.Title);

                if (title != null)
                    continue;

                await _jobTitleRepository.DeleteJobTitle(jobTitle.Id, algoliaSettings);

                titlesToDelete.Add(jobTitle.Id.ToString());
            }
        }
    }
}