using Microsoft.Net.Http.Headers;
using System;

namespace AuthAPI.CookieGenerator
{
    public class CookieGenerator : ICookieGenerator
    {
        public CookieOptions GenerateCookie(DateTime dateTime)
        {
            CookieOptions cookieOptions = new();
            cookieOptions.Expires = dateTime;
            cookieOptions.Secure = true;
            cookieOptions.HttpOnly = true;
            cookieOptions.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
            return cookieOptions;
        }

        //public CookieOptions GenerateNonHttpCookie(DateTime dateTime)
        //{
        //    CookieOptions cookieOptions = new();
        //    cookieOptions.Expires = dateTime;
        //    cookieOptions.Secure = true;
        //    cookieOptions.HttpOnly = false;
        //    cookieOptions.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
        //    return cookieOptions;
        //}
    }
}
