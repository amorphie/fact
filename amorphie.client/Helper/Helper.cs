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
}