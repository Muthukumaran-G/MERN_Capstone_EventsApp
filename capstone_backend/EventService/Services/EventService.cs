namespace EventService.Services
{
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class EventService : IEventService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "6GPwOFrDGpT3oDr4eH5kQDbBlFGAGycv";

        public EventService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Events>> GetEventsAsync()
        {
            string apiUrl = $"https://app.ticketmaster.com/discovery/v2/events.json?apikey={_apiKey}";

            var response = await _httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode) return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var ticketmasterResponse = JsonSerializer.Deserialize<TicketmasterResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return ticketmasterResponse?._embedded?.events?.Select(e => new Events
            {
                Id = e.Id,
                Name = e.Name,
                Url = e.Url,
                ImageUrl = e.Images?.FirstOrDefault()?.Url,
                Date = e.Dates?.Start?.LocalDate,
                Time = e.Dates?.Start?.LocalTime
            }).ToList();
        }
    }
}
