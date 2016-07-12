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
        /// Struct UserInfo contain instagram user infomation     
        /// </summary>
        private struct UserInfo
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string PhoneNumber { get; set; }
            public int Gender { get; set; }
            public DateTime BirthDay { get; set; }
            public string Biography { get; set; }
            public string ExternalUrl { get; set; }
            public bool ChainingEnabled { get; set; }

            public UserInfo(string username, string password)
            {
                Username = username;
                Password = password;
                Email = string.Empty;
                FirstName = string.Empty;
                LastName = string.Empty;
                PhoneNumber = string.Empty;
                Gender = 1;
                Biography = string.Empty;
                ExternalUrl = string.Empty;
                ChainingEnabled = true;
                BirthDay = new DateTime(2000, 1, 1);
            }

            public override string ToString()
            {
                return
$@"
FirstName  : {FirstName}
LastName   : {LastName}
Email      : {Email}
Username   : {Username}
Password   : {Password}
PhoneNumber: {PhoneNumber}
Gender     : {Gender}
BirthDay   : {BirthDay}
Biography  : {Biography}
ExternalURL: {ExternalUrl}
ChainingEna: {ChainingEnabled}
";
            }
        }
        /// <summary>
        /// CookieContainer help HttpWebRequest keep session
        /// </summary>
        private CookieContainer mCookieContainer;
        /// <summary>
        /// Store user information
        /// </summary>
        private UserInfo mUserInfo;

        /// <summary>
        /// Create new instance of User
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public Instaguser(string username, string password)
        {
            // Remove Expect: 100-continue header
            System.Net.ServicePointManager.Expect100Continue = false;
            mUserInfo = new UserInfo
            {
                Username = username,
                Password = password
            };

            mCookieContainer = new CookieContainer();
            mCookieContainer.Add(new Cookie("ig-pr", "1") { Domain = "www.instagram.com" });
            mCookieContainer.Add(new Cookie("ig_vw", "1366") { Domain = "www.instagram.com" });
            Bootstrap();
            LogIn();
            GetUserInfo();
        }

        /// <summary>
        /// Store cookies the first time
        /// </summary>
        private void Bootstrap()
        {
            var request = HttpRequestBuilder.Get(new Uri("https://www.instagram.com/accounts/login/"), mCookieContainer);
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers["Upgrade-Insecure-Requests"] = "1";
            var response = request.GetResponse() as HttpWebResponse;
            mCookieContainer.Add(response.Cookies);
            response.Close();
        }

        /// <summary>
        /// Login to store more cookies
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        private void LogIn()
        {
            var data = $"username={mUserInfo.Username}&password={mUserInfo.Password}";
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
        private void GetUserInfo()
        {
            var uri = new Uri("https://www.instagram.com/accounts/edit/?__a=1");
            var request = HttpRequestBuilder.Get(uri, mCookieContainer);
            request.Referer = $"https://www.instagram.com/{mUserInfo.Username}/";
            //request.Headers["X-Requested-With"] = "XMLHttpRequest";
            var response = request.GetResponse() as HttpWebResponse;
            mCookieContainer.Add(response.Cookies);
            // Read data
            using (var gzipStream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
            using (var streamReader = new StreamReader(gzipStream))
            {
                var data = streamReader.ReadToEnd();

                dynamic jObject = JsonConvert.DeserializeObject(data);
                var form_data = jObject.form_data;

                mUserInfo.PhoneNumber = form_data.phone_number;
                mUserInfo.FirstName = form_data.first_name;
                mUserInfo.LastName = form_data.last_name;
                mUserInfo.Gender = form_data.gender;
                mUserInfo.BirthDay = form_data.birthday == null ? new DateTime(2000, 1, 1) : form_data.birthday;
                mUserInfo.ChainingEnabled = form_data.chaining_enabled;
                mUserInfo.Email = form_data.email;
                mUserInfo.Biography = form_data.biography;
                mUserInfo.ExternalUrl = form_data.external_url;

                Console.WriteLine(mUserInfo);

                response.Close();
            }
        }

        /// <summary>
        /// Change user info -- at the moment, this method does not work correctly
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool SetUserInfo(UserInfo user)
        {            
            var chainingEnable = user.ChainingEnabled ? "on" : "off";
            var data = $"first_name={WebUtility.UrlEncode(user.FirstName)}&email={user.Email}&username={WebUtility.UrlEncode(user.Username)}&phone_number={WebUtility.UrlEncode(user.PhoneNumber)}&gender={user.Gender}&biography={WebUtility.UrlEncode(user.Biography)}&external_url={WebUtility.UrlEncode(user.ExternalUrl)}&chaining_enabled={chainingEnable}";
            // first_name=ThinhVu93&email=vutrongthinhk7%40gmail.com&username=thinhvu93aloha&phone_number=%2B84+164+965+3841&gender=2&biography=Gai+Gu+la+phu+du%2C+MU+la+tat+ca&external_url=https%3A%2F%2Fwww.facebook.com%2Ff.i.n.0ne&chaining_enabled=on
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
                var response = request.GetResponse() as HttpWebResponse;
                mCookieContainer.Add(response.Cookies);                
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var responseData = streamReader.ReadToEnd();
                    response.Close();
                    return responseData.Contains("ok");
                }
            }            
        }

        /// <summary>
        /// Change UserBiography
        /// </summary>
        /// <param name="bioGraphy"></param>
        /// <returns></returns>
        public bool SetUserBioGraphy(string bioGraphy)
        {
            var temp = mUserInfo;
            temp.Biography = bioGraphy;
            var setSuccess = SetUserInfo(temp);
            if (setSuccess) mUserInfo = temp;
            return setSuccess;
        }

        /// <summary>
        /// Set instagram username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool SetUsername(string username)
        {
            var temp = mUserInfo;
            temp.Username = username;
            var setSuccess = SetUserInfo(temp);
            if (setSuccess) mUserInfo = temp;
            return setSuccess;
        }
    }
}


// TODO: Add function : "Register new user" - important, Like picture, follow, comment, unfollow, ... dirty work