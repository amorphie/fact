using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace amorphie.fact.core.Helper
{
    public class LangHelper
    {
        public static string GetLangCode(HttpContext context)
        {
            try
            {
                var headers = context.Request.Headers;
                var lang = headers["acceptlanguage"];
                if (string.IsNullOrWhiteSpace(lang))
                    return "tr-TR";
                return lang;
            }
            catch (Exception ex)
            {
                return "tr-TR";
            }
        }

        public static string GetLang(HttpContext context)
        {
            var lang = GetLangCode(context);
            return lang switch{
                "tr-TR" => "tr",
                "en-US" => "en",
                "en-EN" => "en",
                _ => "tr"
            };
        }
    }
}