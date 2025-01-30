using CL8.Interfaces;

namespace CL8.BLL.Infrastructure
{
    public class UserDto : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
