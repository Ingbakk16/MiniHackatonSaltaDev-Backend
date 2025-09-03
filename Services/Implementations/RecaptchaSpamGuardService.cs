using AnimalesPerdidos.Services.Interfaces;
using System.Text.Json;
namespace AnimalesPerdidos.Services.Implementations
{
    public class RecaptchaSpamGuardService : ISpamGuard
    {
        private readonly HttpClient _http;
        private readonly string _secret;


        public RecaptchaSpamGuardService(IConfiguration cfg, IHttpClientFactory httpFactory)
        {
            _secret = cfg["Recaptcha:SecretKey"] ?? throw new InvalidOperationException("Recaptcha secret missing");
            _http = httpFactory.CreateClient();
        }


        public async Task<bool> ValidateCaptchaAsync(string token, string ip, CancellationToken ct)
        {
            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string,string>("secret", _secret),
new KeyValuePair<string,string>("response", token),
new KeyValuePair<string,string>("remoteip", ip)
});
            using var resp = await _http.PostAsync("https://www.google.com/recaptcha/api/siteverify", content, ct);
            var json = await resp.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetProperty("success", out var success) && success.GetBoolean();
        }
    }
}
