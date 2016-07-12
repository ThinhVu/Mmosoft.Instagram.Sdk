using System;
using System.Net;
using static InstagramUser.Instaguser;

namespace InstagramUser
{
    class Program
    {
        static void Main(string[] args)
        {           
            var iUser = new Instaguser("Your username", "You password");
            Console.WriteLine("Set username " + iUser.SetUsername("new username"));
            Console.ReadLine();
        }
    }
}
