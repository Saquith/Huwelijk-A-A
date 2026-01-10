using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WeddingSite.Services
{
    public class EmailService
    {
        private readonly HttpClient _http;
        private readonly string _endpoint;

        public EmailService(HttpClient http, string endpoint)
        {
            _http = http;
            _endpoint = endpoint ?? string.Empty;
        }

        public record ContactPayload
        {
            public string Name { get; init; } = "";
            public string Email { get; init; } = "";
            public string Subject { get; init; } = "";
            public string Message { get; init; } = "";
            // Honeypot field (should be empty) to help reduce spam
            public string Hp { get; init; } = "";
        }

        /// <summary>
        /// Sends the contact payload to the configured endpoint as JSON.
        /// Returns true when the API responds with a success status code.
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> SendContactAsync(ContactPayload payload)
        {
            if (string.IsNullOrWhiteSpace(_endpoint))
                return (false, "Email endpoint not configured.");

            try
            {
                var resp = await _http.PostAsJsonAsync(_endpoint, payload);
                if (resp.IsSuccessStatusCode) return (true, null);

                var text = await resp.Content.ReadAsStringAsync();
                return (false, $"API returned {(int)resp.StatusCode}: {text}");
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}