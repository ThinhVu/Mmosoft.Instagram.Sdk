using System.Collections.Generic;

namespace Mmosoft.Instagram.Sdk.Models
{        
    public class PublicInfoResult
    {
        public class EdgeFollowedBy
        {
            public int count { get; set; }
        }

        public class EdgeFollow
        {
            public int count { get; set; }
        }

        public class EdgeMutualFollowedBy
        {
            public int count { get; set; }
            public List<object> edges { get; set; }
        }

        public class PageInfo
        {
            public bool has_next_page { get; set; }
            public object end_cursor { get; set; }
        }

        public class Node2
        {
            public string text { get; set; }
        }

        public class Edge2
        {
            public Node2 node { get; set; }
        }

        public class EdgeMediaToCaption
        {
            public List<Edge2> edges { get; set; }
        }

        public class EdgeMediaToComment
        {
            public int count { get; set; }
        }

        public class Dimensions
        {
            public int height { get; set; }
            public int width { get; set; }
        }

        public class EdgeLikedBy
        {
            public int count { get; set; }
        }

        public class EdgeMediaPreviewLike
        {
            public int count { get; set; }
        }

        public class Owner
        {
            public string id { get; set; }
            public string username { get; set; }
        }

        public class ThumbnailResource
        {
            public string src { get; set; }
            public int config_width { get; set; }
            public int config_height { get; set; }
        }

        public class Node
        {
            public string __typename { get; set; }
            public string id { get; set; }
            public EdgeMediaToCaption edge_media_to_caption { get; set; }
            public string shortcode { get; set; }
            public EdgeMediaToComment edge_media_to_comment { get; set; }
            public bool comments_disabled { get; set; }
            public int taken_at_timestamp { get; set; }
            public Dimensions dimensions { get; set; }
            public string display_url { get; set; }
            public EdgeLikedBy edge_liked_by { get; set; }
            public EdgeMediaPreviewLike edge_media_preview_like { get; set; }
            public object location { get; set; }
            public object gating_info { get; set; }
            public string media_preview { get; set; }
            public Owner owner { get; set; }
            public string thumbnail_src { get; set; }
            public List<ThumbnailResource> thumbnail_resources { get; set; }
            public bool is_video { get; set; }
            public object encoding_status { get; set; }
            public bool is_published { get; set; }
            public string product_type { get; set; }
            public string title { get; set; }
            public double video_duration { get; set; }
            public int video_view_count { get; set; }
        }

        public class Edge
        {
            public Node node { get; set; }
        }

        public class EdgeFelixVideoTimeline
        {
            public int count { get; set; }
            public PageInfo page_info { get; set; }
            public List<Edge> edges { get; set; }
        }

        public class PageInfo2
        {
            public bool has_next_page { get; set; }
            public string end_cursor { get; set; }
        }

        public class Node4
        {
            public string text { get; set; }
        }

        public class Edge4
        {
            public Node4 node { get; set; }
        }

        public class EdgeMediaToCaption2
        {
            public List<Edge4> edges { get; set; }
        }

        public class EdgeMediaToComment2
        {
            public int count { get; set; }
        }

        public class Dimensions2
        {
            public int height { get; set; }
            public int width { get; set; }
        }

        public class EdgeLikedBy2
        {
            public int count { get; set; }
        }

        public class EdgeMediaPreviewLike2
        {
            public int count { get; set; }
        }

        public class Owner2
        {
            public string id { get; set; }
            public string username { get; set; }
        }

        public class ThumbnailResource2
        {
            public string src { get; set; }
            public int config_width { get; set; }
            public int config_height { get; set; }
        }

        public class Node3
        {
            public string __typename { get; set; }
            public string id { get; set; }
            public EdgeMediaToCaption2 edge_media_to_caption { get; set; }
            public string shortcode { get; set; }
            public EdgeMediaToComment2 edge_media_to_comment { get; set; }
            public bool comments_disabled { get; set; }
            public int taken_at_timestamp { get; set; }
            public Dimensions2 dimensions { get; set; }
            public string display_url { get; set; }
            public EdgeLikedBy2 edge_liked_by { get; set; }
            public EdgeMediaPreviewLike2 edge_media_preview_like { get; set; }
            public object location { get; set; }
            public object gating_info { get; set; }
            public string media_preview { get; set; }
            public Owner2 owner { get; set; }
            public string thumbnail_src { get; set; }
            public List<ThumbnailResource2> thumbnail_resources { get; set; }
            public bool is_video { get; set; }
            public int video_view_count { get; set; }
            public string accessibility_caption { get; set; }
        }

        public class Edge3
        {
            public Node3 node { get; set; }
        }

        public class EdgeOwnerToTimelineMedia
        {
            public int count { get; set; }
            public PageInfo2 page_info { get; set; }
            public List<Edge3> edges { get; set; }
        }

        public class PageInfo3
        {
            public bool has_next_page { get; set; }
            public object end_cursor { get; set; }
        }

        public class EdgeSavedMedia
        {
            public int count { get; set; }
            public PageInfo3 page_info { get; set; }
            public List<object> edges { get; set; }
        }

        public class PageInfo4
        {
            public bool has_next_page { get; set; }
            public object end_cursor { get; set; }
        }

        public class EdgeMediaCollections
        {
            public int count { get; set; }
            public PageInfo4 page_info { get; set; }
            public List<object> edges { get; set; }
        }

        public class User
        {
            public string biography { get; set; }
            public bool blocked_by_viewer { get; set; }
            public bool country_block { get; set; }
            public string external_url { get; set; }
            public string external_url_linkshimmed { get; set; }
            public EdgeFollowedBy edge_followed_by { get; set; }
            public bool followed_by_viewer { get; set; }
            public EdgeFollow edge_follow { get; set; }
            public bool follows_viewer { get; set; }
            public string full_name { get; set; }
            public bool has_channel { get; set; }
            public bool has_blocked_viewer { get; set; }
            public int highlight_reel_count { get; set; }
            public bool has_requested_viewer { get; set; }
            public string id { get; set; }
            public bool is_business_account { get; set; }
            public bool is_joined_recently { get; set; }
            public object business_category_name { get; set; }
            public object business_email { get; set; }
            public object business_phone_number { get; set; }
            public object business_address_json { get; set; }
            public bool is_private { get; set; }
            public bool is_verified { get; set; }
            public EdgeMutualFollowedBy edge_mutual_followed_by { get; set; }
            public string profile_pic_url { get; set; }
            public string profile_pic_url_hd { get; set; }
            public bool requested_by_viewer { get; set; }
            public string username { get; set; }
            public object connected_fb_page { get; set; }
            public EdgeFelixVideoTimeline edge_felix_video_timeline { get; set; }
            public EdgeOwnerToTimelineMedia edge_owner_to_timeline_media { get; set; }
            public EdgeSavedMedia edge_saved_media { get; set; }
            public EdgeMediaCollections edge_media_collections { get; set; }
        }

        public class Graphql
        {
            public User user { get; set; }
        }

        public class FelixOnboardingVideoResources
        {
            public string mp4 { get; set; }
            public string poster { get; set; }
        }

        public class RootObject
        {
            public string logging_page_id { get; set; }
            public bool show_suggested_profiles { get; set; }
            public Graphql graphql { get; set; }
            public FelixOnboardingVideoResources felix_onboarding_video_resources { get; set; }
        }
    }
}
