namespace homes_API.Helpers;

public class AppSettings
{

    //borrowed from:
    //https://jasonwatmore.com/post/2022/02/26/net-6-boilerplate-api-tutorial-with-
    //email-sign-up-verification-authentication-forgot-password#validate-reset-token-request-cs

    // public string Secret { get; set; }

    // refresh token time to live (in days), inactive tokens are
    // automatically deleted from the database after this time
    // public int RefreshTokenTTL { get; set; }

    public string? EmailFrom { get; set; }
    public string? SmtpHost { get; set; }
    public int SmtpPort { get; set; }
    public string? SmtpUser { get; set; }
    public string? SmtpPass { get; set; }
}