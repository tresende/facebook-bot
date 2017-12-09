using System;

namespace Bot.Models
{
    public class Message
    {
        public string id { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string type { get; set; }
        public string content { get; set; }
    }
}
