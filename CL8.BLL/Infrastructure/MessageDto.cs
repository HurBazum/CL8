using CL8.Interfaces;

namespace CL8.BLL.Infrastructure
{
    public class MessageDto : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
    }
}