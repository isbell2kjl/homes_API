namespace blog_API.Helpers;

using System.Globalization;

//borrowed from:
//https://jasonwatmore.com/post/2022/02/26/net-6-boilerplate-api-tutorial-with-
//email-sign-up-verification-authentication-forgot-password#validate-reset-token-request-cs

// custom exception class for throwing application specific exceptions (e.g. for validation) 
// that can be caught and handled within the application
public class AppException : Exception
{
    public AppException() : base() {}

    public AppException(string message) : base(message) { }

    public AppException(string message, params object[] args) 
        : base(String.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
}