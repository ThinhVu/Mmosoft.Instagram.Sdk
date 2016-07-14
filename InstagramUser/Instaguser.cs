namespace InstagramUser
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using Newtonsoft.Json;
    using System.IO.Compression;
    using System.Diagnostics;
    using Models;

    public class InstagamUser
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
        /// Create new instance of instaguser
        /// </summary>
        public InstagamUser() : this(string.Empty, string.Empty)
        {

        }

        /// <summary>
        /// Create new instance of User
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public InstagamUser(string username, string password)
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
        public LoginResult LogIn()
        {
            try
            {
                // Get bootstrap cookie
                var bootstrapRequest = HttpRequestBuilder.Get("https://www.instagram.com/accounts/login/", mCoockieC);
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
                throw bex;
            }

            try
            {
                var data = $"username={Username}&password={Password}";
                var content = Encoding.ASCII.GetBytes(data);

                var request = HttpRequestBuilder.Post("https://www.instagram.com/accounts/login/ajax/", mCoockieC);
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

                        return JsonConvert.DeserializeObject<LoginResult>(responseData);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Login progress occur exception " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Get user profile => need logged in
        /// </summary>
        /// <returns></returns>
        private ProfileResult GetProfile()
        {
            Debug.WriteLine("Get profile");
            var request = HttpRequestBuilder.Get("https://www.instagram.com/accounts/edit/?__a=1", mCoockieC);
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
                    return JsonConvert.DeserializeObject<ProfileResult>(data);
                }
            }
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        /// <param name="user"></param>
        /// <returns>dynamic data maybe dangerous but i think some time dangerous is good LOL!!! - cause i'm lazy</returns>
        private UpdateProfileResult UpdateProfile(ProfileResult profile)
        {
            Debug.WriteLine("Update profile");

            string chainingEnable = "", data = "";

            try
            {
                chainingEnable = profile.form_data.chaining_enabled ? "on" : "off";
                data = $"first_name={WebUtility.UrlEncode(profile.form_data.first_name)}&email={profile.form_data.email}&username={WebUtility.UrlEncode(profile.form_data.username)}&phone_number={WebUtility.UrlEncode(profile.form_data.phone_number)}&gender={profile.form_data.gender}&biography={WebUtility.UrlEncode(profile.form_data.biography)}&external_url={WebUtility.UrlEncode(profile.form_data.external_url)}&chaining_enabled={chainingEnable}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }

            try
            {
                var content = Encoding.ASCII.GetBytes(data);
                var request = HttpRequestBuilder.Post("https://www.instagram.com/accounts/edit/", mCoockieC);
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
                            var responseData = streamReader.ReadToEnd();
                            return JsonConvert.DeserializeObject<UpdateProfileResult>(responseData);                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // When you change your username with existed username, you will receive 404 error
                // and obviously exception will occur. In this case, just return false
                Debug.WriteLine(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Change UserBiography
        /// </summary>
        /// <param name="bioGraphy"></param>
        /// <returns></returns>
        public UpdateProfileResult SetBiography(string newBiography)
        {
            try
            {
                ProfileResult profile = GetProfile();
                profile.form_data.biography = newBiography;
                return UpdateProfile(profile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Change biography error " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Set instagram username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public UpdateProfileResult SetUsername(string newUsername)
        {
            try
            {
                ProfileResult profile = GetProfile();
                profile.form_data.username = newUsername;
                var updateProfileResult = UpdateProfile(profile);
                if (updateProfileResult.status == "ok") Username = newUsername;
                return updateProfileResult;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Change username error " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Follow
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public FollowResult Follow(string username)
        {
            try
            {
                Debug.WriteLine("Following " + username);
                var publicInfo = GetPublicInfo(username);
                var request = HttpRequestBuilder.Post($"https://www.instagram.com/web/friendships/{publicInfo.user.id}/follow/", mCoockieC);
                request.Referer = $"https://www.instagram.com/{publicInfo.user.username}/";
                request.Headers["X-CSRFToken"] = mCoockieC.GetCookies(new Uri("https://www.instagram.com"))["csrftoken"].Value;
                request.Headers["X-Instagram-AJAX"] = "1";
                request.Headers["X-Requested-With"] = "XMLHttpRequest";
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    mCoockieC.Add(response.Cookies);
                    if (request.HaveResponse)
                    {
                        using (var responseStream = response.GetResponseStream())
                        using (var streamReader = new StreamReader(responseStream))
                        {
                            var responseData = streamReader.ReadToEnd();
                            return JsonConvert.DeserializeObject<FollowResult>(responseData);
                        }
                    }
                    else
                    {
                        throw new WebException("no response");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Like
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public LikeResult Like(string postId)
        {
            // Note that to get postId, you simple get postlink then add /?__a=1   
            // result return is jsonresult contain all infomation about this post.
            // you can like, comment,... example            
            // https://www.instagram.com/p/BHtOw-7gKGs/?__a=1 
            var request = HttpRequestBuilder.Post($"https://www.instagram.com/web/likes/{postId}/like/", mCoockieC);
            request.Referer = $"https://www.instagram.com";
            request.Headers["X-CSRFToken"] = mCoockieC.GetCookies(new Uri("https://www.instagram.com"))["csrftoken"].Value;
            request.Headers["X-Instagram-AJAX"] = "1";
            request.Headers["X-Requested-With"] = "XMLHttpRequest";
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                mCoockieC.Add(response.Cookies);
                if (request.HaveResponse)
                {
                    using (var responseStream = response.GetResponseStream())
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        var responseData = streamReader.ReadToEnd();
                        return JsonConvert.DeserializeObject<Models.LikeResult>(responseData);
                    }
                }
                else
                {
                    throw new WebException("no response");
                }
            }
        }

        /// <summary>
        /// Comment
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public CommentResult Comment(string postId, string comment)
        {
            var data = "comment_text=" + WebUtility.UrlEncode(comment);
            var content = Encoding.ASCII.GetBytes(data);
            var request = HttpRequestBuilder.Post($"https://www.instagram.com/web/comments/{postId}/add/", mCoockieC);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Referer = $"https://www.instagram.com";
            request.ContentLength = content.Length;
            request.Headers["X-CSRFToken"] = mCoockieC.GetCookies(new Uri("https://www.instagram.com"))["csrftoken"].Value;
            request.Headers["X-Instagram-AJAX"] = "1";
            request.Headers["X-Requested-With"] = "XMLHttpRequest";
            using (var stream = request.GetRequestStream())
            {
                stream.Write(content, 0, content.Length);
            }
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                mCoockieC.Add(response.Cookies);
                if (request.HaveResponse)
                {
                    using (var responseStream = response.GetResponseStream())
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        var responseData = streamReader.ReadToEnd();
                        return JsonConvert.DeserializeObject<Models.CommentResult>(responseData);
                    }
                }
                else
                {
                    throw new WebException("no response");
                }
            }
        }

        /// <summary>
        /// Report - return Models.ReportResult
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public bool Report(string postId)
        {
            // I'm a good man, i don't test report XD
            throw new NotImplementedException();
        }

        /// <summary>
        /// Allow register new user account -- return Models.RegisterResult
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
        public static PublicInfoResult GetPublicInfo(string username)
        {
            var request = HttpRequestBuilder.Get($"https://www.instagram.com/{username}/?__a=1");
            request.Referer = $"https://www.instagram.com/{username}/";
            request.Headers["X-Requested-With"] = "XMLHttpRequest";
            using (var response = request.GetResponse() as HttpWebResponse)
            using (var responseStream = response.GetResponseStream())
            using (var gzipStream = new GZipStream(responseStream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(gzipStream))
            {
                var data = streamReader.ReadToEnd();
                return JsonConvert.DeserializeObject<PublicInfoResult>(data);
            }
        }
    }
}