using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Common
{
    public class CookieHelper
    {
        /// <summary>
        /// 设置Cookie值和过期时间
        /// </summary>
        /// <param name="cookieName">Cookie名称</param>
        /// <param name="value">值</param>
        /// <param name="expires">过期时间</param>
        public static void SetCookie(string cookieName, string value, DateTime expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                cookie.Value = value;
                cookie.Expires = expires;
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            else
            {
                cookie = new HttpCookie(cookieName);
                cookie.Value = value;
                cookie.Expires = expires;
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// 获得Cookie的值
        /// </summary>
        /// <param name="cookieName">Cookie名称</param>
        /// <returns></returns>
        public static string GetCookieValue(string cookieName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie == null)
                return "";
            else
                return cookie.Value;
        }

        /// <summary>
        /// 删除Cookie的值
        /// </summary>
        /// <param name="cookieName">Cookie名称</param>
        /// <returns></returns>
        public static void RemoveCookie(string cookieName)
        {
            SetCookie(cookieName, "", DateTime.Now.AddDays(1));
        }
    }
}