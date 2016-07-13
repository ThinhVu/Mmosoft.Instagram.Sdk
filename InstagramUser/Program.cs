using System;

namespace InstagramUser
{
    class Program
    {
        static void Main(string[] args)
        {           
            var user = new Instaguser("username", "password");
            
            if (user.LogIn())
            {
                Console.WriteLine(user.SetBiography("new biography"));
                Console.WriteLine(user.SetUsername("new username"));                
            }

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
