using System.Text.Json.Serialization;

// Model for deserialization
public class RecaptchaResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("challenge_ts")]
    public string ChallengeTimestamp { get; set; }

    [JsonPropertyName("hostname")]
    public string Hostname { get; set; }  // Ensure this matches the JSON key exactly
}