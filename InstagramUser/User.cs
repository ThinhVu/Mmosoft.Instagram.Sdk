namespace InstagramUser
{    
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using Newtonsoft.Json;

    public class User
    {       
        /// <summary>
        /// Store user information        
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
            public string ChainingEnabled { get; set; }     

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
                ChainingEnabled = "on";
                BirthDay = DateTime.Now;
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
        
        private CookieCollection mCookieCollection;        
        private UserInfo mUserInfo;
        
        /// <summary>
        /// Create new instance of User
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public User(string username, string password)
        {
            mUserInfo = new UserInfo
            {
                Username = username,
                Password = password
            };
            // Bootstrap 
            mCookieCollection = new CookieCollection();
            mCookieCollection = Get("https://www.instagram.com/accounts/login/").Cookies;
            Login();
            GetUserInfo();
        }
        
        /// <summary>
        /// Change username
        /// </summary>
        /// <param name="username"></param>
        public bool ChangeUsername(string username)
        {
            Console.WriteLine("Change username to " + username);
            var temp = mUserInfo;
            temp.Username = username;
            return ChangeUserInfo(temp);            
        }

        /// <summary>
        /// Change External Url
        /// </summary>
        /// <param name="externalUrl"></param>
        public bool ChangeExternalUrl(string externalUrl)
        {
            Console.WriteLine("Change ex_url to " + externalUrl);
            var temp = mUserInfo;
            temp.ExternalUrl = externalUrl;
            return ChangeUserInfo(temp);            
        }

        /// <summary>
        /// Change user info -- at the moment, this method does not work correctly
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool ChangeUserInfo(UserInfo user)
        {
            // Note that i do not update birthday -- still need update
            var data = $"first_name={mUserInfo.FirstName}&email={mUserInfo.Email}&username={mUserInfo.Username}&phone_number={mUserInfo.PhoneNumber}&gender={mUserInfo.Gender}&biography={mUserInfo.Biography}&external_url={mUserInfo.ExternalUrl}&chaining_enabled={mUserInfo.ChainingEnabled}";
            var response = Post("https://www.instagram.com/accounts/edit", data);
            mCookieCollection.Add(response.Cookies);
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                // Log
                Console.WriteLine(result);
                if (result.Contains("ok"))
                {
                    mUserInfo = user;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Login to store more cookies
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        private void Login()
        {
            var data = $"username={mUserInfo.Username}&password={mUserInfo.Password}";
            var response = Post("https://www.instagram.com/accounts/login/ajax/", data);
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                // Store cookie -- Now use can using to make request.
                mCookieCollection.Add(response.Cookies);

                // Log
                foreach (Cookie item in mCookieCollection)
                    Console.WriteLine(item.Name + ":" + item.Value);

                Console.WriteLine(streamReader.ReadToEnd());
            }
        }

        /// <summary>
        /// Retrieve user's infor
        /// </summary>
        private void GetUserInfo()
        {
            HttpWebResponse response = Get("https://www.instagram.com/accounts/edit/?wo=1");
            var html = string.Empty;
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                html = streamReader.ReadToEnd();

                // TODO : Fix parse info
                // I'm very busy so i do not take a time to extract data.
                // Instead, i parse string to json object then using dynamic to get info   
                // Exception may occur each line of code XD

                // We can not extract data from DOM because what you see is not what you get.
                // We need extract data from javascript object.
                var openSquare = html.IndexOf("[{\"form_data\":");
                var closeSquare = html.IndexOf("}]", openSquare);
                var data = html.Substring(openSquare + 1, closeSquare - openSquare);                

                // And now get dynamic object - lazy way
                dynamic jObject = JsonConvert.DeserializeObject(data);
                mUserInfo.PhoneNumber = jObject.form_data.phone_number;
                mUserInfo.FirstName = jObject.form_data.first_name;
                mUserInfo.LastName = jObject.form_data.last_name;
                mUserInfo.Biography = jObject.form_data.biography;
                mUserInfo.Gender = jObject.form_data.gender;
                mUserInfo.ChainingEnabled = jObject.form_data.chaining_enabled;
                mUserInfo.Email = jObject.form_data.email;
                mUserInfo.BirthDay = jObject.form_data.birthday == null ? DateTime.Now : jObject.form_data.birthday;
                mUserInfo.ExternalUrl = jObject.form_data.external_url;
             
                // Log 
                Console.WriteLine(mUserInfo);
            }
        }

        /// <summary>
        /// Send get message
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private HttpWebResponse Get(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = true;
            request.Headers.Add("Upgrade-Insecure-Requests", "1");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");

            // Store cookies
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(this.mCookieCollection);

            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// Send post message
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        private HttpWebResponse Post(string url, string postData)
        {
            // Compute data
            byte[] content = Encoding.ASCII.GetBytes(postData);

            // Make rq
            var rq = (HttpWebRequest)WebRequest.Create(url);

            // Option
            rq.Method = "POST";
            rq.ProtocolVersion = HttpVersion.Version11;

            // Headers
            rq.Host = "www.instagram.com";
            rq.KeepAlive = true;
            rq.ContentLength = content.Length;
            rq.Headers.Add("Origin", "https://www.instagram.com");
            rq.Headers.Add("X-Instagram-AJAX", "1");
            rq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:41.0) Gecko/20100101 Firefox/41.0";
            rq.ContentType = "application/x-www-form-urlencoded";
            rq.Accept = "*/*";
            rq.Headers.Add("X-Requested-With", "XMLHttpRequest");
            rq.Headers.Add("X-CSRFToken", mCookieCollection["csrftoken"].Value);
            rq.Referer = "https://www.instagram.com/";
            rq.Headers.Add("Accept-Language", "en -US,en;q=0.5");

            // Need bonus cookie
            rq.CookieContainer = new CookieContainer();
            rq.CookieContainer.Add(this.mCookieCollection);

            // Send request
            Stream requestStream = rq.GetRequestStream();
            requestStream.Write(content, 0, content.Length);
            requestStream.Close();

            // return response
            return rq.GetResponse() as HttpWebResponse;
        }
    }
}

// TODO: Add function : "Register new user" - important, Like picture, follow, comment, unfollow, ... dirty work