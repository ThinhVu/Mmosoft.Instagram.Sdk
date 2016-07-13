using System;

namespace InstagramUser
{
    class Program
    {
        static void Main(string[] args)
        {           
            var user = new Instaguser("username", "password");

            var loginRs = user.LogIn();

            // Set user info
            var s = user.SetBiography("Nothing special");
            Console.WriteLine(s);

            // Get my info
            var myInfo1 = user.PublicInfo;            
            var myInfo2 = Instaguser.GetUserPublicInfo(user.Username);
     
            // Get other's info
            var theRock = Instaguser.GetUserPublicInfo("therock");
            var hoobclips = Instaguser.GetUserPublicInfo("hoodclips");
            
            Console.ReadLine();
        }
    }
}
