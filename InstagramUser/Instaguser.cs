namespace InstagramUser
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using Newtonsoft.Json;
    using System.IO.Compression;

    public class Instaguser : IInstaguser
    {
        /// <summary>
        /// CookieContainer help HttpWebRequest keep session
        /// </summary>
        private CookieContainer mCoockieC;
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Indicate that user logged on not
        /// </summary>
        private bool LoggedIn;
        /// <summary>
        /// User public info
        /// </summary>
        public User PublicInfo => GetUserPublicInfo(Username);
        
        /// <summary>
        /// Store editable info / in edit page
        /// </summary>
        private dynamic mEditableInfo;

        /// <summary>
        /// Create new instance of instaguser
        /// </summary>
        public Instaguser() : this(string.Empty, string.Empty)
        {
            
        }

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
            mCoockieC = new CookieContainer();            
        }

        /// <summary>
        /// Login to store more cookies
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public bool LogIn()
        {
            try
            {
                // If user logged in, don't need logged anymore
                if (LoggedIn) return true;

                // Bootstrap progress
                var bootstrapRequest = HttpRequestBuilder.Get(new Uri("https://www.instagram.com/accounts/login/"), mCoockieC);
                bootstrapRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                bootstrapRequest.Headers["Upgrade-Insecure-Requests"] = "1";
                var bootstrapResponse = bootstrapRequest.GetResponse() as HttpWebResponse;
                mCoockieC.Add(bootstrapResponse.Cookies);
                bootstrapResponse.Close();

                // Login
                var data = $"username={Username}&password={Password}";
                var content = Encoding.ASCII.GetBytes(data);

                // Create request
                // Missing referer get 403 - forbiden status code
                var request = HttpRequestBuilder.Post(new Uri("https://www.instagram.com/accounts/login/ajax/"), mCoockieC);                
                request.Referer = "https://www.instagram.com/accounts/login/";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = content.Length;
                request.KeepAlive = true;
                request.Headers["Origin"] = "https://www.instagram.com";
                request.Headers["X-CSRFToken"] = mCoockieC.GetCookies(new Uri("https://www.instagram.com"))["csrftoken"].Value;
                request.Headers["X-Instagram-AJAX"] = "1";
                request.Headers["X-Requested-With"] = "XMLHttpRequest";
                
                // Send login data
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(content, 0, content.Length);
                    var response = request.GetResponse() as HttpWebResponse;

                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        var responseData = streamReader.ReadToEnd();
                        response.Close();

                        dynamic dataObject = JsonConvert.DeserializeObject(responseData);
                        LoggedIn = dataObject.authenticated.Value;

                        if (LoggedIn) mCoockieC.Add(response.Cookies);                                                
                        return LoggedIn;
                    }
                }                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// Pull editable info - this method require user logged in, of course
        /// </summary>
        /// <returns></returns>
        public void PullEditableInfo()
        {
            try
            {                
                var uri = new Uri("https://www.instagram.com/accounts/edit/?__a=1");
                var request = HttpRequestBuilder.Get(uri, mCoockieC);
                request.Referer = $"https://www.instagram.com/{Username}/";
                request.Headers["X-Requested-With"] = "XMLHttpRequest";
                var response = request.GetResponse() as HttpWebResponse;
                mCoockieC.Add(response.Cookies);
                // Read data
                using (var gzipStream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                using (var streamReader = new StreamReader(gzipStream))
                {
                    var data = streamReader.ReadToEnd();

                    dynamic jObject = JsonConvert.DeserializeObject(data);
                    mEditableInfo = jObject.form_data;
                    response.Close();                    
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Change user info
        /// </summary>
        /// <param name="user"></param>
        /// <returns>dynamic data maybe dangerous but i think some time dangerous is good LOL!!! - cause i'm lazy</returns>
        private bool PushEditableInfo(dynamic form_data)
        {
            if (!LoggedIn) throw new Exception("User not logged in");
                
            var chainingEnable = form_data.chaining_enabled.Value ? "on" : "off";
            var data = $"first_name={WebUtility.UrlEncode(form_data.first_name.Value)}&email={form_data.email.Value}&username={WebUtility.UrlEncode(form_data.username.Value)}&phone_number={WebUtility.UrlEncode(form_data.phone_number.Value)}&gender={form_data.gender.Value}&biography={WebUtility.UrlEncode(form_data.biography.Value)}&external_url={WebUtility.UrlEncode(form_data.external_url.Value)}&chaining_enabled={chainingEnable}";
            var content = Encoding.ASCII.GetBytes(data);
            var request = HttpRequestBuilder.Post(new Uri("https://www.instagram.com/accounts/edit/"), mCoockieC);
            // Missing referer get 403 - forbiden
            request.Referer = "https://www.instagram.com/accounts/edit/";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = content.Length;
            request.KeepAlive = true;
            request.Headers["Origin"] = "https://www.instagram.com";
            request.Headers["X-CSRFToken"] = mCoockieC.GetCookies(new Uri("https://www.instagram.com"))["csrftoken"].Value;
            request.Headers["X-Instagram-AJAX"] = "1";
            request.Headers["X-Requested-With"] = "XMLHttpRequest";
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(content, 0, content.Length);
                try
                {
                    var response = request.GetResponse() as HttpWebResponse;
                    mCoockieC.Add(response.Cookies);
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
        public bool SetBiography(string newBiography)
        {
            if (mEditableInfo == null) PullEditableInfo();

            // store for rollback purpose
            var oldBiograph = mEditableInfo.biography.Value;

            // update info
            mEditableInfo.biography = newBiography;
            if (PushEditableInfo(mEditableInfo)) return true;

            // update fail -> rollback
            mEditableInfo.biography = oldBiograph;
            return false;
        }

        /// <summary>
        /// Set instagram username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool SetUsername(string username)
        {
            if (mEditableInfo == null) PullEditableInfo();

            var oldUsername = mEditableInfo.username.Value;

            // update info
            mEditableInfo.username = username;
            if (PushEditableInfo(mEditableInfo)) return true;

            // update fail -> rollback
            mEditableInfo.username = oldUsername;
            return false;
        }
       
        public bool Follow(string username)
        {
            throw new NotImplementedException();
        }

        public bool Like(string postId)
        {
            throw new NotImplementedException();
        }

        public bool Comment(string postId, string comment)
        {
            throw new NotImplementedException();
        }

        public bool Report(string postId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieve user public info
        /// This method doesn't require logged in
        /// </summary>
        public static User GetUserPublicInfo(string username)
        {
            var uri = new Uri($"https://www.instagram.com/{username}/?__a=1");
            var request = HttpRequestBuilder.Get(uri, new CookieContainer());
            request.Referer = $"https://www.instagram.com/{username}/";
            request.Headers["X-Requested-With"] = "XMLHttpRequest";
            var response = request.GetResponse() as HttpWebResponse;
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
        /// Allow register new user account
        /// </summary>
        /// <param name="email"></param>
        /// <param name="fullName"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Register(string email, string fullName, string username, string password)
        {
            throw new NotImplementedException();
        }

    }
}