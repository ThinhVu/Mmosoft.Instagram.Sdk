using System;
using System.Net;

namespace InstagramUser
{
    public class HttpRequestBuilder
    {
        private static HttpWebRequest CreateRequest(Uri uri, CookieContainer cookies = null)
        {
            var request = WebRequest.Create(uri) as HttpWebRequest;            
            request.ProtocolVersion = HttpVersion.Version11;
            request.Timeout = 10000;
            request.Host = uri.Host;
            request.Accept = "*/*";
            request.KeepAlive = true;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:46.0) Gecko/20100101 Firefox/46.0";            
            request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            if (cookies != null) request.CookieContainer = cookies;
            return request;
        }

        public static HttpWebRequest Post(string url, CookieContainer cookies = null)
        {           
            var request = CreateRequest(new Uri(url), cookies);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/x-www-form-urlencoded";                        
            return request;
        }

        public static HttpWebRequest Get(string url, CookieContainer cookies = null)
        {
            var request = CreateRequest(new Uri(url), cookies);
            request.Method = WebRequestMethods.Http.Get;
            return request;
        }
    }
}
