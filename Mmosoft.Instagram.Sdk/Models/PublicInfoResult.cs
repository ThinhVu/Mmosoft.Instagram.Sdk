using System.Collections.Generic;

namespace Mmosoft.Instagram.Sdk.Models
{        
    public class PublicInfoResult
    {
        public User user { get; set; }

        public class User
        {
            public string username { get; set; }
            public Follows follows { get; set; }
            public bool requested_by_viewer { get; set; }
            public FollowedBy followed_by { get; set; }
            public object country_block { get; set; }
            public bool has_requested_viewer { get; set; }
            public string external_url_linkshimmed { get; set; }
            public bool follows_viewer { get; set; }
            public string profile_pic_url { get; set; }
            public string external_url { get; set; }
            public bool is_private { get; set; }
            public string full_name { get; set; }
            public Media media { get; set; }
            public bool has_blocked_viewer { get; set; }
            public bool followed_by_viewer { get; set; }
            public bool is_verified { get; set; }
            public string id { get; set; }
            public string biography { get; set; }
            public bool blocked_by_viewer { get; set; }
        }

        public class Follows
        {
            public int count { get; set; }
        }

        public class FollowedBy
        {
            public int count { get; set; }
        }

        public class PageInfo
        {
            public bool has_previous_page { get; set; }
            public object start_cursor { get; set; }
            public object end_cursor { get; set; }
            public bool has_next_page { get; set; }
        }

        public class Media
        {
            public int count { get; set; }
            public PageInfo page_info { get; set; }
            public List<object> nodes { get; set; }
        }        
    }
}
