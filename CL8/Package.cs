namespace CL8
{
    public enum QueryType
    {
        GET, 
        POST, 
        PUT, 
        DELETE
    }
    public class Package
    {
        public int Length { get; set; }
        public QueryType Type { get; set; }
        public MessageObject? Message { get; set; }
    }
}