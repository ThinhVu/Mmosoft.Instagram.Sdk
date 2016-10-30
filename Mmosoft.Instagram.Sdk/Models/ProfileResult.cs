namespace Mmosoft.Instagram.Sdk.Models
{
    public class ProfileResult
    {
        public Form_Data form_data { get; set; }

        public class Form_Data
        {
            public string username { get; set; }
            public string phone_number { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public int gender { get; set; }
            public object birthday { get; set; }
            public bool chaining_enabled { get; set; }
            public string email { get; set; }
            public string biography { get; set; }
            public string external_url { get; set; }
        }
    }    
}
