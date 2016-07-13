using System;

namespace InstagramUser
{
    class Program
    {
        static void Main(string[] args)
        {           
            var user = new Instaguser("your username ", "your password");

            // Set user info
            var s = user.SetUserBiography("This is my instagram biography");
            Console.WriteLine(s);

            // Get my user info
            var myInfo = user.PublicInfo;

            // Show my avater
            Console.WriteLine(myInfo.profile_pic_url);

            // Get other user info
            var theRockPublicInfo = user.GetUserPublicInfo("therock");
            var hoobclips = user.GetUserPublicInfo("hoodclips");
            
            Console.ReadLine();
        }
    }
}
