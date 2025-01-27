using CL8.Interfaces;

namespace CL8.DAL.Entities
{
    public class Message : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}