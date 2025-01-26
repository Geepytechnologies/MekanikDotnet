using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Infrastructure.Services
{
    public interface ILocationService
    {
        Task<double> GetTravelTimeAsync(double originLatitude, double originLongitude, double destinationLatitude, double destinationLongitude);
    }
    public class LocationService : ILocationService
    {
        private readonly string GoogleApiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");

        public async Task<double> GetTravelTimeAsync(double originLatitude, double originLongitude, double destinationLatitude, double destinationLongitude)
        {
            string origin = $"{originLatitude},{originLongitude}";
            string destination = $"{destinationLatitude},{destinationLongitude}";
            var url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={Uri.EscapeDataString(origin)}&destinations={Uri.EscapeDataString(destination)}&key={GoogleApiKey}";

            using var httpClient = new System.Net.Http.HttpClient();
            try
            {
                var response = await httpClient.GetStringAsync(url);
                dynamic result = JsonConvert.DeserializeObject(response);

                // Validate the response structure
                if (result?.rows != null && result.rows.Count > 0 &&
                    result.rows[0]?.elements != null && result.rows[0].elements.Count > 0)
                {
                    var element = result.rows[0].elements[0];

                    if (element?.status == "OK" && element.duration?.value != null)
                    {
                        // Travel time in seconds
                        return (double)element.duration.value;
                    }
                    else
                    {
                        throw new Exception($"API returned status: {element?.status}");
                    }
                }
                else
                {
                    throw new Exception("Invalid response structure from Google Maps API.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow it to be handled by the calling code
                Console.WriteLine($"Error fetching travel time: {ex.Message}");
                throw;
            }
        }
    }
}
