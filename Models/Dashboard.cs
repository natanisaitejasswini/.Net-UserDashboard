using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspuserdashboard.Models
{
    public abstract class BaseEntity {}

    public class User : BaseEntity
        {
            public int id;
            [Required]
            [MinLength(2)]
            [RegularExpression(@"^[a-zA-Z]+$")]
            public string first_name { get; set; }
            [Required]
            [MinLength(1)]
            [RegularExpression(@"^[a-zA-Z]+$")]
            public string last_name { get; set; }
            [Required]
            [EmailAddress]
            public string email{ get; set; }
            [Required]
            [MinLength(3)]
            public string password { get; set; }
            [Required]
            [Compare("password")]
            public string confirm_password {get; set;}
            public int auth_level;
            public string image;
            public string description;
            public DateTime created_at;
            public DateTime updated_at;
        }
        public class Message :  BaseEntity
        {
            public int id;
            [Required]
            [MinLength(2)]
            public string message { get; set; }
            public DateTime created_at;
            public DateTime updated_at;
            public int user_id {get; set;}
            public string first_name {get; set;}
            public int tag_id {get; set;}
        }
        public class Comment : BaseEntity
        {
            public int id;
            [Required]
            [MinLength(2)]
            public string comment { get; set; }
            public DateTime created_at;
            public DateTime updated_at;
            public int message_id {get; set;}
            public int user_id {get; set;}

            public int temp_id {get; set;}
            public string first_name {get; set;}
        }
}