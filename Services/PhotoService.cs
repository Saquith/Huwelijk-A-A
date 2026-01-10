using System.Net.Http.Json;

namespace WeddingSite.Services
{
    public class PhotoService
    {
        private readonly HttpClient _http;

        public PhotoService(HttpClient http)
        {
            _http = http;
        }

        public class AppSettings
        {
            public PhotoSettings? PhotoSource { get; set; }
        }

        public class PhotoSettings
        {
            public string Mode { get; set; } = "Local"; // "Local" or "Urls"
            public string? LocalFolder { get; set; } = "images";
            public List<string>? PhotoUrls { get; set; } = new();
        }

        public async Task<List<string>> GetPhotoUrlsAsync()
        {
            try
            {
                var settings = await _http.GetFromJsonAsync<AppSettings>("appsettings.json");
                if (settings?.PhotoSource == null) return new List<string>();

                if (settings.PhotoSource.Mode?.Equals("Urls", StringComparison.OrdinalIgnoreCase) == true)
                {
                    return settings.PhotoSource.PhotoUrls ?? new List<string>();
                }
                else // Local
                {
                    // If using Local mode, the user should put images in wwwroot/images and list filenames here.
                    // For convenience, check for a PhotoUrls list and prefix with local folder if present.
                    var urls = settings.PhotoSource.PhotoUrls ?? new List<string>();
                    if (urls.Any())
                    {
                        return urls.Select(u => $"{settings.PhotoSource.LocalFolder!.TrimEnd('/')}/{u}").ToList();
                    }
                    else
                    {
                        // fallback to some default local images (names assumed present)
                        return new List<string>
                        {
                            $"{settings.PhotoSource.LocalFolder}/carousel1.jpg",
                            $"{settings.PhotoSource.LocalFolder}/carousel2.jpg",
                            $"{settings.PhotoSource.LocalFolder}/carousel3.jpg"
                        };
                    }
                }
            }
            catch
            {
                // In case appsettings cannot be read, fall back to default placeholders
                return new List<string> { "images/carousel1.jpg", "images/carousel2.jpg", "images/carousel3.jpg" };
            }
        }
    }
}