using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WeddingSite.Services
{
    public class EmailService
    {
        private readonly HttpClient _http;

        public record ContactPayload
        {
            public string Name { get; init; } = "";
            public string Email { get; init; } = "";
            public string Subject { get; init; } = "";
            public string Message { get; init; } = "";
            public string Hp { get; init; } = "";
        }

        public EmailService(HttpClient http)
        {
            _http = http;
        }

        /// <summary>
        /// Posts the contact payload to the backend API (POST /api/contact).
        /// The backend handles SMTP using secret credentials.
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> SendContactAsync(ContactPayload payload)
        {
            try
            {
                var resp = await _http.PostAsJsonAsync("api/contact", payload);
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