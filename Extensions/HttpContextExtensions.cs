using Microsoft.AspNetCore.Http;
using SofttrendsAddon.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SofttrendsAddon.Extensions
{
    public static class HttpContextExtensions
    {
        public static void SetCookie(this HttpContext httpContext, string key, string value, CookieOptions options = null)
        {
            if (httpContext == null)
                return;
            if (httpContext.Response == null)
                return;
            if (httpContext.Request.Cookies.ContainsKey(key))
                httpContext.Response.Cookies.Delete(key);

            value = Utilities.EncryptText(value);
            if (options == null)
                httpContext.Response.Cookies.Append(key, value);
            else
                httpContext.Response.Cookies.Append(key, value, options);
        }

        public static string GetCookie(this HttpContext httpContext, string key)
        {
            if (httpContext == null)
                return "";
            if (httpContext.Request == null)
                return "";

            string cookieValue = string.Empty;
            if (httpContext.Request.Cookies.ContainsKey(key))
            {
                cookieValue = httpContext.Request.Cookies[key];
                if (!string.IsNullOrEmpty(cookieValue))
                    cookieValue = Utilities.DecryptText(cookieValue);
            }
            return cookieValue;
        }
    }
}
