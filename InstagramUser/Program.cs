using System;

namespace InstagramUser
{
    class Program
    {
        static void Main(string[] args)
        {
            ReportTest();
            Console.ReadLine();
        }

        static void LoginTest()
        {
            var user = new InstagamUser("username", "password");
            var loginResult = user.LogIn();
            if (loginResult.authenticated)
            {
                Console.WriteLine(user.SetBiography("new biography"));
                Console.WriteLine(user.SetUsername("new username"));
            }                       
        }   

        static void GetPublicInfoTest()
        {
            var theRockpublicInfo = InstagamUser.GetPublicInfo("therock");
            var hoobclipspublicInfo = InstagamUser.GetPublicInfo("hoodclips");
        }
        
        static void SetUserInfoTest()
        {
            var user = new InstagamUser("usernaem", "password");

            var loginResult = user.LogIn();
            if (loginResult.authenticated)
            {
                var setBioGraphyResult = user.SetBiography("new biography");
                Console.WriteLine(setBioGraphyResult.status);

                var setUsernameResult = user.SetUsername("new username");
                Console.WriteLine(setUsernameResult.status);
            }

            Console.ReadLine();
        }

        static void FollowTest()
        {
            var user = new InstagamUser("username", "password");
            
            var loginResult = user.LogIn();
            if (loginResult.authenticated)
            {                
                var followResult = user.Follow("kawanocy");
                Console.WriteLine(followResult.status);
                Console.WriteLine(followResult.result);
                var likeResult = user.Like("1291753606881517996");
            }
                        
            Console.ReadLine();
        }  
        
        static void LikeTest()
        {
            var user = new InstagamUser("username", "password");

            var loginResult = user.LogIn();
            if (loginResult.authenticated)
            {                
                var likeResult = user.Like("1291753606881517996");
                Console.WriteLine(likeResult.status);
            }

            Console.ReadLine();
        }  

        static void CommentTest()
        {
            var user = new InstagamUser("username", "password");

            var loginResult = user.LogIn();
            if (loginResult.authenticated)
            {
                var commentResult = user.Comment("1291753606881517996", "Good art");
                Console.WriteLine(commentResult.status);
                Console.WriteLine(commentResult.id);
                Console.WriteLine(commentResult.text);
                Console.WriteLine(commentResult.created_time);

                var commentFrom = commentResult.from;
                Console.WriteLine(commentFrom.id);
                Console.WriteLine(commentFrom.full_name);
                Console.WriteLine(commentFrom.username);
                Console.WriteLine(commentFrom.profile_picture);
            }

            Console.ReadLine();
        }

        static void ReportTest()
        {
            var user = new InstagamUser("username", "password");        
            var loginResult = user.LogIn();
            if (loginResult.authenticated)
            {
                var commentResult = user.Report("1289832948493489827", Models.ReportReasonId.NudityOrPornography);
                Console.WriteLine(commentResult.status);
            }

            Console.ReadLine();
        }
    }
}
