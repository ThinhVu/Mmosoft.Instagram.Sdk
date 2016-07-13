namespace InstagramUser
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using Newtonsoft.Json;
    using System.IO.Compression;
    using System.Diagnostics;

    public class Instaguser : IInstaguser
    {
        /// <summary>
        /// CookieContainer help HttpWebRequest keep session
        /// </summary>
        private CookieContainer mCoockieC;
        /// <summary>
        /// Indicate that user logged on not
        /// </summary>
        private bool mLoggedIn;
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// User public info
        /// </summary>
        public User PublicInfo => GetUserPublicInfo(Username);

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
            // If user logged in, don't need logged anymore
            if (mLoggedIn) return true;
            try
            {
                // Get bootstrap cookie
                var bootstrapRequest = HttpRequestBuilder.Get(new Uri("https://www.instagram.com/accounts/login/"), mCoockieC);
                bootstrapRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                bootstrapRequest.Headers["Upgrade-Insecure-Requests"] = "1";
                using (var bootstrapResponse = bootstrapRequest.GetResponse() as HttpWebResponse)
                {
                    mCoockieC.Add(bootstrapResponse.Cookies);
                }
            }
            catch (Exception bex)
            {
                Debug.WriteLine("Bootstrap progress meet exception " + bex.Message);
                throw;
            }

            try
            {
                var data = $"username={Username}&password={Password}";
                var content = Encoding.ASCII.GetBytes(data);

                var request = HttpRequestBuilder.Post(new Uri("https://www.instagram.com/accounts/login/ajax/"), mCoockieC);
                // Missing referer get 403 - forbiden status code
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
                    using (var response = request.GetResponse() as HttpWebResponse)
                    using (var responsStream = response.GetResponseStream())
                    using (var streamReader = new StreamReader(responsStream))
                    {
                        var responseData = streamReader.ReadToEnd();

                        dynamic dynObj = JsonConvert.DeserializeObject(responseData);
                        mLoggedIn = dynObj.authenticated.Value;

                        if (mLoggedIn) mCoockieC.Add(response.Cookies);
                        return mLoggedIn;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Login progress occur exception " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get user profile => need logged in
        /// </summary>
        /// <returns></returns>
        private dynamic GetProfile()
        {
            try
            {
                Debug.WriteLine("Pull editable info");
                var uri = new Uri("https://www.instagram.com/accounts/edit/?__a=1");
                var request = HttpRequestBuilder.Get(uri, mCoockieC);
                request.Referer = $"https://www.instagram.com/{Username}/";
                request.Headers["X-Requested-With"] = "XMLHttpRequest";
                request.AllowAutoRedirect = false;
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    mCoockieC.Add(response.Cookies);
                    using (var responseStream = response.GetResponseStream())
                    using (var gzipStream = new GZipStream(responseStream, CompressionMode.Decompress))
                    using (var streamReader = new StreamReader(gzipStream))
                    {
                        var data = streamReader.ReadToEnd();
                        return ((dynamic)JsonConvert.DeserializeObject(data)).form_data;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        /// <param name="user"></param>
        /// <returns>dynamic data maybe dangerous but i think some time dangerous is good LOL!!! - cause i'm lazy</returns>
        private bool UpdateProfile(dynamic form_data)
        {
            Debug.WriteLine("Update profile");

            string chainingEnable = "", data = "";

            try
            {
                chainingEnable = form_data.chaining_enabled.Value ? "on" : "off";
                data = $"first_name={WebUtility.UrlEncode(form_data.first_name.Value)}&email={form_data.email.Value}&username={WebUtility.UrlEncode(form_data.username.Value)}&phone_number={WebUtility.UrlEncode(form_data.phone_number.Value)}&gender={form_data.gender.Value}&biography={WebUtility.UrlEncode(form_data.biography.Value)}&external_url={WebUtility.UrlEncode(form_data.external_url.Value)}&chaining_enabled={chainingEnable}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }

            try
            {
                var content = Encoding.ASCII.GetBytes(data);
                var request = HttpRequestBuilder.Post(new Uri("https://www.instagram.com/accounts/edit/"), mCoockieC);
                request.Referer = "https://www.instagram.com/accounts/edit/";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = content.Length;
                request.KeepAlive = true;
                request.Headers["Origin"] = "https://www.instagram.com";
                // maybe exception if mCookieC not contain csrftoken
                request.Headers["X-CSRFToken"] = mCoockieC.GetCookies(new Uri("https://www.instagram.com"))["csrftoken"].Value;
                request.Headers["X-Instagram-AJAX"] = "1";
                request.Headers["X-Requested-With"] = "XMLHttpRequest";

                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(content, 0, content.Length);

                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        mCoockieC.Add(response.Cookies);
                        using (var responseStream = response.GetResponseStream())
                        using (var streamReader = new StreamReader(responseStream))
                        {
                            // If we get result, it always return status ok. Otherwise, exception will occur.                                           
                            return streamReader.ReadToEnd() == "{\"status\": \"ok\"}";
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                // When you change your username with existed username, you will receive 404 error
                // and obviously exception will occur. In this case, just return false
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Change UserBiography
        /// </summary>
        /// <param name="bioGraphy"></param>
        /// <returns></returns>
        public bool SetBiography(string newBiography)
        {
            try
            {
                dynamic profile = GetProfile();
                profile.biography.Value = newBiography;
                return UpdateProfile(profile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Change biography error " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Set instagram username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool SetUsername(string newUsername)
        {
            try
            {
                dynamic profile = GetProfile();
                profile.username.Value = newUsername;
                bool updateSuccess = UpdateProfile(profile);
                if (updateSuccess) Username = newUsername;
                return updateSuccess;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Change username error " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Follow
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool Follow(string username)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Like
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public bool Like(string postId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Comment
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public bool Comment(string postId, string comment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Report
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public bool Report(string postId)
        {
            throw new NotImplementedException();
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
            using (var response = request.GetResponse() as HttpWebResponse)
            using (var responseStream = response.GetResponseStream())
            using (var gzipStream = new GZipStream(responseStream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(gzipStream))
            {
                var data = streamReader.ReadToEnd();
                return JsonConvert.DeserializeObject<RootObject>(data).user;
            }
        }
    }
}