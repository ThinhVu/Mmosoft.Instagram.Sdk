namespace InstagramUser
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using Newtonsoft.Json;
    using System.IO.Compression;

    public class Instaguser
    {
        /// <summary>
        /// CookieContainer help HttpWebRequest keep session
        /// </summary>
        private CookieContainer mCookieContainer;
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Public info
        /// </summary>
        public User PublicInfo { get; set; }
        
        /// <summary>
        /// Create new instance of User
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public Instaguser(string username, string password)
        {
            Username = username;
            Password = password;

            // Remove Expect: 100-continue header
            System.Net.ServicePointManager.Expect100Continue = false;            
            mCookieContainer = new CookieContainer();
            
            // Login                        
            LogIn();
            // Get public info
            PublicInfo = GetUserPublicInfo(Username);
        }

        /// <summary>
        /// Login to store more cookies
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        private void LogIn()
        {
            // Bootstrap
            var bootstrapRequest = HttpRequestBuilder.Get(new Uri("https://www.instagram.com/accounts/login/"), mCookieContainer);
            bootstrapRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            bootstrapRequest.Headers["Upgrade-Insecure-Requests"] = "1";
            var bootstrapResponse = bootstrapRequest.GetResponse() as HttpWebResponse;
            mCookieContainer.Add(bootstrapResponse.Cookies);
            bootstrapResponse.Close();

            // Login to system
            var data = $"username={Username}&password={Password}";
            var content = Encoding.ASCII.GetBytes(data);

            var request = HttpRequestBuilder.Post(new Uri("https://www.instagram.com/accounts/login/ajax/"), mCookieContainer);
            // Missing referer get 403 - forbiden
            request.Referer = "https://www.instagram.com/accounts/login/";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = content.Length;
            request.KeepAlive = true;
            request.Headers["Origin"] = "https://www.instagram.com";
            request.Headers["X-CSRFToken"] = mCookieContainer.GetCookies(new Uri("https://www.instagram.com"))["csrftoken"].Value;
            request.Headers["X-Instagram-AJAX"] = "1";
            request.Headers["X-Requested-With"] = "XMLHttpRequest";
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(content, 0, content.Length);
                var response = request.GetResponse() as HttpWebResponse;
                mCookieContainer.Add(response.Cookies);
                response.Close();
            }
        }

        /// <summary>
        /// Retrieve user's infor
        /// </summary>
        public User GetUserPublicInfo(string username)
        {
            var uri = new Uri($"https://www.instagram.com/{username}/?__a=1");
            var request = HttpRequestBuilder.Get(uri, mCookieContainer);
            request.Referer = $"https://www.instagram.com/{username}/";
            //request.Headers["X-Requested-With"] = "XMLHttpRequest";
            var response = request.GetResponse() as HttpWebResponse;
            mCookieContainer.Add(response.Cookies);
            // Read data
            using (var gzipStream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
            using (var streamReader = new StreamReader(gzipStream))
            {
                var data = streamReader.ReadToEnd();
                dynamic rootObject = JsonConvert.DeserializeObject<RootObject>(data);
                response.Close();
                return rootObject.user;
            }
        }

        /// <summary>
        /// Get editable info - info in edit page
        /// </summary>
        /// <returns></returns>
        private dynamic GetEditableInfo()
        {
            var uri = new Uri("https://www.instagram.com/accounts/edit/?__a=1");
            var request = HttpRequestBuilder.Get(uri, mCookieContainer);
            request.Referer = $"https://www.instagram.com/{Username}/";
            request.Headers["X-Requested-With"] = "XMLHttpRequest";
            var response = request.GetResponse() as HttpWebResponse;
            mCookieContainer.Add(response.Cookies);
            // Read data
            using (var gzipStream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
            using (var streamReader = new StreamReader(gzipStream))
            {
                var data = streamReader.ReadToEnd();

                dynamic jObject = JsonConvert.DeserializeObject(data);
                var form_data = jObject.form_data;                                
                response.Close();
                return form_data;
            }
        }

        /// <summary>
        /// Change user info
        /// </summary>
        /// <param name="user"></param>
        /// <returns>dynamic data maybe dangerous but i think some time dangerous is good LOL!!! - cause i'm lazy</returns>
        private bool SetEditableUserInfo(dynamic form_data)
        {         
            var chainingEnable = form_data.chaining_enabled.Value ? "on" : "off";
            var data = $"first_name={WebUtility.UrlEncode(form_data.first_name.Value)}&email={form_data.email.Value}&username={WebUtility.UrlEncode(form_data.username.Value)}&phone_number={WebUtility.UrlEncode(form_data.phone_number.Value)}&gender={form_data.gender.Value}&biography={WebUtility.UrlEncode(form_data.biography.Value)}&external_url={WebUtility.UrlEncode(form_data.external_url.Value)}&chaining_enabled={chainingEnable}";
            var content = Encoding.ASCII.GetBytes(data);
            var request = HttpRequestBuilder.Post(new Uri("https://www.instagram.com/accounts/edit/"), mCookieContainer);
            // Missing referer get 403 - forbiden
            request.Referer = "https://www.instagram.com/accounts/edit/";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = content.Length;
            request.KeepAlive = true;
            request.Headers["Origin"] = "https://www.instagram.com";
            request.Headers["X-CSRFToken"] = mCookieContainer.GetCookies(new Uri("https://www.instagram.com"))["csrftoken"].Value;
            request.Headers["X-Instagram-AJAX"] = "1";
            request.Headers["X-Requested-With"] = "XMLHttpRequest";
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(content, 0, content.Length);
                try
                {
                    var response = request.GetResponse() as HttpWebResponse;
                    mCookieContainer.Add(response.Cookies);
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        var responseData = streamReader.ReadToEnd();
                        response.Close();
                        // If we get result, it always return status ok. Otherwise, exception will occur.
                        return responseData == "{\"status\": \"ok\"}";
                    }
                }
                catch (Exception ex)
                {
                    // When you change your username with existed username, you will receive 404 error
                    // and obviously exception will occur. In this case, just return false
                    return false;
                }
            }
        }

        /// <summary>
        /// Change UserBiography
        /// </summary>
        /// <param name="bioGraphy"></param>
        /// <returns></returns>
        public bool SetUserBiography(string biography)
        {
            var editableInfo = GetEditableInfo();
            editableInfo.biography = biography;            
            var setSuccess = SetEditableUserInfo(editableInfo);
            if (setSuccess)
                PublicInfo.biography = editableInfo.biography.Value;
            return setSuccess;
        }

        /// <summary>
        /// Set instagram username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool SetUsername(string username)
        {
            var editableInfo = GetEditableInfo();
            editableInfo.username = username;
            var setSuccess = SetEditableUserInfo(editableInfo);
            if (setSuccess)
            {
                Username = editableInfo.username.Value;
                PublicInfo.username = editableInfo.username.Value;
            }                
            return setSuccess;
        }
    }
}
