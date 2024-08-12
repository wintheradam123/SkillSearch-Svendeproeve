using EmployeeHangfireCron.Models;
using Newtonsoft.Json;
using Shared.Models;

namespace EmployeeHangfireCron
{
    public class EmployeeHelper
    {
        public static async Task<List<User>> GetAllUsersAsync()
        {
            using var httpClient = new HttpClient();

            // Set the base address if you plan to make multiple requests to the same domain
            httpClient.BaseAddress = new Uri("https://localhost:7270/");

            try
            {
                // Make a GET request to the specified endpoint
                HttpResponseMessage response = await httpClient.GetAsync("api/Employee");
                response.EnsureSuccessStatusCode(); // Throw an exception if not successful

                // Read the response content as a string
                string content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);

                var extEmployees = JsonConvert.DeserializeObject<List<ExternalEmployee>>(content);

                return extEmployees.Select(externalEmployee => new User
                    {
                        ExternalId = externalEmployee.Id,
                        DisplayName = externalEmployee.Name,
                        UserPrincipalName = externalEmployee.Email,
                        JobTitle = externalEmployee.JobTitle,
                        OfficeLocation = externalEmployee.Location
                        //ImageUrl = externalEmployee.ImageUrl
                    })
                    .ToList();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request exception: {e.Message}");
                return new List<User>();
            }
        }
    }
}
