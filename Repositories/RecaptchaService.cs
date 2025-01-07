using System.Text.Json;

namespace homes_API.Repositories;

public class RecaptchaService : IRecaptchaService
{

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private const string RecaptchaVerifyUrl = "https://www.google.com/recaptcha/api/siteverify";

    public RecaptchaService(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> VerifyRecaptcha(string token)
    {
        var secretKey = _config["RecaptchaSecretKey"];
        var httpClient = _httpClientFactory.CreateClient();

        // Send the request to the reCAPTCHA API with the secret and token
        var response = await httpClient.PostAsync(
            $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}",
            null);

        if (!response.IsSuccessStatusCode)
        {
            // Console.WriteLine($"Failed to call reCAPTCHA API. Status Code: {response.StatusCode}");
            return false;
        }

        // Parse the response content
        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Recaptcha response: {responseContent}");
        var verificationResult = JsonSerializer.Deserialize<RecaptchaResponse>(responseContent);

        if (verificationResult == null)
        {
            // Console.WriteLine("Deserialization returned null. Response content may be invalid.");
            return false;
        }

        Console.WriteLine($"reCAPTCHA validation result: Success = {verificationResult.Success}, Hostname = {verificationResult.Hostname}");
        return verificationResult.Success;

    }
}

