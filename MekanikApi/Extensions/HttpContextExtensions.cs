namespace sispay.Api.Extensions
{
    public static class HttpContextExtensions
    {
        public static string? GetAuthorizationHeader(this HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                Console.WriteLine($"Authorization Header: {authorizationHeader}");

                const string bearerPrefix = "Bearer ";
                if (authorizationHeader.ToString().StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    return authorizationHeader.ToString()[bearerPrefix.Length..].Trim();
                }
            }

            return null;
        }
    }
}