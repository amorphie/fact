using amorphie.core.security.Extensions;

public static class Helper
{
    public static string GetHeaderLanguage(this HttpContext httpContext)
    {
        var language = "en-EN";

        if (httpContext.Request.Headers.ContainsKey("Language"))
        {
            language = httpContext.Request.Headers["Language"].ToString();
        }

        return language;
    }

    public static async Task SetSecrets(this WebApplicationBuilder builder)
    {
        await builder.Configuration.AddVaultSecrets("user-secretstore", new string[] { "user-secretstore" });

        ApplicationSettings.ClientSecretKey = builder.Configuration["clientSecretKey"]!;

    }
}