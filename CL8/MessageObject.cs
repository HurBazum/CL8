using System.Net;

namespace CL8
{
    public class MessageObject
    {
        public MessageType Type { get; set; }
        public string Content { get; set; }
        
        [NonSerialized]
        public EndPoint EndPoint;
    }
}