using System;
using System.Net;

namespace InstagramUser
{
    public class HttpRequestBuilder
    {
        public static HttpWebRequest Post(Uri uri, CookieContainer cookies)
        {            
            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = WebRequestMethods.Http.Post;
            request.ProtocolVersion = HttpVersion.Version11;
            request.Host = uri.Host;
            request.Accept = "*/*";
            request.KeepAlive = true;            
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:46.0) Gecko/20100101 Firefox/46.0";
            request.ContentType = "application/x-www-form-urlencoded";            
            request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.CookieContainer = cookies;
            return request;
        }

        public static HttpWebRequest Get(Uri uri, CookieContainer cookies)
        {
            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = WebRequestMethods.Http.Get;
            request.ProtocolVersion = HttpVersion.Version11;
            request.Host = uri.Host;
            request.Accept = "*/*";
            request.KeepAlive = true;               
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:46.0) Gecko/20100101 Firefox/46.0";
            request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.CookieContainer = cookies;
            return request;
        }
    }
}
