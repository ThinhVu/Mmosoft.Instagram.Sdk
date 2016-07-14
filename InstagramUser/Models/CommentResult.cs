namespace InstagramUser.Models
{
    public class CommentResult
    {
        public class From
        {
            public string username { get; set; }
            public string profile_picture { get; set; }
            public string id { get; set; }
            public string full_name { get; set; }
        }

        public int created_time { get; set; }
        public string text { get; set; }
        public string status { get; set; }
        public From from { get; set; }
        public string id { get; set; }
    } 
}
