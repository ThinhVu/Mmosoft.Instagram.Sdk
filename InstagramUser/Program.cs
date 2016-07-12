using System;

namespace InstagramUser
{          
    class Program
    {
        static void Main(string[] args)
        {                     
            var iUser = new User("Your_UserName","Your_Password");
            // At the moment, this function does not work correctly.
            var changeExUrlResult = iUser.ChangeExternalUrl("https://abc.com");
            Console.WriteLine("Change success? " + changeExUrlResult);                                   
            Console.ReadLine();
        }
    }    
}
