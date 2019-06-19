using System;
using System.ComponentModel.DataAnnotations;

namespace chat_application.Identity
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        [MaxLength(50)]
        public string Username { get; set; }
        [MaxLength(144)]
        public string MessageContent { get; set; }
        public DateTime MessageDate { get; set; }
    }
}