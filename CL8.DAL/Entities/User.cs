using CL8.Interfaces;

namespace CL8.DAL.Entities
{
    public class User : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
        public ICollection<Message> Messages { get; set; } = [];
    }
}