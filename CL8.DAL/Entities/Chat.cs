using CL8.Interfaces;
using System.ComponentModel;

namespace CL8.DAL.Entities
{
    public class Chat : IEntity
    {
        public int Id { get; set; }        
        public string Name { get; set; } = null!;

        [DefaultValue(typeof(string), "This description was stolen. . .")]
        public string Description { get; set; } = null!;
        public ICollection<User> Users { get; set; } = [];
        public ICollection<Message> Messages { get; set; } = [];
    }
}